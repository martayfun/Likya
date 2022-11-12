using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Likya.Core.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Likya.Api.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest userRequest)
        {
            var user = await _userManager.FindByNameAsync(userRequest.UserName);

            if (user == null)
            {
                var result = await _userManager.CreateAsync(new AppUser { Email = userRequest.Email, UserName = userRequest.UserName }, userRequest.Password);

                if (result.Succeeded)
                {
                    var findUser = await _userManager.FindByNameAsync(userRequest.UserName);
                    return Ok(new
                    {
                        findUser.Id,
                        findUser.UserName,
                        findUser.Email
                    });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest("This username is taken.");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var signIn = await _signInManager.PasswordSignInAsync(loginRequest.UserName.ToLower(), loginRequest.Password, false, false);


            if (signIn != null && signIn.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginRequest.UserName);

                var token = GenerateToken(user);

                // Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                // Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                return Ok(new { signIn.Succeeded, token });
            }
            else
            {
                return BadRequest(new { signIn.IsLockedOut, signIn.IsNotAllowed, signIn.RequiresTwoFactor });
            }
        }

        private string GenerateToken(AppUser identityUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.UserName),
                new Claim(ClaimTypes.Email, identityUser.Email)
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt: Audience"], claims, expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
