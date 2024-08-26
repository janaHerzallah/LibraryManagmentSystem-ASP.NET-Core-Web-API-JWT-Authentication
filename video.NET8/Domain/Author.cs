using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{
    public class Author: DateBaseClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

      
        public ICollection<Book> Books { get; set; }

        public Author()
        {
            Books = new List<Book>();

        }


    }
}
