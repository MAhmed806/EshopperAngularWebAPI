using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EShopperAngular.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Stripe;

namespace EShopperAngular.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(JsonValue json)
        {
            ApplicationUser user= JsonSerializer.Deserialize<ApplicationUser>(json);
            string userrole = "";
            var result = await _signInManager.PasswordSignInAsync(user.Email,user.PasswordHash,user.RememberMe, false);
           var myuser=await _userManager.FindByEmailAsync(user.Email);
            if (!result.Succeeded)
            {
                return Ok(new {result});
            }
            var roles = _roleManager.Roles;
            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(myuser, role.NormalizedName))
                {
                    userrole= role.Name;
                }
            }
            var token = GenerateJwtToken(user.Email,userrole);
            return Ok(new { token,result });
        }
        private string GenerateJwtToken(string email,string userrole)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(JwtRegisteredClaimNames.Acr,userrole),

        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(JsonValue json)
        {
            var user = new ApplicationUser();
            user = JsonSerializer.Deserialize<ApplicationUser>(json);
            user.UserName = user.Email;
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, user.PasswordHash);
            //_userManager.AddToRoleAsync(user, "Customer");
            if (!result.Succeeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
    }
   
}
