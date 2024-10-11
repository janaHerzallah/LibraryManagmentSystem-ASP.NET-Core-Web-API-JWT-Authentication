namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddUserFromExcelRequest
    {
      
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }
    }
}
