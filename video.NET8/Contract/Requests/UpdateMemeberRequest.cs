using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateMemeberRequest
    {

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; } 

        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Only true or false values are allowed.")]
        public bool Active { get; set; } 

        public int OverDueCount { get; set; }

        public int userId { get; set; } 
    }
}
