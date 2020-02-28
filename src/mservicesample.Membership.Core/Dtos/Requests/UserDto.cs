using System.ComponentModel.DataAnnotations;

namespace mservicesample.Membership.Core.Dtos.Requests
{
    public class UserDto
    {
        [Required(ErrorMessage = "Please provide your first name")]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please provide your last name")]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please provide your Email address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Not valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please provide your username")]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        public string Password { get; set; }
        //[Required]
        [DataType(DataType.Text)]
        public string IdentityId { get; set; }
        public string PasswordHash { get; private set; }

        [DataType(DataType.Text)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

    }
}
