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

            RegisterUserResponse Userresponse = new RegisterUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Role = user.Role.ToString(),
                DateCreated = user.DateCreated,
            };
            return Userresponse;
        }

        public async Task<LogInUserResponse> LogInUser(LogInUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null) {
                throw new InvalidOperationException("Invalid username or password");
            }
            // i want to think of a way to tell the user if the problem is the username or the password
            // i think i can do that by checking if the username exists first and then checking if the password is correct
            // if the username does not exist, i can return a message saying the username does not exist
            // if the username exists but the password is wrong, i can return a message saying the password is wrong
            // i can also return a message saying the username and password are wrong if both are wrong

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
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMonths(1), //valid for 1 year
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

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
        public async Task<IEnumerable<User>> GetMembersByAdminOnly()
        {
            return await _context.Users.Where(u => u.Role == UserRole.Member && u.Active== true).ToListAsync();
        }
    }
}
