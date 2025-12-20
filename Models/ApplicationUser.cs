using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Glowry.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(200, ErrorMessage = "Address can't exceed 200 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "City can't exceed 100 characters")]
        public string? City { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public bool IsAdmin { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Second Phone Number")]
        [StringLength(20)]
        public string? SecondPhoneNumber { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;//viiiiiiii that is default value

        //nav
        public Cart cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}
