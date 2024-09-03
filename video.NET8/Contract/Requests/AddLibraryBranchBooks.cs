namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddLibraryBranchBooks
    {
        public string Title { get; set; }

        public int? AvailableCopies { get; set; }

        public int? TotalCopies { get; set; }

    }
}
