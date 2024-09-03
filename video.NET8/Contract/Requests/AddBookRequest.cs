using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Contract.Requests
{
    public class AddBookRequest
    {
        [Required]
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }

        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }

        public int? LibraryBranchId { get; set; }

        // Active is not included here since it will be set to true by default in the Book class.
    }
}
