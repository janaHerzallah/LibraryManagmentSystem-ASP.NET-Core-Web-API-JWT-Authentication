namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetAllBooksResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }

        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }

        public int? LibraryBranchId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        // Active is not included here since it will be set to true by default in the Book class.
    }
}
