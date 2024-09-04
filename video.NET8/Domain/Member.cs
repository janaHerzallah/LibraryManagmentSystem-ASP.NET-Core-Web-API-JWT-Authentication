using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Domain
{
    public class Member: DateBaseClass
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; } // no problem if its null 

        public string? Email { get; set; }

        public bool Active { get; set; } = true; // Default to true

        public int OverDueCount { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; } // it can be null if there is no user related to it

        public User User { get; set; } // Navigation property for the User

    }
}
