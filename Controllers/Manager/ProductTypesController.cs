using EShopperAngular.Models;
using EShopperAngular.Repositories.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EShopperAngular.Controllers.Manager
{
    [Route("/api/Manager")]
    public class ProductTypesController : Controller
    {
        private IGenericRepository<ProductTypes> _ptypeRepository;
        public ProductTypesController(IGenericRepository<ProductTypes> repository)
        {
            _ptypeRepository = repository;
        }
        [HttpGet("ProductTypes")]
        // GET: product
        public IEnumerable<ProductTypes> Get()
        {
            return _ptypeRepository.Index();
        }
        [HttpGet("ProductTypeDetails/{id}")]
        public ActionResult<ProductTypes> Get(int id)
        {
            var ptype = _ptypeRepository.Details(id);
            if (ptype == null) { return NotFound(); }
            else { return ptype; }
        }
        [HttpPost("AddProductType")]
        public ActionResult<ProductTypes> Post([FromBody] JsonObject json)
        {
            var ptype = JsonSerializer.Deserialize<ProductTypes>(json);
            var imagestring = (string)json["Imagestring"];
            if (ptype != null)
            {
                try
                {
                    string finalimage = imagestring.Trim('"').Split(',')[1];
                    byte[] imageBytes = Convert.FromBase64String(finalimage);
                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularPortal/src/assets/images/", ptype.Image);
                    string filePath2 = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularCustomer/src/assets/images/", ptype.Image);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    System.IO.File.WriteAllBytes(filePath2, imageBytes);
                    ptype.Image = "assets/images/" + ptype.Image;
                    _ptypeRepository.Create(ptype);
                    _ptypeRepository.Save();
                    return Ok(ptype);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
            }
            else
            {
                return NoContent();
            }
        }
        [HttpPut("UpdateProductType")]
        public ActionResult<ProductTypes> Put([FromBody] JsonObject json)
        {
            var ptype = JsonSerializer.Deserialize<ProductTypes>(json);
            var imagestring = (string)json["Imagestring"];
            if (ptype.Id != null)
            {
                if (imagestring != null)
                {
                    try
                    {
                        string finalimage = imagestring.Trim('"').Split(',')[1];
                        byte[] imageBytes = Convert.FromBase64String(finalimage);
                        string fileName = Guid.NewGuid().ToString() + ".jpg";
                        string filePath = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularPortal/src/assets/images/", ptype.Image);
                        string filePath2 = Path.Combine("C:/Users/Ahmed/source/repos/EShopperAngularCustomer/src/assets/images/", ptype.Image);
                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                        System.IO.File.WriteAllBytes(filePath2, imageBytes);
                        ptype.Image = "assets/images/" + ptype.Image;
                        _ptypeRepository.Edit(ptype);
                        _ptypeRepository.Save();
                        return Ok(new { Message = "Product Type Updated Successfully" });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { Message = ex.Message });
                    }
                }
                _ptypeRepository.Edit(ptype);
                _ptypeRepository.Save();
                return Ok(new {Message= "Product Type Updated Successfully" });
            }
            return NotFound();
        }
        [HttpDelete("DeleteProductType/{id}")]  
        public ActionResult<ProductTypes> Delete(int id)
        {
            if(id== 0||id == null)
            {
                return BadRequest();
            }
            var prod = _ptypeRepository.Details(id);
            if (prod != null)
            {
                _ptypeRepository.Delete(id);
                _ptypeRepository.Save();
               
                return Ok();
            }
            else
            { return NotFound(); }
        }
    }
}                           
