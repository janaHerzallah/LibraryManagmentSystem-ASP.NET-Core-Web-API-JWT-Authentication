using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain
{
    public class Member: DateBaseClass
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; } // no problem if its null 

        public string? Email { get; set; }

        public bool Active { get; set; } = true; // Default to true


    }
}
