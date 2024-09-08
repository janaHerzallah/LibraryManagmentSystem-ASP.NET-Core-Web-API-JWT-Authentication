using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddCategoryRequest
    {

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }

      
    }
}
