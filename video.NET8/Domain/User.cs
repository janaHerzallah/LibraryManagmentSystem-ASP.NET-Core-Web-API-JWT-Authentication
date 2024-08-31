using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{

    public enum UserRole
    {
        Admin, //0
        Member //1
    }


    public class User: DateBaseClass
    {


        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // Store hashed passwords, not plain text in the future

        [Required]
        public UserRole Role { get; set; } // Either "admin" or "member"

        public string? Token { get; set; } // JWT token

        public bool Active { get; set; } = true; // Default to true

    }
}
