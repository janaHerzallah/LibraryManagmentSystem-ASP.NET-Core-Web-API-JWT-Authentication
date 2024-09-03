using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{
    public class Category: DateBaseClass
    {

        [Key]
        public int Id { get; set; }
        
        public string? Name { get; set; }

        public string? Description { get; set; }


        public ICollection<Book> Books { get; set; }

        public bool Active { get; set; } = true; // Default to true

        public Category() { 

        Books = new List<Book>();

        }
    }
}
