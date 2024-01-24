using System.ComponentModel.DataAnnotations.Schema;

namespace EShopperAngular.Models
{
    public class ProductColors
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
    }
}
