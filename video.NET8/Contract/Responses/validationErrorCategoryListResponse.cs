namespace LibraryManagmentSystem.Contract.Responses
{
    public class validationErrorCategoryListResponse
    {
       
            public string Name { get; set; }
            public string Description { get; set; } // I Added to show the description that caused the error
            public string ErrorMessage { get; set; }
            public int RowNumber { get; set; }
    }
    
}
