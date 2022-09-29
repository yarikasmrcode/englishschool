using EnglishSchool.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnglishSchool.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupController> _logger;
        public SetupController(
            AppDbContext context, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The role {roleName} was created");

                    return Ok(new { 
                        result = $"{roleName} was created"
                    });
                }
                else
                {
                    _logger.LogInformation($"The role {roleName} was not created");

                    return BadRequest(new
                    {
                        error = $"{roleName} was not created"
                    });
                }
            }

            return BadRequest(new {error = "role already exists"});
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogInformation($"user {email} does not exist");
                return BadRequest(new { error = "user does not exist"});
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                _logger.LogInformation($"role {roleExists} does not exist");
                return BadRequest(new { error = "role does not exist" });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (roleResult.Succeeded)
            {
                return Ok(new
                {
                    result = $"{roleName} to was added user"
                });
            }
            else
            {
                _logger.LogInformation($"The user was not able to be added to the role {roleName}");

                return BadRequest(new
                {
                    error = $"{roleName} to user was not added"
                });
            }
        }

        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogInformation($"user {email} does not exist");
                return BadRequest(new { error = "user does not exist" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogInformation($"user {email} does not exist");
                return BadRequest(new { error = "user does not exist" });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                _logger.LogInformation($"role {roleExists} does not exist");
                return BadRequest(new { error = "role does not exist" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new { result = $"User {email} was removed from role {roleName}" });
            }
            else
            {
                return BadRequest(new { error = "Unalbe to remove user from role" });
            }
        }
    }
} 
