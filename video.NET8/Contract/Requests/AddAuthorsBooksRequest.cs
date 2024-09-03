namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddAuthorsBooksRequest
    {
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }


        
    }
}
