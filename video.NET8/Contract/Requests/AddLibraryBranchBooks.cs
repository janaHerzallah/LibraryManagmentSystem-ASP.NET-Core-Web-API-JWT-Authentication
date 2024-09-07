using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddLibraryBranchBooks
    {
        [Required(ErrorMessage = "Title of the book is required.")]
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }

    }
}
