using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.Domain
{
    public class Book:  DateBaseClass
    {
        [Key]
        public int Id { get ; set; }

        [Required]
        public string? Title { get; set; }
        

        public int? AvailableCopies { get; set; }// it can be null if there is no available copies

        public int? TotalCopies { get; set; } // it can be null if there is no total copies

        public int? AuthorId { get; set; } // it can be null if there is no author related to it 

        public int? CategoryId { get; set; } // it can be null if there is no category related to it

        public int? LibraryBranchId { get; set; } // it can be null if there is no library branch related to it
        public Author Author { get; set; }

        public Category Category { get; set; }

        public LibraryBranch LibraryBranch { get; set; }


    }
}
