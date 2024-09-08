using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddAuthorsBooksRequest
    {
        [Required(ErrorMessage = "Title is required.")]

        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }


        
    }
}
