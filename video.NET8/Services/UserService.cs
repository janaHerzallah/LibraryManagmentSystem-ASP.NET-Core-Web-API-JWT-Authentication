using LibraryManagmentSystem.Contract.Requests;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using LibraryManagmentSystem.Contract.Responses;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace LibraryManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public UserService(ApplicationDbContext context ,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request)
        {
            // Check if the username already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }
            // if the user enters the names in small letter convert the first letter to upper case 

            if (request.Role == "admin" || request.Role == "member")
            {
                request.Role = request.Role.Substring(0, 1).ToUpper() + request.Role.Substring(1);
                // convert the first letter to upper case
                //and the rest of the letters to lower case
            }


            if (request.Role != "Admin" && request.Role !="Member")
            {
                throw new InvalidOperationException("Invalid role. Make sure to enter either Member or Admin.");
            }

           

            var user = new User
            {
                Username = request.Username,
                Password = request.Password, // Store plain text password
                Role = Enum.Parse<UserRole>(request.Role, true),
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
                
            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (user.Role == UserRole.Member)
            {
                Member member = new Member
                {
                    User = user,
                    Name = request.MemberName,
                    Email = request.MemberEmail,
                    Active = true,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    UserId = user.Id

                };
                _context.Members.Add(member);
                await _context.SaveChangesAsync();
            }
           

            RegisterUserResponse Userresponse = new RegisterUserResponse
            { 
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Role = user.Role.ToString(),
                DateCreated = user.DateCreated,
                MemberName = request.MemberName,
                MemberEmail = request.MemberEmail
            };
            return Userresponse;
        }

        public async Task<LogInUserResponse> LogInUser(LogInUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null) {
                throw new InvalidOperationException("Invalid username or password");
            }
          
            var token = GenerateToken(user);
            user.Token = token;
            user.DateModified = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new LogInUserResponse
            {
                Token = token,
                Role = user.Role.ToString()
            };

        }

        private string GenerateToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // key is used to sign the token 

            // configuring the token description 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // the subject has two main claims, the name(username) and the role   
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMonths(1), //valid for 1 month
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                //symmetric key to sign the token and use the sha256 hash function 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // serialize the token into a string and return it

        }


        public async Task<bool> ValidateAdminsToken(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token && u.Active == true);

            if (user == null || user.Role != UserRole.Admin)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateUsersToken(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token && u.Active == true);

            if (user == null)
            {
                return false;
            }
            // if its a user either its an admin or a member it will return true so its authenticated 
            return true;
        }
        // the authorization is done in the controller since its only allowed by admins 
        public async Task<IEnumerable<GetUserResponse>> GetActiveMembers()
        {
            return await _context.Users.Where(u => u.Role == UserRole.Member && u.Active== true)
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Password = u.Password,
                    Role = u.Role,
                    Token = u.Token,
                    Active = u.Active
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<GetUserResponse>> GetAllMembers()
        {
            return await _context.Users.Where(u => u.Role == UserRole.Member)
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Password = u.Password,
                    Role = u.Role,
                    Token = u.Token,
                    Active = u.Active

                }).ToListAsync();
        }


        public async Task<ActivateAndDeactivateUserResponse> DeactivateUser(ActivateDeActivateUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Active = false;
            user.DateModified = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new ActivateAndDeactivateUserResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Active = user.Active,
                Message = "User has been successfully deactivated"
            };
        }


        public async Task<ActivateAndDeactivateUserResponse> ReActivateUser(ActivateDeActivateUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Active = true;
            user.DateModified = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new ActivateAndDeactivateUserResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Active = user.Active,
                Message = "User has been successfully activated"
            };
        }

        public async Task<(List<AddUserFromExcelRequest> validUsers, List<ValidationErrorUserResponse> validationErrors)> ImportUsersFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validUsers = new List<AddUserFromExcelRequest>();
            var validationErrorList = new List<ValidationErrorUserResponse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet

                    // Validate columns
                    var expectedColumns = new List<string> { "UserName", "Password", "Role" ,"Active"};
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

                        // Validate username
                        var usernameText = worksheet.Cells[row, 1].Text;
                        if (string.IsNullOrWhiteSpace(usernameText))
                        {
                            errorMessage += "Username is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(usernameText, out _))
                        {
                            errorMessage += "Username must be a string, not a number. ";
                            haveError = true;
                        }

                        // Validate password
                        var passwordText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrWhiteSpace(passwordText))
                        {
                            errorMessage += "Password is required. ";
                            haveError = true;
                        }
                        else if (double.TryParse(passwordText, out _))
                        {
                            errorMessage += "Password must be a string, not a number. ";
                            haveError = true;
                        }

                        // Validate role (must be either "member" or "admin")
                        var roleText = worksheet.Cells[row, 3].Text.Trim().ToLower();
                        if (roleText != "member" && roleText != "admin")
                        {
                            errorMessage += "Role must be either 'member' or 'admin'. ";
                            haveError = true;
                        }

                        var activeText = worksheet.Cells[row, 4].Text;
                        bool activeBool= false;

                        if (string.IsNullOrWhiteSpace(activeText))
                        {

                            haveError = true;
                            errorMessage += "Active is required. ";

                        }

                        else if (!bool.TryParse(activeText, out  activeBool))
                        {
                            errorMessage += "Active must be true or false ";
                            haveError = true;
                        }

                        // If there are errors, add to the error list
                        if (haveError)
                        {
                            validationErrorList.Add(new ValidationErrorUserResponse
                            {
                                RowNumber = row,
                                Username = usernameText,
                                Password = passwordText,
                                Role = roleText,
                                Active = activeBool,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create user request object
                        var userRequest = new AddUserFromExcelRequest
                        {
                            Username = usernameText,
                            Password = passwordText,
                            Role = roleText, 
                            Active = activeBool 
                        };

                        validUsers.Add(userRequest); // Add valid user to the list
                    }
                }
            }

            // Create users in the database for valid entries
            foreach (var user in validUsers)
            {
                await AddUserFromExcel(user); // AddUser is a placeholder method, assuming you have user creation logic
            }

            return (validUsers, validationErrorList);
        }



        public async Task<RegisterUserResponse> AddUserFromExcel(AddUserFromExcelRequest request)
        {
            // Check if the username already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }
            // if the user enters the names in small letter convert the first letter to upper case 

            if (request.Role == "admin" || request.Role == "member")
            {
                request.Role = request.Role.Substring(0, 1).ToUpper() + request.Role.Substring(1);
                // convert the first letter to upper case
                //and the rest of the letters to lower case
            }


            if (request.Role != "Admin" && request.Role != "Member")
            {
                throw new InvalidOperationException("Invalid role. Make sure to enter either Member or Admin.");
            }



            var user = new User
            {
                Username = request.Username,
                Password = request.Password, // Store plain text password
                Role = Enum.Parse<UserRole>(request.Role, true),
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Active = request.Active

            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            RegisterUserResponse Userresponse = new RegisterUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Role = user.Role.ToString(),
                DateCreated = user.DateCreated
            };
            return Userresponse;
        }

    }
}
