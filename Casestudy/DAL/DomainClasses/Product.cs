using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Product
    {
        [Key]
        [MaxLength(15)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }


        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }
        [Required]
        public int? BrandId { get; set; }


        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[]? Timer { get; set; }
        public string? ProductName { get; set; }
        public string? GraphicName { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal MSRP { get; set; }
        [Required]
        public int QtyOnHand { get; set; }
        [Required]
        public int QtyOnBackOrder { get; set; }
        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}
