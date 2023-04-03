using Microsoft.AspNetCore.Mvc.Formatters;

namespace EShopperAngular.Models
{
    public class TestUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RememberMe { get; set; }
    }
}
