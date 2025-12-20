using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class WishListItem
    {
        [Key]
        public int WishlistItemId { get; set; }

        [ForeignKey("Wishlist")]
        public int WishlistId { get; set; }
        public WishList Wishlist { get; set; }

        [ForeignKey("ProductOption")]
        public int OptionId { get; set; }
        public ProductOption ProductOption { get; set; }
    }
}
