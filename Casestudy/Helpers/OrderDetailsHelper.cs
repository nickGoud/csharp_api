using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.Helpers
{
    public class OrderDetailsHelper
    {
        public int orderId { get; set; }
        public string? productName { get; set; }
        [Column(TypeName = "money")]
        public decimal price { get; set; }
        public int qtyO { get; set; }
        public int qtyS { get; set; }
        public int qtyB { get; set; }
        public string? DateCreated { get; set; }
    }
}