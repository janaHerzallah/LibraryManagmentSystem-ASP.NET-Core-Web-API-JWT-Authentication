namespace LibraryManagementSystem.Contract.Responses
{
    public class GetBookMarketingInfoResponse
    {
        public string Id { get; set; } // Represents the unique identifier for the book marketing info.
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; } // Author name from author table
        public int TimesBorrowed { get; set; } // Number of times the book has been borrowed
        public int? AvailableCopies { get; set; } // Number of available copies in the library
        public string Category { get; set; } // Category name from category table
        public string LibraryBranch { get; set; } // Library branch name from library branch table
    }
}
