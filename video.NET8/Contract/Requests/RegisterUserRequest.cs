using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
    {
        public class RegisterUserRequest
        {

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(admin|member)$", ErrorMessage = "Role must be either 'admin' or 'member'.")]
        public string Role { get; set; }


        [EmailAddress]
            public string? MemberEmail { get; set; } // ensures it has the @
                                                     // only for members

            public string? MemberName { get; set; } // no problem if its null
                                              // only for members
        }
    }
