using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        //member and admin
        public async Task<IEnumerable<GetLibraryBranchResponse>> GetAllBranches()
        {
            return await _context.Branches
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

        //member and admin
        public async Task<IEnumerable<GetLibraryBranchResponse>> GetActiveBranches()
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

        //member and admin
        public async Task<GetLibraryBranchResponse> GetBranchById(int id)
        {
            var branch = await _context.Branches.Include(lb => lb.Books).FirstOrDefaultAsync(lb => lb.Id == id);
            if (branch == null)
            {
                throw new KeyNotFoundException("Branch not found.");
            }
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

        public async Task<AddLibraryBranchResponse> AddBranch(AddLibraryBranchRequest branch)
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
                if (book.TotalCopies < book.AvailableCopies)
                {
                    throw new ArgumentException("Total copies have to be equal or more than Available ones");
                }

                var bookDataBase = new Book
                {
                    Title = book.Title,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    LibraryBranchId = BranchDataBase.Id,
                    Active = true,
                    AvailableCopies = book.AvailableCopies ?? 0,
                    TotalCopies = book.TotalCopies ?? 0
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

        public async Task<UpdateLibraryBranchResponse> UpdateBranch(int id, UpdateLibraryBranchRequest branch)
        {
            var existingBranch = await _context.Branches.FirstOrDefaultAsync(lb => lb.Id == id);
            if (existingBranch == null)
            {
                throw new KeyNotFoundException("Branch not found.");
            }

            existingBranch.Name = branch.Name;
            existingBranch.Location = branch.Location;
            existingBranch.DateModified = DateTime.UtcNow;
            existingBranch.Active = branch.Active;

            _context.Branches.Update(existingBranch);
            await _context.SaveChangesAsync();
            return new UpdateLibraryBranchResponse
            {
                BranchId = id,
                Name = branch.Name,
                Location = branch.Location,
                Active = existingBranch.Active,
                CreatedDate = existingBranch.DateCreated,
                UpdatedDate = existingBranch.DateModified



            };

        }

        public async Task<bool> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(lb => lb.Id == id && lb.Active);
            if (branch == null)
            {
                throw new KeyNotFoundException("Branch not found.");

            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SoftDeleteBranch(int id)
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

        // member and admin
        public async Task<IEnumerable<GetBooksDetailsResponse>> GetBooksInBranch(int branchId)
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

        public async Task AssignBookToBranch(int branchId, int bookId)
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


        public async Task<bool> RemoveBookFromBranch(int bookId, int branchId)
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

        public async Task<(List<AddLibraryBranchRequest> validBranches, List<ValidationErrorBranchResponse> validationErrors)> ImportBranchesFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validBranches = new List<AddLibraryBranchRequest>();
            var validationErrorList = new List<ValidationErrorBranchResponse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet

                    // Validate columns
                    var expectedColumns = new List<string> { "Name", "Location", "Books" };
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

                        // Validate Branch Name
                        var nameText = worksheet.Cells[row, 1].Text;
                        if (string.IsNullOrWhiteSpace(nameText))
                        {
                            errorMessage += "Branch name is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(nameText, out _))
                        {
                            errorMessage += "Branch name must be a string, not a number. ";
                            haveError = true;
                        }

                        // Validate Location
                        var locationText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrWhiteSpace(locationText))
                        {
                            errorMessage += "Location is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(locationText, out _))
                        {
                            errorMessage += "Location must be a string, not a number. ";
                            haveError = true;
                        }

                        // Validate Books (optional)
                        var booksText = worksheet.Cells[row, 3].Text;
                        var books = new List<AddLibraryBranchBooks>();

                        if (!string.IsNullOrWhiteSpace(booksText))
                        {
                            // Assuming booksText is a comma-separated list of book titles
                            var bookTitles = booksText.Split(',');

                            foreach (var bookTitle in bookTitles)
                            {
                                if (!string.IsNullOrWhiteSpace(bookTitle))
                                {
                                    books.Add(new AddLibraryBranchBooks { Title = bookTitle.Trim() });
                                }
                            }
                        }

                        // If there are errors, add to the error list
                        if (haveError)
                        {
                            validationErrorList.Add(new ValidationErrorBranchResponse
                            {
                                RowNumber = row,
                                Name = nameText,
                                Location = locationText,
                                Books = booksText,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create branch request object
                        var branchRequest = new AddLibraryBranchRequest
                        {
                            Name = nameText,
                            Location = locationText,
                            Books = books
                        };

                        validBranches.Add(branchRequest); // Add valid branch to the list
                    }
                }
            }

            return (validBranches, validationErrorList);
        }
    
        

    }
}
