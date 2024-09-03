using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class LibraryBranchService : ILibraryBranchService
    {
        private readonly ApplicationDbContext _context;

        public LibraryBranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetLibraryBranchResponse>> GetAllBranchesAsync()
        {
            return await _context.Branches
                                 .Where(lb => lb.Active)
                                 .Select(lb => new GetLibraryBranchResponse
                                 {
                                     BranchId = lb.Id,
                                     Name = lb.Name,
                                     Location = lb.Location,
                                     Active = lb.Active,
                                     CreatedDate = lb.DateCreated,
                                     UpdatedDate = lb.DateModified,
                                     Books = lb.Books.Select(b => new GetBooksDetailsResponse
                                     {
                                         Id = b.Id,
                                         Title = b.Title
                                     }).ToList()
                                 })
                                 .ToListAsync();
        }


        public async Task<GetLibraryBranchResponse> GetBranchByIdAsync(int id)
        {
            var branch= await _context.Branches.Include(lb => lb.Books).FirstOrDefaultAsync(lb => lb.Id == id && lb.Active);
            return new GetLibraryBranchResponse
            {
                BranchId = branch.Id,
                Name = branch.Name,
                Location = branch.Location,
                Active = branch.Active,
                CreatedDate = branch.DateCreated,
                UpdatedDate = branch.DateModified,
                Books = branch.Books.Select(b => new GetBooksDetailsResponse
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToList()
            };
        }

        public async Task<AddLibraryBranchResponse> AddBranchAsync(AddLibraryBranchRequest branch)
        {

            LibraryBranch BranchDataBase = new LibraryBranch
            {
                Active = true,
                Name = branch.Name,
                Location = branch.Location,
               DateCreated = DateTime.UtcNow,
               DateModified = DateTime.UtcNow

            };

            _context.Branches.Add(BranchDataBase);
            await _context.SaveChangesAsync();

            if (branch.Books == null || branch.Books.Count == 0)
            {
                return new AddLibraryBranchResponse
                {
                    Id = BranchDataBase.Id,
                    Name = BranchDataBase.Name,
                    Location = BranchDataBase.Location,
                    DateCreated = BranchDataBase.DateCreated,
                    DateModified = BranchDataBase.DateModified

                };

            }
            foreach (var book in branch.Books)
            {
                var bookDataBase = new Book
                {
                    Title = book.Title,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    LibraryBranchId = BranchDataBase.Id,
                    Active=true   
                };

                _context.Books.Add(bookDataBase);
                await _context.SaveChangesAsync();

            }

            return new AddLibraryBranchResponse
            {
                Id = BranchDataBase.Id,
                Name = BranchDataBase.Name,
                Location = BranchDataBase.Location,
                DateCreated = BranchDataBase.DateCreated,
                DateModified = BranchDataBase.DateModified,
                Books = branch.Books
            };


        }

        public async Task<UpdateLibraryBranchResponse> UpdateBranchAsync(int id, UpdateLibraryBranchRequest branch)
        {
            var existingBranch = await _context.Branches.FirstOrDefaultAsync(lb => lb.Id == id && lb.Active);
            if (existingBranch == null)
            {
                throw new KeyNotFoundException("Branch not found.");
            }

            existingBranch.Name = branch.Name;
            existingBranch.Location = branch.Location;
            existingBranch.DateModified = DateTime.UtcNow;

            _context.Branches.Update(existingBranch);
            await _context.SaveChangesAsync();
            return new UpdateLibraryBranchResponse
            {
                BranchId=id,
                Name = branch.Name,
                Location = branch.Location,
                Active = existingBranch.Active,
                CreatedDate = existingBranch.DateCreated,
                UpdatedDate = existingBranch.DateModified



            };
            
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(lb => lb.Id == id && lb.Active);
            if (branch == null)
            {
                return false;
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SoftDeleteBranchAsync(int id)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(lb => lb.Id == id && lb.Active);
            if (branch == null)
            {
                throw new KeyNotFoundException("Branch not found or is already inactive.");
            }

            branch.Active = false;
            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetBooksDetailsResponse>> GetBooksInBranchAsync(int branchId)
        {
            var branch = await _context.Branches
                                       .Include(lb => lb.Books)
                                       .FirstOrDefaultAsync(lb => lb.Id == branchId && lb.Active);

            if (branch == null)
            {
                throw new KeyNotFoundException("Branch not found.");
            }

            return branch.Books.Where(b => b.Active)
                               .Select(b => new GetBooksDetailsResponse
                               {
                                   Id = b.Id,
                                   Title = b.Title
                               }).ToList();
        }

        public async Task AssignBookToBranchAsync(int branchId, int bookId)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(lb => lb.Id == branchId && lb.Active);
            if (branch == null)
            {
                throw new KeyNotFoundException("Branch not found.");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.Active);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            // Assign the book to the branch
            book.LibraryBranchId = branchId;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> RemoveBookFromBranchAsync(int bookId, int branchId)
        {
            // Fetch the book by its ID
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.LibraryBranchId == branchId);

            if (book == null)
            {
                return false; // Book not found or does not belong to the specified branch
            }

            // Set the BranchId to null to remove the association
            book.LibraryBranchId = null;

            // Save changes to the database
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
