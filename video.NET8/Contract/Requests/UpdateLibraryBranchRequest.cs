using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateLibraryBranchRequest
    {

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Only true or false values are allowed.")]
        public bool Active { get; set; } 
    }
}
