namespace LibraryManagmentSystem.Contract.Responses
{
    public class RegisterUserResponse
    {
        /*    "id": 2,
        "username": "ahmad",
        "password": "ahmad123",
        "role": 1,
        "token": null,
        "dateCreated": "2024-08-27T11:55:27.8668294Z",
        "dateModified": "2024-08-27T11:55:27.8668745Z"
        */
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Either "admin" or "member"
  
        public DateTime DateCreated { get; set; }
      


    }
}
