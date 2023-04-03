using EShopperAngular.Models;
using EShopperAngular.Repositories.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EShopperAngular.Controllers.Manager
{
    [Route("/api/Manager")]
    [ApiController]
    public class ProductController : Controller
    {
        private IGenericRepository<Products> _productsRepository;
        private IGenericRepository<ProductTypes> _productsTypesRepository;
        public ProductController(IGenericRepository<Products> repository, IGenericRepository<ProductTypes> ptyperepo)
        {
            _productsRepository = repository;
            _productsTypesRepository = ptyperepo;
        }
        [HttpGet("Products")]
        // GET: product
        public IEnumerable<Products> Get()
        {
            var products = _productsRepository.Index();
            foreach (var product in products)
            {
                product.ProductTypes = _productsTypesRepository.Details(product.ProductTypeID);
            }
            return products;
        }
        [HttpGet("ProductDetails/{id}")]
        public ActionResult<Products> Get(int id)
        {
            if (id != null)
            {
                var prod = _productsRepository.Details(id);
                prod.ProductTypes = _productsTypesRepository.Details(prod.ProductTypeID);
                if (prod == null) { return NotFound(); }
                else { return prod; }

            }
            return BadRequest("Invalid Id");

        }
        [HttpPost("AddProduct")]
        public ActionResult<Products> Post(JsonObject json)
        {
            var product = JsonSerializer.Deserialize<Products>(json);
            var imagestring = (string)json["Imagestring"];
            if (product != null)
            {
                try
                {
                    string finalimage = imagestring.Trim('"').Split(',')[1];
                    byte[] imageBytes = Convert.FromBase64String(finalimage);
                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularPortal/src/assets/images/", product.Image);
                    string filePath2 = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularCustomer/src/assets/images/", product.Image);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    System.IO.File.WriteAllBytes(filePath2, imageBytes);
                    product.Image = "assets/images/" + product.Image;
                    _productsRepository.Create(product);
                    _productsRepository.Save();
                    return Ok(product);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
            }
            else
            {
                return BadRequest("Empty Post Call");
            }
        }
        [HttpPut("UpdateProduct")]
        public ActionResult<Products> Put(JsonObject json)
        {
            var product = JsonSerializer.Deserialize<Products>(json);
            string imagestring = (string)json["Imagestring"];
            if (product != null)
            {
                if (imagestring != null)
                {
                    try
                    {
                        string finalimage = imagestring.Trim('"').Split(',')[1];
                        byte[] imageBytes = Convert.FromBase64String(finalimage);
                        string fileName = Guid.NewGuid().ToString() + ".jpg";
                        string filePath = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularPortal/src/assets/images/", product.Image);
                        string filePath2 = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularCustomer/src/assets/images/", product.Image);
                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                        System.IO.File.WriteAllBytes(filePath2, imageBytes);
                        product.Image = "assets/images/" + product.Image;
                        _productsRepository.Edit(product);
                        _productsRepository.Save();
                        return Ok(product);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { Message = ex.Message });
                    }
                }
                _productsRepository.Edit(product);
                _productsRepository.Save();
                return Ok(product);
            }
            return NoContent();

        }
        [HttpDelete("DeleteProduct/{id}")]
        public ActionResult Delete(int id)
        {
            if(id == null)
            {
                return BadRequest("Incorrect ID");
            }                       
            var prod = _productsRepository.Details(id);
            if (prod != null)
            {
                _productsRepository.Delete(id);
                _productsRepository.Save();
                return Ok(new {Message="Product Deleted Successfully"});
            }
            else
            { return BadRequest("Product Not Found"); }
        }
    }
}
