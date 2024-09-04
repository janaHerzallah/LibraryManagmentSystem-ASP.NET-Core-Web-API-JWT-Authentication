namespace LibraryManagmentSystem.Contract.Requests
{
    public class BorrowBookRequest
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public DateTime ClaimedReturnDate { get; set; }

    }
}
