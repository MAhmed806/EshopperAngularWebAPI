namespace EShopperAngular.Models
{
    public class LoginModel
    {
       public string Email {  get; set; }
       public string PasswordHash { get; set; }
       public bool RememberMe { get; set; }
    }
}
