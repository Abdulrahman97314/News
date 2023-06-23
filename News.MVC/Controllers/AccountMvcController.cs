using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using News.Core.Specifications;
using News.Dtos;
using News.Errors;
using NewsApi.Dtos;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace News.MVC.Controllers
{
    public class AccountMvcController : Controller
    {
        private readonly HttpClient client;

        public AccountMvcController(HttpClient client)
        {
            this.client = client;
            this.client.BaseAddress = new Uri("https://localhost:7229/api/Account/");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync("Login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userDto = JsonConvert.DeserializeObject<UserDto>(responseContent);

                    // Set authentication cookie
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Email),
                new Claim(ClaimTypes.Name, userDto.UserName),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMonths(1)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity), authProperties);
                    return RedirectToAction("Index", "NewsMvc");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                    ModelState.AddModelError("", apiResponse.Message);
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while logging in.");
                }
            }
            return View(loginDto);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
    }
}
