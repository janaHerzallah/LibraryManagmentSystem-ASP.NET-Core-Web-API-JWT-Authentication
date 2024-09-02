using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
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

        public async Task<BorrowBookResponse> BorrowBook(int memberId, int bookId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.Active);

            if (book == null || book.AvailableCopies < 1)
            {
                throw new Exception("Book not available for borrowing.");
            }

            book.AvailableCopies -= 1;

            var borrowRecord = new Borrow
            {
                MemberId = memberId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow,
                Active = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
                
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
                Title = book.Title
                
                
            };
        }

        public async Task<ReturnBookResponse> ReturnBook(int memberId, int bookId)
        {
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

            borrowRecord.ReturnDate = DateTime.UtcNow;
            borrowRecord.Active = true;
            book.AvailableCopies += 1;
            book.DateModified = DateTime.UtcNow;

            _context.BookBorrows.Update(borrowRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return new ReturnBookResponse
            {
                bookId = borrowRecord.Id,
                MemberId = borrowRecord.MemberId,
                BorrowDate = borrowRecord.BorrowDate,
                ReturnDate = borrowRecord.ReturnDate,
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title
            };
        }

        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksByMember(int memberId)
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
                             ReturnDate = b.ReturnDate,
                             DateCreated = b.DateCreated,
                             DateModified = b.DateModified,
                             availableCopies = b.Book.AvailableCopies,
                             Title = b.Book.Title
                         })
                         .ToListAsync();
        }
    }
}
