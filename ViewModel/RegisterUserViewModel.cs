using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ghazal.ViewModel
{
    public class RegisterUserViewModel
    {
       
        [Display(Name ="Username")]
        [Required (ErrorMessage ="User name is required")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password",ErrorMessage = "Passwords do not match")]
        [Display(Name ="Confirm password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

      //  [Remote(
      //action: "IsEmailUnique",
      //controller: "Account",
      //ErrorMessage = "This email already exists"
      // )]
        public string Email { get; set; }


    }
}
