using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateAuthorRequest
    {
        // no need for updating the ID

        [Required(ErrorMessage = "Name is required.")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Only true or false values are allowed.")]
        public bool Active { get; set; }



    }
}
