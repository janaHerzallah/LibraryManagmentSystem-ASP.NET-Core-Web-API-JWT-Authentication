using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
    }
}




