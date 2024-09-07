using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateBookRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }

        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }

        public int? LibraryBranchId { get; set; }

        [Required(ErrorMessage = "Only true or false values are allowed.")]
        public bool active { get; set; }
    }
}
