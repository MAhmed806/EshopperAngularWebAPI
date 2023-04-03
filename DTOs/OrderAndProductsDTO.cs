using EShopperAngular.Models;

namespace EShopperAngular.DTOs
{
    public class OrderAndProductsDTO
    {
        public Order myorder { get; set; }
        public List<Products> myorderproducts { get; set; }
    }
}
