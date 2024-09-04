using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly ApplicationDbContext _context;

        public BorrowService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, string token)
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
                throw new Exception("Unauthorized user.");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.BookId && b.Active);

            if (book == null || book.AvailableCopies < 1)
            {
                throw new Exception("Book not available for borrowing.");
            }
            

            if (member.OverDueCount > 4 ) // you can't be late more than 4 times !!!
            {
                throw new Exception("You can't borrow a book because you have been late more than 4 times.");
            }

            if ((request.ClaimedReturnDate - DateTime.UtcNow).TotalDays > 15)
            {
                throw new ArgumentException("You can't borrow a book for more than 15 days.");
            }

            // if all conditions are met , then proceed with borrowing the book
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
                bookId= borrowRecord.Id,
                MemberId = borrowRecord.MemberId,
                BorrowDate = borrowRecord.BorrowDate,
                 
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title,
                ClaimedReturnDate = borrowRecord.ClaimedReturnDate
               
                
            };
        }

        public async Task<ReturnBookResponse> ReturnBook(int memberId, int bookId , string token)
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
                throw new Exception("Unauthorized user.");
            }

            var borrowRecord = await _context.BookBorrows.FirstOrDefaultAsync(b => b.MemberId == memberId && b.BookId == bookId && b.Active);
            if (borrowRecord == null)
            {
                throw new Exception("Borrow record not found.");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.Active);
            if (book == null)
            {
                throw new Exception("Book record not found.");
            }

            borrowRecord.ActualReturnDate = DateTime.UtcNow;
            borrowRecord.Active = true;
            book.AvailableCopies += 1;
            book.DateModified = DateTime.UtcNow;

            _context.BookBorrows.Update(borrowRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            

            // Check for overdue
            if (borrowRecord.ActualReturnDate > borrowRecord.ClaimedReturnDate)
            {
               
                if (member != null)
                {
                    //if (member.OverDueCount == null)
                    //{
                    //    member.OverDueCount = 0;
                    //}
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
                ReturnDate = borrowRecord.ActualReturnDate,
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title,
                ClaimedReturnDate = borrowRecord.ClaimedReturnDate
            };
        }

        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksByMember(int memberId, string token)
        {
            // get all borrowed books either its retruned or not
            // edit the result to get details of the borrowed books

            return await _context.BookBorrows
                         .Where(b => b.MemberId == memberId && b.Active)
                         .Include(b => b.Book)
                         .Select(b => new GetBorrowedBooksForAMemberResponse
                         {
                             MemberId = b.MemberId,
                             bookId = b.BookId,
                             BorrowDate = b.BorrowDate,
                             ReturnDate = b.ActualReturnDate,
                             DateCreated = b.DateCreated,
                             DateModified = b.DateModified,
                             availableCopies = b.Book.AvailableCopies,
                             Title = b.Book.Title
                         })
                         .ToListAsync();
        }
    }
}
