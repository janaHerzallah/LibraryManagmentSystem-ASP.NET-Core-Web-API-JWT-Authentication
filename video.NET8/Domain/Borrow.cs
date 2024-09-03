using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Domain
{
    public class Borrow: DateBaseClass
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int? BookId;

        [ForeignKey("Member")]

        public int? MemberId;

        public DateTime BorrowDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public Member Member { get; set; }  // Navigation property for the Member

        public Book Book { get; set; }  // Navigation property for the Member

        public bool Active { get; set; } = true; // Default to true

    }
}
