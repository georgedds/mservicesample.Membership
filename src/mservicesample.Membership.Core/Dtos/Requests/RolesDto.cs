using System.ComponentModel.DataAnnotations;

namespace mservicesample.Membership.Core.Dtos.Requests
{
    public class RolesDto
    {
        [Required(ErrorMessage = "Please enter role name")]
        [DataType(DataType.Text)]
        [Display(Name = "Role name")]
        public string Name { get; set; }
        //public string Description { get; set; }
        public string Id { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
