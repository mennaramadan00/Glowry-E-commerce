using System.ComponentModel.DataAnnotations;

namespace Glowry.View_Model
{
    public class AddRoleViewModel
    {
        [Display(Name ="Role name")]
        public string RoleName {  get; set; }
    }
}
