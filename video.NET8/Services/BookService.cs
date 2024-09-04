using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetAllBooksResponse>> GetAllBooks()
        {
           
           var list =  await _context.Books.Where(b => b.Active == true).ToListAsync();
            return list.Select(b => new GetAllBooksResponse
            {
                Id = b.Id,
                Title = b.Title,
                AuthorId = b.AuthorId,
                CategoryId = b.CategoryId,
                LibraryBranchId = b.LibraryBranchId,
                AvailableCopies = b.AvailableCopies,
                TotalCopies = b.TotalCopies,
                CreatedAt = b.DateCreated,
                ModifiedAt = b.DateModified
            });
        }

        public async  Task<GetBookByIdResponse> GetBookById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author) // Include the Author entity
                .FirstOrDefaultAsync(b => b.Id == id && b.Active);

            if (book == null)
            {
                throw new KeyNotFoundException("Book not found or is inactive.");
            }
            return new GetBookByIdResponse { 


                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author != null ? book.Author.Name : null,
                CategoryId = book.CategoryId,
                LibraryBranchId = book.LibraryBranchId,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                CreatedAt = book.DateCreated,
                ModifiedAt = book.DateModified
            };
        }

        public async Task<AddBookResponse> AddBookByAdmin(AddBookRequest book)
        {
            if (book.TotalCopies < book.AvailableCopies)
            {
                throw new ArgumentException("Total copies have to be equal or more than Available ones");
            }
            Book bookDataBase = new Book
            {
                Title = book.Title,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                LibraryBranchId = book.LibraryBranchId,
                AvailableCopies = book.AvailableCopies ?? 0,
                TotalCopies = book.TotalCopies ?? 0,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Active=true
                


            };

            bookDataBase.Active = true;
            _context.Books.Add(bookDataBase);
            await _context.SaveChangesAsync();


            AddBookResponse response = new AddBookResponse
            {
                Title = bookDataBase.Title,
                AvailableCopies = bookDataBase.AvailableCopies,
                TotalCopies = bookDataBase.TotalCopies,
                AuthorId = bookDataBase.AuthorId,
                CategoryId = bookDataBase.CategoryId,
                LibraryBranchId = bookDataBase.LibraryBranchId,
                CreatedAt = bookDataBase.DateCreated,
                ModifiedAt = bookDataBase.DateModified,
                Id = bookDataBase.Id
            };
            return response;
        }

        public async Task<AddBookResponse> UpdateBookByAdmin(int id, UpdateBookRequest updatedBook)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found or is inactive.");
            }

            book.Title = updatedBook.Title;
            book.AvailableCopies = updatedBook.AvailableCopies;
            book.TotalCopies = updatedBook.TotalCopies;
            book.Active= updatedBook.active;
            book.DateModified = DateTime.UtcNow;
            book.CategoryId = updatedBook.CategoryId;
            book.AuthorId = updatedBook.AuthorId;
            book.LibraryBranchId = updatedBook.LibraryBranchId;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return new  AddBookResponse
            {
                Id = book.Id,
                Title = updatedBook.Title,
                AvailableCopies = updatedBook.AvailableCopies,
                TotalCopies = updatedBook.TotalCopies,
                AuthorId = updatedBook.AuthorId,
                CategoryId = updatedBook.CategoryId,
                LibraryBranchId = updatedBook.LibraryBranchId,
                CreatedAt = book.DateCreated,
                ModifiedAt = book.DateModified
            };
            
        }

        public async Task<bool> DeleteBookByAdmin(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.Active);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SoftDeleteBookByAdmin(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.Active);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found or is already inactive.");
            }

            book.Active = false;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<GetAllBooksResponse>> GetActiveAndInactive()
        {

            var list = await _context.Books.ToListAsync();
            return list.Select(b => new GetAllBooksResponse
            {
                Id = b.Id,
                Title = b.Title,
                AuthorId = b.AuthorId,
                CategoryId = b.CategoryId,
                LibraryBranchId = b.LibraryBranchId,
                AvailableCopies = b.AvailableCopies,
                TotalCopies = b.TotalCopies,
                CreatedAt = b.DateCreated,
                ModifiedAt = b.DateModified
            });
        }
    }
}
