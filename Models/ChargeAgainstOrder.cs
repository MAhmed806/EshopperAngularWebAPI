using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShopperAngular.Models
{
    public class ChargeAgainstOrder
    {
        public int Id { get; set; }
        public string ChargeId { get; set; }
        public string ChargeStatus { get; set; }
        [Display(Name ="Order")]
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public string RefundId { get; set; }
        public string RefundStatus { get; set;}
    }
}
