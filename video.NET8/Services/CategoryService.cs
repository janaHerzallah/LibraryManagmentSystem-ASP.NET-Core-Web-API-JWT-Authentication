using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<GetCategoryResponse>> GetAllCategoriesAsync()
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
                                     Books = c.Books.Select(b => new GetAuthorsBookResponse
                                     {
                                         Id = b.Id,
                                         Title = b.Title
                                     }).ToList()
                                 })
                                 .ToListAsync();
        }

        public async Task<GetCategoryResponse> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.Include(c => c.Books)
                                                    .FirstOrDefaultAsync(c => c.Id == id && c.Active);

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
                Books = category.Books.Select(b => new GetAuthorsBookResponse
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList()
            };
        }

        public async Task<AddCategoryResponse> AddCategoryAsync(AddCategoryRequest request)
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

        public async Task<UpdateCategoryResponse> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
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

        public async Task SoftDeleteCategoryAsync(int id)
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


        public async Task AssignBookToCategoryAsync(int categoryId, int bookId)
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


        public async Task<IEnumerable<GetAuthorsBookResponse>> GetBooksInCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.Include(c => c.Books)
                                                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.Active);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found or inactive.");
            }

            return category.Books.Select(b => new GetAuthorsBookResponse
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




    }
}
