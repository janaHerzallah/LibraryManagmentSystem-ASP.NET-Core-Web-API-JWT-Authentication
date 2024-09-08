using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class BorrowBookRequest
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }

        [Required(ErrorMessage = "The claimed return date is required.")]
        public DateTime ClaimedReturnDate { get; set; }

    }
}
