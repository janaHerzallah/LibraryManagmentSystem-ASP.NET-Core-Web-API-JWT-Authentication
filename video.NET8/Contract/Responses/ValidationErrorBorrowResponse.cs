namespace LibraryManagmentSystem.Contract.Responses
{
    
        public class ValidationErrorBorrowResponse
        {
            public int RowNumber { get; set; }
            public string BookId { get; set; }
            public string MemberId { get; set; }
            public string BorrowDate { get; set; }
            public string ClaimedReturnDate { get; set; }
            public string ActualReturnDate { get; set; }
            public string ErrorMessage { get; set; }

            public string Active { get; set; }
        }

    
}
