using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using LadyFirstShop.Data.Services.User;
using LadyFirstShop.Data.Request;
using LadyFirstShop.Data.Constants;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LadyFirstShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class LoginController : Controller
	{
		private readonly IUserService _userService;
		private readonly IConfiguration _configuration;
		public LoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
			_configuration = configuration;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
			if (!ModelState.IsValid)
				return View(request);
			var result = await _userService.Login(request);
			if (result == UserConstant.AccountNotExist)
			{
				ModelState.AddModelError("", UserConstant.AccountNotExist);
				return View();
			}
			// giải mã token
			var userPrincipal = this.ValidateToken(result);
			var authProperties = new AuthenticationProperties
			{
				ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
				IsPersistent = false
			};
			HttpContext.Session.SetString("Token", result);
            HttpContext.Session.SetString("FullName", userPrincipal.Identity.Name);
            await HttpContext.SignInAsync(
						CookieAuthenticationDefaults.AuthenticationScheme,
						userPrincipal,
						authProperties);
            return RedirectToAction("Index", "Order");
        }
		[HttpGet]
		public async Task<IActionResult> Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			if (!ModelState.IsValid)
				return View(request);
			var result = await _userService.Register(request);
			if (result)
				return RedirectToAction("Index");
			return View();
		}
		

		[HttpGet]
        public async Task<IActionResult> Logout()
        {
            // logout những cookie cũ
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
        // Hàm giải mã token
        private ClaimsPrincipal ValidateToken(string jwtToken)
		{
			IdentityModelEventSource.ShowPII = true;

			SecurityToken validatedToken;
			TokenValidationParameters validationParameters = new TokenValidationParameters();

			validationParameters.ValidateLifetime = true;

			validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
			validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
			validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

			ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

			return principal;
		}
	}
}
