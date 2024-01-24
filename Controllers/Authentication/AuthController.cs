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
using Newtonsoft.Json.Linq;

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
        public AuthController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                string userrole = "";
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, login.PasswordHash);
                var result = await _signInManager.CheckPasswordSignInAsync(user, login.PasswordHash, false);
                if (!result.Succeeded)
                {
                    return Ok(new { result });
                }
                var roles = await _userManager.GetRolesAsync(user);
                userrole = roles.FirstOrDefault();
                var token = GenerateJwtToken(login.Email, userrole,user.UserName);
                return Ok(new { token, result });
            }
            catch (JsonException ex)
            {
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(JsonValue json)
        {
            var user = new ApplicationUser();
            user = JsonSerializer.Deserialize<ApplicationUser>(json);
            user.UserName = user.Email.Split('@')[0];
            var result = await _userManager.CreateAsync(user, user.PasswordHash);
            //_userManager.AddToRoleAsync(user, "Customer");
            if (!result.Succeeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
        private string GenerateJwtToken(string email, string? userrole, string username)
        {
            try
            {
                userrole = userrole == null ? string.Empty : userrole;
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(JwtRegisteredClaimNames.Acr,userrole),
                    new Claim(JwtRegisteredClaimNames.UniqueName,username)

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
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }

}
