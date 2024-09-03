namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetAuthorByIdResponse
    {

        public int Id { get; set; }


        public string? Name { get; set; }

        public List<GetBooksDetailsResponse> Books { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
