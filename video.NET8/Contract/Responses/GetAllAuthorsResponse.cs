using LibraryManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetAllAuthorsResponse
    {
        // get all authors with their books

        public int Id { get; set; }

        
        public string? Name { get; set; }

        public List<GetAuthorsBookResponse> Books { get; set; }


    }
}
