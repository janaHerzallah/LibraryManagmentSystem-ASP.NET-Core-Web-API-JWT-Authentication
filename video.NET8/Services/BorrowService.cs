using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public BorrowService(ApplicationDbContext context , IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.Active);

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }

                // this code segment has been placed here 
                // to check if the member has been late more than 4 times
                // out side the member will out of scope of the member

                if (member.OverDueCount > 4) // Can't borrow if late more than 4 times
                {
                    throw new Exception("You can't borrow a book because you have been late more than 4 times.");
                }
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.BookId && b.Active);

            if (book == null || book.AvailableCopies < 1)
            {
                throw new Exception("Book not available for borrowing.");
            }

            // **Restriction: Claimed return date must be at least 2 days ahead**
            if ((request.ClaimedReturnDate - DateTime.UtcNow).TotalDays < 2)
            {
                throw new ArgumentException("The claimed return date must be at least 2 days from now.");
            }

            if ((request.ClaimedReturnDate - DateTime.UtcNow).TotalDays > 15)
            {
                throw new ArgumentException("You can't borrow a book for more than 15 days.");
            }

            // **Check if the book was already borrowed by the member and is still active (i.e., not returned)**
            var activeBorrowRecord = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BookId == request.BookId && b.MemberId == request.MemberId && b.Active);

            if (activeBorrowRecord != null)
            {
                throw new InvalidOperationException("You already have an active borrow record for this book.");
            }

            // All conditions met, proceed with borrowing the book
            book.AvailableCopies -= 1;

            var borrowRecord = new Borrow
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                BorrowDate = DateTime.UtcNow,
                Active = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                ClaimedReturnDate = request.ClaimedReturnDate
            };

            _context.BookBorrows.Add(borrowRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return new BorrowBookResponse
            {
                bookId = borrowRecord.Id,
                MemberId = borrowRecord.MemberId,
                BorrowDate = borrowRecord.BorrowDate,
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title,
                ClaimedReturnDate = borrowRecord.ClaimedReturnDate
            };
        }


        public async Task<ReturnBookResponse> ReturnBook(int memberId, int bookId, string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == memberId && m.Active);

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            // Proceed with the return book logic
            var borrowRecord = await _context.BookBorrows.FirstOrDefaultAsync(b => b.MemberId == memberId && b.BookId == bookId && b.Active);
            if (borrowRecord == null)
            {
                throw new Exception("Borrow record not found or its already returned");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.Active);
            if (book == null)
            {
                throw new Exception("Book record not found.");
            }

            borrowRecord.ActualReturnDate = DateTime.UtcNow;
            borrowRecord.Active = false; // set active to false means its not borrowed any more so its returned
            book.AvailableCopies += 1;
            book.DateModified = DateTime.UtcNow;

            _context.BookBorrows.Update(borrowRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            // Check for overdue
            if (borrowRecord.ActualReturnDate > borrowRecord.ClaimedReturnDate)
            {
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId);
                if (member != null)
                {
                    member.OverDueCount += 1;
                    _context.Members.Update(member);
                    await _context.SaveChangesAsync();
                }
            }

          

            return new ReturnBookResponse
            {
                bookId = borrowRecord.Id,
                MemberId = borrowRecord.MemberId,
                BorrowDate = borrowRecord.BorrowDate,
                ReturnDate = borrowRecord.ActualReturnDate == DateTime.MinValue ? "Not Returned" : borrowRecord.ActualReturnDate.ToString("yyyy-MM-dd"),
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title,
                ClaimedReturnDate = borrowRecord.ClaimedReturnDate,
                
            };
        }


        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetMembersBorrowedBooks(int memberId, string token)
        {
            // get all borrowed books either its retruned or not // so even if its active or not
            // validate if its an admin or if its a member it should be the same member requesting the service for 


            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == memberId );

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            return await _context.BookBorrows
                         .Where(b => b.MemberId == memberId )
                         .Include(b => b.Book)
                         .Select(b => new GetBorrowedBooksForAMemberResponse
                         {
                             MemberId = b.MemberId,
                             bookId = b.BookId,
                             BorrowDate = b.BorrowDate,
                             ReturnDate = b.ActualReturnDate == DateTime.MinValue ? "Not Returned" : b.ActualReturnDate.ToString("yyyy-MM-dd"),
                             DateCreated = b.DateCreated,
                             DateModified = b.DateModified,
                             availableCopies = b.Book.AvailableCopies,
                             Title = b.Book.Title ,
                             ClaimedReturnDate = b.ClaimedReturnDate
                         })
                         .ToListAsync();
        }



    }
}
