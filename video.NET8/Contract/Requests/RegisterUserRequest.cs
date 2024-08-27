    namespace LibraryManagmentSystem.Contract.Requests
    {
        public class RegisterUserRequest
        {

            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; } // Either "admin" or "member"
        }
    }
