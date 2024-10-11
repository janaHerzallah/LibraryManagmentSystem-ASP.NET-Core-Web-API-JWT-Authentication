using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


namespace LibraryManagementSystem.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public MemberService(ApplicationDbContext context, IUserService userService )
        {
            _context = context;
            _userService = userService;
        }

        

        public async Task<IEnumerable<GetMemberResponse>> GetAllMembers() { 
            var member = await _context.Members.ToListAsync();

            return member.Select(m => new GetMemberResponse
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Active = m.Active,
                CreatedAt = m.DateCreated,
                UpdatedAt = m.DateModified,
                OverDueCount = m.OverDueCount,
                userId = m.UserId
            });
        }


        public async Task<IEnumerable<GetMemberResponse>> GetActiveMembers()
        {
           var member= await _context.Members.Where(m => m.Active).ToListAsync();

            return member.Select(m => new GetMemberResponse
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Active = m.Active,
                CreatedAt = m.DateCreated,
                UpdatedAt = m.DateModified,
                OverDueCount = m.OverDueCount,
                userId = m.UserId
            });
        }

        public async Task<GetMemberResponse> GetMemberById(int id)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                throw new KeyNotFoundException("Member not found or is inactive.");
            }
            return new GetMemberResponse
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Active = member.Active,
                CreatedAt = member.DateCreated,
                UpdatedAt = member.DateModified,
                OverDueCount = member.OverDueCount
            };
        }

        public async Task<GetMemberResponse> AddMember(AddMemberRequest member)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == member.userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            Member memberDataBase = new Member
            {
                Name = member.Name,
                Email = member.Email,
                Active = true, // default value
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                UserId = member.userId,
                OverDueCount = member.OverDueCount
            };

            _context.Members.Add(memberDataBase);
            await _context.SaveChangesAsync();

            return new GetMemberResponse
            {
                Id = memberDataBase.Id,
                Name = memberDataBase.Name,
                Email = memberDataBase.Email,
                Active = memberDataBase.Active,
                CreatedAt = memberDataBase.DateCreated,
                UpdatedAt = memberDataBase.DateModified,
                OverDueCount = memberDataBase.OverDueCount,
                userId = member.userId
            };
        }

        public async Task<GetMemberResponse> UpdateMember(int id, UpdateMemeberRequest updatedMember)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                throw new KeyNotFoundException("Member not found.");
            }

            member.Name = updatedMember.Name;
            member.Email = updatedMember.Email;
            member.Active = updatedMember.Active;
            member.DateModified = DateTime.UtcNow;
            member.Active = updatedMember.Active;
            member.OverDueCount = updatedMember.OverDueCount;
            member.UserId = updatedMember.userId;


            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            return new GetMemberResponse
            {
                Id = member.Id,
                Name = updatedMember.Name,
                Email = updatedMember.Email,
                Active = updatedMember.Active,
                CreatedAt = member.DateCreated,
                UpdatedAt = member.DateModified,
                OverDueCount = member.OverDueCount,
                userId= updatedMember.userId
                

            };
        }

        public async Task<bool> DeleteMember(int id)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.Active);
            if (member == null)
            {
                return false;
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SoftDeleteMember(int id)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.Active);
            if (member == null)
            {
                throw new KeyNotFoundException("Member not found or is already inactive.");
            }

            member.Active = false;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetMembersAllBorrowedBooks(int memberId , string token)
        {
            // get currently borrowed books --> active books and inactive books

            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
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
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            if (memberId == 0)
            {
                throw new Exception("Please Enter Valied Member Id");
            }

            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId)
                                 .Include(b => b.Book)
                                 .Select(b => new GetBorrowedBooksForAMemberResponse
                                 {
                                     MemberId = b.MemberId,
                                     bookId = b.BookId,
                                     BorrowDate = b.BorrowDate,
                                     ReturnDate = b.ActualReturnDate == DateTime.MinValue ? "Not Returned" : b.ActualReturnDate.ToString("yyyy-MM-dd"),

                                     DateCreated = b.DateCreated,
                                     DateModified = b.DateModified,
                                     availableCopies = b.Book.AvailableCopies,
                                     Title = b.Book.Title,
                                     ClaimedReturnDate = b.ClaimedReturnDate
                                 })
                                 .ToListAsync();
        }

        // set active to false means its not borrowed any more so its returned
        // active = true = borrowed
        //active = false = returned 
        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetNotReturnedBooks(int memberId , string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == memberId && 
                    m.Active);

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            // not returned = > active = true 

            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active && ( b.ActualReturnDate == DateTime.MinValue))
                                 .Include(b => b.Book)
                                 .Select(b => new GetBorrowedBooksForAMemberResponse
                                 {
                                     MemberId = b.MemberId,
                                     bookId = b.BookId,
                                     BorrowDate = b.BorrowDate,
                                     ReturnDate = b.ActualReturnDate == DateTime.MinValue ? "Not Returned" : b.ActualReturnDate.ToString("yyyy-MM-dd"),

                                     DateCreated = b.DateCreated,
                                     DateModified = b.DateModified,
                                     availableCopies = b.Book.AvailableCopies,
                                     Title = b.Book.Title,
                                     ClaimedReturnDate = b.ClaimedReturnDate
                                 })
                                 .ToListAsync();
        }

        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetOverDueBorrowedBooks(int memberId , string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
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
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            // only active books 
            // because if the book is false then the book should be already returned so its impossible to be overdued 
            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active && 
                                 b.ClaimedReturnDate< DateTime.UtcNow &&   // if the claimed date has passed 
                                 ( b.ActualReturnDate == DateTime.MinValue)) // if its infinity it means its not returned yet 
                                 .Include(b => b.Book)
                                 .Select(b => new GetBorrowedBooksForAMemberResponse
                                 {
                                     MemberId = b.MemberId,
                                     bookId = b.BookId,
                                     BorrowDate = b.BorrowDate,
                                     ReturnDate = b.ActualReturnDate == DateTime.MinValue ? "Not Returned" : b.ActualReturnDate.ToString("yyyy-MM-dd"),

                                     DateCreated = b.DateCreated,
                                     DateModified = b.DateModified,
                                     availableCopies = b.Book.AvailableCopies,
                                     Title = b.Book.Title,
                                     ClaimedReturnDate = b.ClaimedReturnDate
                                 })
                                 .ToListAsync();
        }
        public async Task<int> GetOverdueBooksCount(int memberId , string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
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
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active && (b.ActualReturnDate == DateTime.MinValue) && b.ClaimedReturnDate < DateTime.UtcNow )
                                 .CountAsync();
        }



        public async Task<(List<AddMemberRequest> validMembers, List<ValidationErrorMemberListResponse> validationErrors)> ImportMembersFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validMembers = new List<AddMemberRequest>();
            var validationErrorList = new List<ValidationErrorMemberListResponse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet

                    // Validate columns
                    var expectedColumns = new List<string> { "Name", "Email", "UserId", "OverDueCount" };
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

                        // Validate Member Name
                        var nameText = worksheet.Cells[row, 1].Text;
                        if (string.IsNullOrWhiteSpace(nameText))
                        {
                            errorMessage += "Member name is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(nameText, out _)) // Check if name is not a number
                        {
                            errorMessage += "Member name must be a string. ";
                            haveError = true;
                        }

                        // Validate Email format
                        var emailText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrWhiteSpace(emailText))
                        {
                            errorMessage += "Email is required. ";
                            haveError = true;
                        }
                        else if (!IsValidEmail(emailText)) // Email format validation
                        {
                            errorMessage += "Email format is invalid. ";
                            haveError = true;
                        }

                        // Validate UserId
                        var userIdText = worksheet.Cells[row, 3].Text;
                        int userId;
                        if (!int.TryParse(userIdText, out userId))
                        {
                            errorMessage += "UserId must be a valid integer. ";
                            haveError = true;
                        }
                        else
                        {
                            // Check if the UserId exists in the Users table and is not already associated with another member
                            var userExists = await UserExists(userId);
                            var userAssignedToAnotherMember = await IsUserAssignedToAnotherMember(userId);

                            if (!userExists)
                            {
                                errorMessage += "UserId does not exist in the Users table. ";
                                haveError = true;
                            }
                            else if (userAssignedToAnotherMember)
                            {
                                errorMessage += "UserId is already assigned to another member. ";
                                haveError = true;
                            }
                        }

                        // Validate OverDueCount is a number
                        var overDueCountText = worksheet.Cells[row, 4].Text;
                        int overDueCount;
                        if (!int.TryParse(overDueCountText, out overDueCount))
                        {
                            errorMessage += "OverDueCount must be a valid integer. ";
                            haveError = true;
                        }

                        // If there are errors, add to the error list
                        if (haveError)
                        {
                            validationErrorList.Add(new ValidationErrorMemberListResponse
                            {
                                RowNumber = row,
                                Name = nameText,
                                Email = emailText,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create member request object
                        var memberRequest = new AddMemberRequest
                        {
                            Name = nameText,
                            Email = emailText,
                            userId = userId,
                            OverDueCount = overDueCount
                            
                        };

                        validMembers.Add(memberRequest); // Add valid member to the list
                    }
                }
            }


            // Create members in the database for valid entries
            foreach (var member in validMembers)
            {

                await AddMember(member);
            }

            return (validMembers, validationErrorList);
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        /*
         
         The MailAddress constructor attempts to parse the email.
        If the email is valid, the addr.Address will match the input email.
        If parsing fails, it throws an exception, caught in the catch block, and the method returns false.
       
         */


        private async Task<bool> UserExists(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        private async Task<bool> IsUserAssignedToAnotherMember(int userId)
        {
            return await _context.Members.AnyAsync(m => m.UserId == userId);
        }

    }
}
