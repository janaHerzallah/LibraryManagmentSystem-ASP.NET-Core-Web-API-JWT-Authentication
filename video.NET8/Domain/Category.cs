using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{
    public class Category
    {

        [Key]
        public int Id { get; set; }
        
        public string? Name { get; set; }

        public string? Description { get; set; }


        public ICollection<Book> Books { get; set; }


        public Category() { 

        Books = new List<Book>();

        }
    }
}
