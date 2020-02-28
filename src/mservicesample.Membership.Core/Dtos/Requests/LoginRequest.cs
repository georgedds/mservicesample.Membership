using System.ComponentModel.DataAnnotations;

namespace mservicesample.Membership.Core.Dtos.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Please enter your username")]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
