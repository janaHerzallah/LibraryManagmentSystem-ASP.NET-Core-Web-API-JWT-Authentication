namespace LibraryManagmentSystem.Contract.Responses
{
    public class ValidationErrorMemberListResponse
    {
        public int RowNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
    }

}
