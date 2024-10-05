using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

       public async Task<IEnumerable<GetCategoryResponse>> GetAllCategories()
       {
            return await _context.Categories
                                 .Select(c => new GetCategoryResponse
                                 {
                                     Id = c.Id,
                                     Name = c.Name,
                                     Description = c.Description,
                                     CreatedAt = c.DateCreated,
                                     UpdatedAt = c.DateModified,
                                     Books = c.Books.Select(b => new GetBooksDetailsResponse
                                     {
                                         Id = b.Id,
                                         Title = b.Title
                                     }).ToList()
                                 })
                                 .ToListAsync();
        }
        public async Task<IEnumerable<GetCategoryResponse>> GetActiveCategories()
        {

            return await _context.Categories
                                 .Where(c => c.Active)
                                 .Select(c => new GetCategoryResponse
                                 {
                                     Id = c.Id,
                                     Name = c.Name,
                                     Description = c.Description,
                                     CreatedAt = c.DateCreated,
                                     UpdatedAt = c.DateModified,
                                     Books = c.Books.Select(b => new GetBooksDetailsResponse
                                     {
                                         Id = b.Id,
                                         Title = b.Title
                                     }).ToList()
                                 })
                                 .ToListAsync();
        }

        public async Task<GetCategoryResponse> GetCategoryById(int id)
        {
            var category = await _context.Categories.Include(c => c.Books)
                                                    .FirstOrDefaultAsync(c => c.Id == id );

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            return new GetCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.DateCreated,
                UpdatedAt = category.DateModified,
                Books = category.Books.Select(b => new GetBooksDetailsResponse
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList()
            };
        }

        public async Task<AddCategoryResponse> AddCategory(AddCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Active = true,
                DateCreated=DateTime.UtcNow,
                DateModified=DateTime.UtcNow

            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new AddCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.DateCreated,
                UpdatedAt = category.DateModified
            };
        }

        public async Task<UpdateCategoryResponse> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id); // no need to check if the user is active or not
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.DateModified = DateTime.UtcNow;
            category.Active = request.Active;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new UpdateCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.DateCreated,
                UpdatedAt = category.DateModified

            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.Active);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SoftDeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.Active);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found or is already inactive.");
            }

            category.Active = false;
            category.DateModified = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }


        public async Task AssignBookToCategory(int categoryId, int bookId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.Active);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found or inactive.");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            book.CategoryId = categoryId;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<GetBooksDetailsResponse>> GetBooksInCategory(int categoryId)
        {
            var category = await _context.Categories.Include(c => c.Books)
                                                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.Active);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found or inactive.");
            }

            return category.Books.Select(b => new GetBooksDetailsResponse
            {
                Id = b.Id,
                Title = b.Title
            }).ToList();
        }


        public async Task RemoveBookFromCategoryAsync(int categoryId, int bookId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.Active);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found or inactive.");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.CategoryId == categoryId);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found in this category.");
            }

            book.CategoryId = null; // Remove the book from the category by setting the CategoryId to null
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }


       // allowing the authorId to be null, meaning there is no author ID specified.If this value remains null, you  choose to ignore it when building a query, 

        public async Task<IEnumerable<GetBooksDetailsResponse>> FilterBooks(int? authorId = null, bool? available = null)
        {
            //is useful when in need to build queries dynamically based on conditions at runtime. 
            var query = _context.Books.AsQueryable(); // allows to add conditions to the query without executing it immediatly

            if (authorId.HasValue) // if the author id is provided 
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            if (available.HasValue)
            {
                query = query.Where(b => b.AvailableCopies > 0 == available.Value);
            }

            return await query.Select(b => new GetBooksDetailsResponse
            {
                Id = b.Id,
                Title = b.Title
            }).ToListAsync();
        }


        public async Task<IEnumerable<GetBooksDetailsResponse>> SearchBooks(string? title = null, string? authorName = null)
        {
            var query = _context.Books.Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(b => b.Author.Name.Contains(authorName));
            }

            return await query.Select(b => new GetBooksDetailsResponse
            {
                Id = b.Id,
                Title = b.Title
             
            }).ToListAsync();
        }


        public async Task<IEnumerable<ExcelExportCategoryResponse>> ExportCategoriesToExcel()
        {
            return await _context.Categories
                                 .Select(c => new ExcelExportCategoryResponse
                                 {
                                     Id = c.Id,
                                     Name = c.Name,
                                     Description = c.Description,
                                     CreatedAt = c.DateCreated,
                                     UpdatedAt = c.DateModified,
                                   
                                 })
                                 .ToListAsync();
        }
        public async Task<(List<AddCategoryRequest> validCategories, List<validationErrorCategoryListResponse> validationErrors)> ImportCategoriesFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validCategories = new List<AddCategoryRequest>();
            var validationErrorList = new List<validationErrorCategoryListResponse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet



                    // Validate columns
                    var expectedColumns = new List<string> { "Name", "Description" };
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

                        // Validate Category Name
                        var nameText = worksheet.Cells[row, 1].Text;
                        if (string.IsNullOrWhiteSpace(nameText))
                        {
                            errorMessage += "Category name is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(nameText, out _))
                        {
                            errorMessage += "Category name must be a string, not a number. ";
                            haveError = true;
                        }

                        // Validate Description (optional)
                        var descriptionText = worksheet.Cells[row, 2].Text;
                        if (!string.IsNullOrWhiteSpace(descriptionText))
                        {
                            if (double.TryParse(descriptionText, out _))
                            {
                                errorMessage += "Description must be a string, not a number. ";
                                haveError = true;
                            }
                        }

                        // If there are errors, add to the error list
                        if (haveError)
                        {
                            validationErrorList.Add(new validationErrorCategoryListResponse
                            {
                                RowNumber = row,
                                Name = nameText,
                                Description = descriptionText,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create category request object
                        var categoryRequest = new AddCategoryRequest
                        {
                            Name = nameText,
                            Description = string.IsNullOrWhiteSpace(descriptionText) ? null : descriptionText
                        };

                        validCategories.Add(categoryRequest); // Add valid category to the list
                    }
                }
            }

            return (validCategories, validationErrorList);
        }


    }
}
