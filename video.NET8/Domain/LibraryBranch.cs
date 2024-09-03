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

        public bool Active { get; set; } = true; // Default to true
        public ICollection<Book> Books { get; set; }


        public LibraryBranch()
        {
            Books = new List<Book>();

        }


    }
}
