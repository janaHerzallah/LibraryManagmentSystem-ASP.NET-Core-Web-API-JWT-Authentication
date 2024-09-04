namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateBookRequest
    {
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }

        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }

        public int? LibraryBranchId { get; set; }

        public bool active { get; set; }
    }
}
