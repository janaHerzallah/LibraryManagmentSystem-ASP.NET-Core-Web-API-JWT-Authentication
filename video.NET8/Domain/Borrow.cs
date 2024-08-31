using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{
    public class Borrow
    {
        [Key]
        public int Id { get; set; }

        public int? BookId;

        public int? MemberId;

        public DateTime? BorrowDate;

        public DateTime? ReturnDate;

        public bool Active { get; set; } = true; // Default to true

    }
}
