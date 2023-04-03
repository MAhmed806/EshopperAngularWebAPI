using EShopperAngular.Models;

namespace EShopperAngular.DTOs
{
    public class ProductAndProductTypesDTO
    {
        public IEnumerable<Products> products { get; set; }
        public IEnumerable<ProductTypes> ptypes { get; set; }
    }
}
