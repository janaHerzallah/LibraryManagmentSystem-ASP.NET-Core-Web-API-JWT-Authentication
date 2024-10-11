namespace LibraryManagmentSystem.Contract.Responses
{
    public class ValidationErrorUserResponse
    {
        public int RowNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public bool Active { get; set; }
        public string ErrorMessage { get; set; }

    }
}
