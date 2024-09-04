namespace LibraryManagmentSystem.Contract.Responses
{
    public class ReturnBookResponse
    {

        public int? MemberId { get; set; }

        public int? bookId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime ClaimedReturnDate { get; set; }

        public DateTime DateModified { get; set; }

        public int? availableCopies { get; set; }

        public string? Title { get; set; }


    }
}
