namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<GetAuthorsBookResponse> Books { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
