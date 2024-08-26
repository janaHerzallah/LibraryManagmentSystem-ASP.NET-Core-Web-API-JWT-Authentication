using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{

    public enum UserRole
    {
        Admin,
        Member
    }


    public class User: DateBaseClass
    {


        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed passwords, not plain text

        [Required]
        public UserRole Role { get; set; } // Either "admin" or "member"

        public string? Token { get; set; } // JWT token

        
    }
}
