namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetBookByIdResponse
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public int? AvailableCopies { get; set; }
        public int? TotalCopies { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; } // Add AuthorName property
        public int? CategoryId { get; set; }
        public int? LibraryBranchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
