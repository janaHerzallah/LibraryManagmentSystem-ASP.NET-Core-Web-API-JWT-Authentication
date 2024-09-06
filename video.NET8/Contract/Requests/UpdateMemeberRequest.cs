using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateMemeberRequest
    {
       
        public string? Name { get; set; } // no problem if its null 

        [EmailAddress]
        public string? Email { get; set; }

        public bool Active { get; set; } = true; // Default to true

        public int OverDueCount { get; set; }

        public int userId { get; set; } 
    }
}
