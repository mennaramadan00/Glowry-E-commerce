using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class ProductImg
    {
        [Key]
        public int ImgId { get; set; }

        public byte[] Img { get; set; }

        [MaxLength(150)]
        public string Alt { get; set; }

        public bool IsMain { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public ICollection<ImageMap> ImageMaps { get; set; }= new List<ImageMap>();
    }
}
