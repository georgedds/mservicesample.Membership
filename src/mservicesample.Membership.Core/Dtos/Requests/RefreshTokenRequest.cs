using System.ComponentModel.DataAnnotations;

namespace mservicesample.Membership.Core.Dtos.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "AccessToken")]
        public string AccessToken { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "RefreshToken")]
        public string RefreshToken { get; set; }
    }
}
