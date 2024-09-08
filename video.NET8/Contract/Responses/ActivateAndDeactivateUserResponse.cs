namespace LibraryManagmentSystem.Contract.Responses
{
    public class ActivateAndDeactivateUserResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool Active { get; set; }
        public string Message { get; set; }
    }
}
