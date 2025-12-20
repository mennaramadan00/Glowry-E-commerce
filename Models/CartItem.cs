using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        [ForeignKey("ProductOption")]
        public int OptionId { get; set; }
        public ProductOption ProductOption { get; set; }

        public int Quantity { get; set; }
    }
}
