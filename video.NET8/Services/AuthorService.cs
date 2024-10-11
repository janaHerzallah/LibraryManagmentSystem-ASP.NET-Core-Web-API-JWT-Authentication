using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


namespace LibraryManagementSystem.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;


        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }
        

        public async Task<IEnumerable<GetAllAuthorsResponse>> GetActiveAuthors()
        {
            var list = await _context.Authors
                                     .Where(a => a.Active == true)
                                     .Include(a => a.Books) // Include the Books property
                                     .Select(a => new GetAllAuthorsResponse
                                     {
                                         Id = a.Id,
                                         Name = a.Name,
                                         Books = a.Books.Select(b => new GetBooksDetailsResponse
                                         {
                                             Id = b.Id,
                                             Title = b.Title
                                         }).ToList()
                                     }).ToListAsync();
            return list;


        }

        public async Task<IEnumerable<GetAllAuthorsResponse>> GetAllAuthors()
        {
            var list = await _context.Authors
                                     .Include(a => a.Books) // Include the Books property
                                     .Select(a => new GetAllAuthorsResponse
                                     {
                                         Id = a.Id,
                                         Name = a.Name,
                                         Books = a.Books.Select(b => new GetBooksDetailsResponse
                                         {
                                             Id = b.Id,
                                             Title = b.Title
                                         }).ToList()
                                     }).ToListAsync();
            return list;


        }

        public async Task<GetAuthorByIdResponse> GetAuthorById(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books) // Include related Books
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                throw new KeyNotFoundException("Author not found or is inactive.");
            }

            return new GetAuthorByIdResponse
            {
                Id = author.Id,
                Name = author.Name,
                Books = author.Books.Select(b => new GetBooksDetailsResponse
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList(),
                CreatedAt = author.DateCreated,
                UpdatedAt = author.DateModified
            };
        }

        public async Task<AddAuthorResponse> AddAuthor(AddAuthorRequest author)
        {


            Author authorDataBase = new Author
            {
                Name = author.Name,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Active = true

            };

            _context.Authors.Add(authorDataBase);
            await _context.SaveChangesAsync();

            if (author.Books == null || author.Books.Count == 0)
            {
                return new AddAuthorResponse
                {
                    Id = authorDataBase.Id,
                    Name = authorDataBase.Name,
                    DateCreated = authorDataBase.DateCreated,
                    DateModified = authorDataBase.DateModified,
                    Active = authorDataBase.Active
                };
            }
            foreach (var book in author.Books)
            {
                if (book.TotalCopies < book.AvailableCopies)
                {
                    throw new ArgumentException("Total copies have to be equal or more than Available ones");
                }

                Book BookDataBase = new Book
                {

                    Title = book.Title,
                    AuthorId = authorDataBase.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Active = true,

                    TotalCopies = book.TotalCopies ?? 0,
                    AvailableCopies = book.AvailableCopies ?? 0
                };

                _context.Books.Add(BookDataBase);
                await _context.SaveChangesAsync();
            }
            return new AddAuthorResponse
            {
                Id = authorDataBase.Id,
                Name = authorDataBase.Name,
                DateCreated = authorDataBase.DateCreated,
                DateModified = authorDataBase.DateModified,
                Books = author.Books,
                Active = authorDataBase.Active

            };
        }

        public async Task<UpdateAuthorResponse> UpdateAuthor(int id, UpdateAuthorRequest updatedAuthor)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id );


            if (author == null)
            {
                throw new KeyNotFoundException("Author not found or is inactive.");
            }

            author.Name = updatedAuthor.Name;
            author.Active = updatedAuthor.Active;
            author.DateModified = DateTime.UtcNow;


            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return new UpdateAuthorResponse
            {

                Id = author.Id,
                Name = author.Name,
                createdAt = author.DateCreated,
                updatedAt = author.DateModified

            }; 
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id && a.Active);
        if (author == null)
        {
            return false;
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SoftDeleteAuthor(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id && a.Active);
        if (author == null)
        {
            throw new KeyNotFoundException("Author not found or is already inactive.");
        }

        author.Active = false;
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
    }


        public async Task<IEnumerable<ExcelExportAuthorResponse>> ExportAllAuthorsToExcel()
        {
            var list = await _context.Authors
                                     .Select(a => new ExcelExportAuthorResponse
                                     {
                                         Id = a.Id,
                                         Name = a.Name,
                                         
                                     }).ToListAsync();
            return list;


        }


        public async Task<IEnumerable<ExcelExportAuthorResponse>> ExportActiveAuthorsToExcel()
        {
            var list = await _context.Authors
                                     .Where(a => a.Active == true)
                                     .Select(a => new ExcelExportAuthorResponse
                                     {
                                         Id = a.Id,
                                         Name = a.Name,

                                     }).ToListAsync();
            return list;


        }

        public async Task<(List<AddAuthorRequest> validAuthors, List<validationErrorAuthorListResponse> validationErrors)> ImportAuthorsFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validAuthors = new List<AddAuthorRequest>();
            var validationErrorList = new List<validationErrorAuthorListResponse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet

                    // Validate columns
                    var expectedColumns = new List<string> { "Name", "Books" };
                    var columnNames = new List<string>();

                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        var columnName = worksheet.Cells[1, col].Text.Trim();
                        columnNames.Add(columnName);
                    }

                    // Check if all expected columns are present and no extra columns exist
                    if (!expectedColumns.SequenceEqual(columnNames))
                    {
                        throw new ArgumentException($"Column validation failed. Expected columns: {string.Join(", ", expectedColumns)}. Found: {string.Join(", ", columnNames)}");
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var errorMessage = string.Empty;
                        var haveError = false;

                        // Validate Author Name
                        var nameText = worksheet.Cells[row, 1].Text;
                        if (string.IsNullOrWhiteSpace(nameText))
                        {
                            errorMessage += "Author name is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(nameText, out _))
                        {
                            errorMessage += "Author name must be a string, not a number. ";
                            haveError = true;
                        }

                        // Optional: Validate if there are books (can be skipped if adding books separately)
                        var booksText = worksheet.Cells[row, 2].Text;
                        var books = new List<AddAuthorsBooksRequest>();

                        // if there is a list of books, split them by comma

                        if (!string.IsNullOrWhiteSpace(booksText))
                        {
                            // Assuming booksText is a comma-separated list of book titles
                            var bookTitles = booksText.Split(',');

                            foreach (var bookTitle in bookTitles)
                            {
                                if (!string.IsNullOrWhiteSpace(bookTitle))
                                {
                                    books.Add(new AddAuthorsBooksRequest { Title = bookTitle.Trim() });
                                }
                            }
                        }

                        // If there are errors, add to the error list
                        if (haveError)
                        {
                            validationErrorList.Add(new validationErrorAuthorListResponse
                            {
                                RowNumber = row,
                                Name = nameText,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create author request object
                        var authorRequest = new AddAuthorRequest
                        {
                            Name = nameText,
                            Books = books
                        };

                        validAuthors.Add(authorRequest); // Add valid author to the list
                    }
                }
            }


            // Create authors in the database for valid entries
            foreach (var author in validAuthors)
            {
                await AddAuthor(author);
            }

            return (validAuthors, validationErrorList);
        }

     

    }
}




