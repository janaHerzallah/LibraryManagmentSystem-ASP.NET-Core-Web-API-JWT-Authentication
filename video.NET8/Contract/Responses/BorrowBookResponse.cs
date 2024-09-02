namespace LibraryManagmentSystem.Contract.Responses
{
    public class BorrowBookResponse
    {
       public int? MemberId { get; set; }

       public int? bookId { get; set; }
               
       public DateTime BorrowDate { get; set; }
                         
       public DateTime DateCreated { get; set;}

       public DateTime DateModified { get; set; }

       public int? availableCopies { get; set; }

       public string? Title { get; set; }

    }
}
