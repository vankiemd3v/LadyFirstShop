using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LadyFirstShop.Api
{
	[Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Login(request);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Register(request);
            if (!result)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
