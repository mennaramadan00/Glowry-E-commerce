using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class WishList
    {
        [Key]
        public int WishlistId { get; set; }

        [ForeignKey("AppUser")]
        public String AppUserId { get; set; }
        public ApplicationUser AppUser { get; set; }

        public ICollection<WishListItem> WishlistItems { get; set; }= new List<WishListItem>();
    }
}
