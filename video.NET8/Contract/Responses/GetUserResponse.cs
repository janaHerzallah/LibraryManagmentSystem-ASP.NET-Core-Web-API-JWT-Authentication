using LibraryManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetUserResponse
    {
        public int Id { get; set; }

        
        public string Username { get; set; }

        
        public string Password { get; set; } // Store hashed passwords, not plain text in the future

        
        public UserRole Role { get; set; } // Either "admin" or "member"

        public string? Token { get; set; } // JWT token

        public bool Active { get; set; } = true; // Default to true


    }
}
