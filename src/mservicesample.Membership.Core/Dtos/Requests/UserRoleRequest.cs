using System.ComponentModel.DataAnnotations;

namespace mservicesample.Membership.Core.Dtos.Requests
{
    public class UserRoleRequest
    {
        [Required(ErrorMessage = "User id is missing")]
        [DataType(DataType.Text)]
        [Display(Name = "UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Role id is missing")]
        [DataType(DataType.Text)]
        [Display(Name = "RoleId")]
        public string RoleId { get; set; }
    }
}
