using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{
    public class LibraryBranch : DateBaseClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Location { get; set; }

        public ICollection<Book> Books { get; set; }

    }
}
