using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // FK → Order
        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // FK → ProductOption
        [Required]
        [ForeignKey("ProductOption")]
        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; }

        // Quantity ordered
        [Required]
        public int Quantity { get; set; }

        // Price at the time of purchase (snapshot)
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAtPurchase { get; set; }
    }
}
