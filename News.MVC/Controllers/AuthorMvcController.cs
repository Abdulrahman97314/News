using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using News.Core.Services;
using News.Core.Specifications;
using News.Dtos;
using News.MVC.Models;
using News.Service;
using NewsApi.Helpers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace News.MVC.Controllers
{
    [Authorize]
    public class AuthorMvcController : Controller
    {
        private readonly HttpClient client;
        private readonly IConfiguration config;

        public AuthorMvcController(HttpClient client,IConfiguration config)
        {
            this.client = client;
            this.config = config;
            this.client.BaseAddress = new Uri("https://localhost:7229/api/Author/");
        }
        
        private string GenerateJwtToken(string Email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, Email),
                }),
                Issuer = config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.Now,
                Audience = config["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetAccessToken()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return GenerateJwtToken(userEmail);
        }

        public async Task<IActionResult> Index([FromQuery] AuthorSpecParams authorSpecParams)
        {
            var response = await client.GetAsync($"GetAllAuthors?Sort={authorSpecParams.Sort}&PageSize={authorSpecParams.PageSize}&PageIndex={authorSpecParams.PageIndex}&Search={authorSpecParams.Search}");
            var content = await response.Content.ReadAsStringAsync();
            var authors = JsonConvert.DeserializeObject<Pagination<AuthorDto>>(content);
            var viewModel = new AuthorViewModel
            {
                PaginationData = authors,
                AuthorSpecParams = authorSpecParams
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await client.GetAsync(id.ToString());
            var content = await response.Content.ReadAsStringAsync();
            var author = JsonConvert.DeserializeObject<AuthorDto>(content);
            return View(author);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorDto authorDto)
        {
            if (ModelState.IsValid)
            {
                var accessToken = GetAccessToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(JsonConvert.SerializeObject(authorDto), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("AddAuthor", content);
                return RedirectToAction(nameof(Index));
            }
            return View(authorDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await client.GetAsync(id.ToString());
            var content = await response.Content.ReadAsStringAsync();
            var author = JsonConvert.DeserializeObject<AuthorDto>(content);
            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AuthorDto authorDto)
        {
            if (ModelState.IsValid)
            {
                var accessToken = GetAccessToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(JsonConvert.SerializeObject(authorDto), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("UpdateAuthor", content);
                return RedirectToAction(nameof(Index));
            }
            return View(authorDto);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await client.GetAsync(id.ToString());
            var content = await response.Content.ReadAsStringAsync();
            var author = JsonConvert.DeserializeObject<AuthorDto>(content);
            return View(author);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessToken = GetAccessToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}