using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EShopperAngular.Models
{
    public class CreateRoleViewModel
    {
        public string? Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }   
    }
}
