using LibraryManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddMemberRequest 
    {
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; } // no problem if its null 

        [EmailAddress]
        public string? Email { get; set; } // ensures it has the @ 

        public int userId { get; set; } // it can be null if there is no user related to it
        
        public int OverDueCount { get; set; }

    }
}
