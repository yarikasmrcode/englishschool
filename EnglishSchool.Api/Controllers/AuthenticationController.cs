using EnglishSchool.Common.Dtos;
using EnglishSchool.DAL.Configs;
using EnglishSchool.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnglishSchool.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            // validate the incoming request
            if (ModelState.IsValid)
            {
                //if the email exist
                var userExists = await _userManager.FindByEmailAsync(requestDto.Email);

                if (userExists != null)
                {
                    return BadRequest(new AuthResult() {Result = false, Errors = new List<string>() 
                    { 
                        "Email already exists"
                    } });
                }

                var user = new IdentityUser()
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Email
                };

                var newUser = new IdentityUser() { Email = user.Email, UserName = user.Email };

                var isCreated = await _userManager.CreateAsync(newUser, requestDto.Password);

                if (isCreated.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "AppUser");

                    var token = await GenerateJwtToken(newUser);
                    return Ok(new AuthResult() { Result = true, Token = token });
                }
                else
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                             {
                                 "server error bruh"
                             }
                    });
                }
            }
            else return BadRequest();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var existingUser =
                    await _userManager.FindByEmailAsync(requestDto.Email);

                if (existingUser == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors
                    = new List<string>() { "Invalid payload" }
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, requestDto.Password);

                if (!isCorrect)
                {
                    return BadRequest(new AuthResult() { Result = false,
                    Errors = new List<string>() { "Invalid credentials"}
                    });
                }

                var jwtToken = await GenerateJwtToken(existingUser);

                return Ok(new AuthResult() { Result = true, Token = jwtToken});
            }

            return BadRequest(new AuthResult()
            { 
                Result = false,
                Errors = new List<string>() { "Invalid payload"}
            });
        }
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            var claims = await GetAllValidClaims(user);
            
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                , SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;

        }

        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var options = new IdentityOptions();

            var claims = new List<Claim>()
            {
                new Claim("Id",user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                     foreach (var roleClaim in roleClaims)
                     {
                        claims.Add(roleClaim);
                     }
                }
            }

            return claims;
        }
    }
}
