using LibraryManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddMemberRequest 
    {
        
        public string? Name { get; set; } // no problem if its null 

        [EmailAddress]
        public string? Email { get; set; } // ensures it has the @ 

        public int userId { get; set; } // it can be null if there is no user related to it
        


    }
}
