using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        public async Task<IEnumerable<GetAllBooksResponse>> GetActiveBooks()
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
                .FirstOrDefaultAsync(b => b.Id == id);

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

        public async Task<AddBookResponse> AddBook(AddBookRequest book)
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

        public async Task<AddBookResponse> UpdateBook(int id, UpdateBookRequest updatedBook)
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

        public async Task<bool> DeleteBook(int id)
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

        public async Task SoftDeleteBook(int id)
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


        public async Task<IEnumerable<GetAllBooksResponse>> GetAllBooks()
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

        public async Task<(List<AddBookRequest> validBooks, List<validationErrorListResonse> validationErrors)> ProcessExcelFileAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validBooks = new List<AddBookRequest>();
            var validationErrorList = new List<validationErrorListResonse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var errorMessage = string.Empty;
                        var haveError = false;

                        // Validate Title
                        var titleText = worksheet.Cells[row, 1].Text;
                        if (string.IsNullOrWhiteSpace(titleText))
                        {
                            errorMessage += "Title is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(titleText, out _))
                        {
                            errorMessage += "Title must be a string, not a number. ";
                            haveError = true;
                        }

                        // Validate Available Copies
                        var availableCopiesText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrWhiteSpace(availableCopiesText))
                        {
                            errorMessage += "Available Copies are required. ";
                            haveError = true;
                        }
                        else if (!int.TryParse(availableCopiesText, out _))
                        {
                            errorMessage += "Available Copies must be an integer. ";
                            haveError = true;
                        }

                        // Validate Total Copies
                        var totalCopiesText = worksheet.Cells[row, 3].Text;
                        if (string.IsNullOrWhiteSpace(totalCopiesText))
                        {
                            errorMessage += "Total Copies are required. ";
                            haveError = true;
                        }
                        else if (!int.TryParse(totalCopiesText, out _))
                        {
                            errorMessage += "Total Copies must be an integer. ";
                            haveError = true;
                        }

                        // Validate Author ID
                        var authorIdText = worksheet.Cells[row, 4].Text;
                        if (string.IsNullOrWhiteSpace(authorIdText))
                        {
                            errorMessage += "Author ID is required. ";
                            haveError = true;
                        }
                        else if (!int.TryParse(authorIdText, out _))
                        {
                            errorMessage += "Author ID must be an integer. ";
                            haveError = true;
                        }

                        // Validate Category ID
                        var categoryIdText = worksheet.Cells[row, 5].Text;
                        if (string.IsNullOrWhiteSpace(categoryIdText))
                        {
                            errorMessage += "Category ID is required. ";
                            haveError = true;
                        }
                        else if (!int.TryParse(categoryIdText, out _))
                        {
                            errorMessage += "Category ID must be an integer. ";
                            haveError = true;
                        }

                        // Validate Library Branch ID
                        var libraryBranchIdText = worksheet.Cells[row, 6].Text;
                        if (string.IsNullOrWhiteSpace(libraryBranchIdText))
                        {
                            errorMessage += "Library Branch ID is required. ";
                            haveError = true;
                        }
                        else if (!int.TryParse(libraryBranchIdText, out _))
                        {
                            errorMessage += "Library Branch ID must be an integer. ";
                            haveError = true;
                        }

                        // If there are errors, add to the error list
                        if (haveError)
                        {
                            validationErrorList.Add(new validationErrorListResonse
                            {
                                RowNumber = row,
                                Title = titleText,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create book request object
                        var bookRequest = new AddBookRequest
                        {
                            Title = titleText,
                            AvailableCopies = TryParseInt(availableCopiesText),
                            TotalCopies = TryParseInt(totalCopiesText),
                            AuthorId = TryParseInt(authorIdText),
                            CategoryId = TryParseInt(categoryIdText),
                            LibraryBranchId = TryParseInt(libraryBranchIdText)
                        };

                        validBooks.Add(bookRequest); // Add valid book to the list
                    }
                }
            }

            return (validBooks, validationErrorList);
        }

        private int? TryParseInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return null;
        }
    }

}

