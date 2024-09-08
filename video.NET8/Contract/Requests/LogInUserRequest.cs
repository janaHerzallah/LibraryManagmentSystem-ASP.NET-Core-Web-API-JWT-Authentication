using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class LogInUserRequest
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "password is required.")]
        public string Password { get; set; }
    }
}
