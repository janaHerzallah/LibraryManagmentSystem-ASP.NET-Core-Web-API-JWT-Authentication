using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetBooksDetailsResponse
    {
        public int Id { get; set; }


        public string Title { get; set; }
    }
}
