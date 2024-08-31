using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;

namespace LibraryManagmentSystem.Contract.Responses
{
    public class AddAuthorResponse: DateBaseClass
    {

        public int Id { get; set; }


        public string? Name { get; set; }

        public bool Active { get; set; } 


        public List<AddAuthorsBooksRequest> Books { get; set; }


        public AddAuthorResponse()
        {
            Books = new List<AddAuthorsBooksRequest>();
        }
    }
}

