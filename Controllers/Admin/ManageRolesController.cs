using EShopperAngular.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EShopperAngular.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class ManageRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageRolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _rolemanager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet("GetRoles")]
        // GET: ManageRolesController
        public ActionResult Get()
        {
            var roles = _rolemanager.Roles;
            return Ok(roles);
        }
        public class Role
        {
            public string RoleName { get; set; }
        }
        [HttpPost("AddRole")]
        public async Task<ActionResult> PostAsync(JsonValue json)
        {
            var myrole =JsonSerializer.Deserialize<Role>(json);
            var rolename = myrole.RoleName;
            if (rolename != null)
            {
                IdentityRole identityrole = new(rolename);
                IdentityResult identityResult = await _rolemanager.CreateAsync(identityrole);
                return Ok(new { Message = "Role Added Successfully" });
            }
            return BadRequest(new { message = "Something Went Wrong" });
        }
        [HttpDelete("DeleteRole/{id}")]
        public async Task<ActionResult> delete(string id)
        {
            var role = await _rolemanager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _rolemanager.DeleteAsync(role);
                return Ok(new { message = "Role Deleted Successfully" });
            }
            return BadRequest(new { message = "No Role Found Against this Id" });
        }
        public class hehe
        {
            public IdentityRole Role { get; set; }
            public List<ApplicationUser> Users { get; set; }
        }
        [HttpGet("GetRole/{id}")]
        public async Task<ActionResult> get (string id)
        {
            if (id != null)
            {
                var role =await _rolemanager.FindByIdAsync(id);
                var users = _userManager.Users;
                var susers = new List<ApplicationUser>();
                foreach (var user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        susers.Add(user);
                    }
                }
                hehe hehe = new hehe();
                hehe.Users = susers;
                hehe.Role=role;
                return Ok(hehe);           
            }
            return BadRequest(new { message = "Invalid Id" });
        }
        [HttpPut("EditRole")]
        public async Task<ActionResult> put(JsonValue json)
        {
            
            var Role= JsonSerializer.Deserialize<IdentityRole>(json);
            var role =await _rolemanager.FindByIdAsync(Role.Id);
            role.Name = Role.Name;
            if (Role != null)
            {
              var result=  await _rolemanager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role Updated Successfully" });
                }
                return BadRequest(new { message = result.Errors });
            }

            return BadRequest(new {message="Something Went Wrong"});
        }
        public class Myuser
        {
            public List<UserData> UserData { get; set; }
            public string roleid { get; set; }
        }
        [HttpPut("EditUsersInRole")]
        public async Task<IActionResult> EditUsersinRole(JsonValue json)
        {
            if (json != null)
            {
                var UserData= JsonSerializer.Deserialize<Myuser>(json);
                var role = await _rolemanager.FindByIdAsync(UserData.roleid);
                foreach (var user in UserData.UserData)
                {
                    var myuser = await _userManager.FindByEmailAsync(user.Username);
                    if (!user.IsSelected && await _userManager.IsInRoleAsync(myuser, role.Name))
                    {
                        await _userManager.RemoveFromRoleAsync(myuser, role.Name);

                    }
                    if (user.IsSelected && !await _userManager.IsInRoleAsync(myuser, role.Name))
                    {
                        await _userManager.AddToRoleAsync(myuser, role.Name);
                    }
                }

                return Ok(new {message="Users in Role Updated Successfully"});
            }
            return NoContent();
            
        }
        [HttpGet("GetRoleOnly/{id}")]
        public async Task<ActionResult> getas(string id)
        {
            if (id != null)
            {
                var role = await _rolemanager.FindByIdAsync(id);
                if (role != null)
                {
                    return Ok(role);
                }
                return NotFound(new { message = "Role Not Found" });
            }
            return NotFound(new { message = "Invalid Id" });
           
        }
        [HttpPost("GetFilteredData")]

        public async Task<JsonResult> GetFilteredItems()
        {
            string id =Convert.ToString(Request.Query["id"]);
            var query = HttpContext.Request.Query;
            System.Threading.Thread.Sleep(100);
            //var id =Convert.ToString( HttpContext.Request.QueryString);
            //Copied from Above
            var role = await _rolemanager.FindByIdAsync(id);

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            //Copied from Above Ended

            int draw = Convert.ToInt32(Request.Query["draw"]);
            int start = Convert.ToInt32(Request.Query["start"]);
                
            // Records count to be fetched after skip
            int length = Convert.ToInt32(Request.Query["length"]);

            // Getting Sort Column Name
            int sortColumnIdx = Convert.ToInt32(Request.Query["order[0][column]"]);
            string sortColumnName = Convert.ToString(Request.Query["columns[" + sortColumnIdx + "][name]"]);
            // Sort Column Direction  
            string sortColumnDirection = Request.Query["order[0][dir]"];

            // Search Value
            string searchValue =Request.Query["search[value]"].FirstOrDefault()?.Trim();
            if(searchValue == null)
            {
                searchValue= string.Empty;
            }
            

            // Records Count matching search criteria 
            int recordsFilteredCount = model.Where(a => a.Username.Contains(searchValue)).Count();

            // Total Records Count
            int recordsTotalCount = model.Count();

            // Filtered & Sorted & Paged data to be sent from server to view
            List<UserRoleViewModel>? filteredData = new List<UserRoleViewModel>();
            if (sortColumnDirection == "asc")
            {
                var test = model.Where(a => a.Username.Contains(searchValue)).ToList();
                var test1 = test.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                filteredData = test1.Skip(start).Take(length).ToList();

            }
            else
            {
                var test = model.Where(a => a.Username.Contains(searchValue)).ToList();
                var test1 = test.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x));
                filteredData = test1.Skip(start).Take(length).ToList();
            }
            // Send data 
            return Json(
                        new
                        {
                            data = filteredData,
                            draw = Request.Query["draw"],
                            recordsFiltered = recordsFilteredCount,
                            recordsTotal = recordsTotalCount
                        }
                    );
        }
    }
}
