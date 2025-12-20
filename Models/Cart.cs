using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public ApplicationUser AppUser { get; set; }

        public ICollection<CartItem> CartItems { get; set; }=new List<CartItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
