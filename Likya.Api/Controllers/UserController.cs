using System.Threading.Tasks;
using Likya.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Likya.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRequest userRequest)
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
    }
}
