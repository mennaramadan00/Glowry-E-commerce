using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("AppUser")]
       public string AppUserId { get; set; }
       public ApplicationUser AppUser { get; set; }

        public DateTime? Date { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
