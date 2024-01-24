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
        private IGenericRepository<Color> _colorRepository;
        private IGenericRepository<ProductColors> _productColorRepository;
        private IGenericRepository<ProductTypes> _productsTypesRepository;
        public ProductController(IGenericRepository<Products> repository, IGenericRepository<ProductTypes> ptyperepo, IGenericRepository<Color> colorRepo,IGenericRepository<ProductColors> productcolorrepo)
        {
            _colorRepository = colorRepo;
            _productColorRepository = productcolorrepo;
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
        [HttpPost("AddColor")]
        public ActionResult Post([FromBody]List<Color> Colors)
        {
            if (Colors != null)
            {
                List<Color> existingcolors = new List<Color>();
                foreach (var color in Colors)
                {
                    if(_colorRepository.Index().Where(x=>x.Name == color.Name)!=null)
                    {
                        var newcolor = new Color()
                        {
                            Name = color.Name
                        };
                        _colorRepository.Create(newcolor);
                    }                  
                }
                _colorRepository.Save();
                return Ok("Color/s Added Successfully");
            }
            return BadRequest("List is Empty");
        }
        [HttpGet("GetColors")]
        public ActionResult GetColors()
        {
            List<Color> colors = _colorRepository.Index().ToList();
            if(colors != null)
            {
                return Ok(colors);
            }
            return NotFound("No Colors Found in DB");
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
                    foreach (var color in product.ProductColors)
                    {
                        var pcolor = new ProductColors()
                        {
                            ProductId = product.Id,
                            ColorId = color.Id,
                        };
                        _productColorRepository.Create(pcolor);
                    }
                    _productColorRepository.Save();
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
                        List<ProductColors> ProductColors = _productColorRepository.Index().Where(x =>x.ProductId == product.Id).ToList();
                        if(ProductColors != null)
                        {
                            foreach (ProductColors color in ProductColors)
                            {
                                _productColorRepository.Delete(color.Id);
                            }
                            _productColorRepository.Save();
                        };
                        foreach(var color in product.ProductColors)
                        {
                            var prodcolor = new ProductColors()
                            {
                                ColorId = color.Id,
                                ProductId = product.Id,
                            };
                            _productColorRepository.Create(prodcolor);
                        }
                        _productColorRepository.Save();
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
