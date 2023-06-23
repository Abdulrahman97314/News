using Microsoft.AspNetCore.Mvc;
using News.Dtos;
using Newtonsoft.Json;
using NewsApi.Helpers;
using News.Core.Specifications;
using News.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Headers;

namespace News.MVC.Controllers
{
    [Authorize]
    public class NewsMvcController : Controller
    {
        private readonly HttpClient client;
        private readonly IConfiguration config;

        public NewsMvcController(HttpClient client, IConfiguration config)
        {
            this.client = client;
            this.config = config;
            this.client.BaseAddress = new Uri("https://localhost:7229/api/");
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
        public async Task<IActionResult> Index([FromQuery] NewsSpecParams newsSpecParams)
        {
            var response = await client.GetAsync($"News/GetAllNews?AuthorId={newsSpecParams.AuthorId}&Sort={newsSpecParams.Sort}&PageSize={newsSpecParams.PageSize}&PageIndex={newsSpecParams.PageIndex}&Search={newsSpecParams.Search}");
            var content = await response.Content.ReadAsStringAsync();
            var news = JsonConvert.DeserializeObject<Pagination<NewsDto>>(content);
            var viewModel = new NewsViewModel
            {
                PaginationData = news,
                NewsSpecParams = newsSpecParams
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            var response = await client.GetAsync($"News/{id}");
            var content = await response.Content.ReadAsStringAsync();
            var news = JsonConvert.DeserializeObject<NewsDto>(content);
            return View(news);
        }
        public async Task<IActionResult> Create()
        {
            var authors = await GetAllAuthors();
            ViewData["Authors"] = authors;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(NewsDto newsDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Authors"] = await GetAllAuthors();
                return View(newsDto);
            }
            var accessToken = GetAccessToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var stream = new MemoryStream();
            await newsDto.Image.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var formData = new MultipartFormDataContent
            {
        { new StreamContent(stream), "Image", newsDto.Image.FileName },
        { new StringContent(newsDto.Title), "Title" },
        { new StringContent(newsDto.AuthorId.ToString()), "AuthorId" },
        { new StringContent(newsDto.NewsContent), "NewsContent" },
        { new StringContent(newsDto.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), "PublicationDate" }
            };

            var response = await client.PostAsync("News/Create", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["Authors"] = await GetAllAuthors();

            return View(newsDto);
        }
        private async Task<IReadOnlyList<AuthorDto>> GetAllAuthors()
        {
            var authorResponse = await client.GetAsync("Author/GetAllAuthors");
            var authorResponseContent = await authorResponse.Content.ReadAsStringAsync();
            var authors = JsonConvert.DeserializeObject<Pagination<AuthorDto>>(authorResponseContent);
            return authors.Data;
        }
        public async Task<IActionResult> Edit(int id)
        {
            var response = await client.GetAsync($"News/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsDto>(content);

                ViewData["Authors"] = await GetAllAuthors();
                return View(news);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NewsDto newsDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Authors"] = await GetAllAuthors();
                return View(newsDto);
            }
            var accessToken = GetAccessToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var stream = new MemoryStream();
            await newsDto.Image.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var formData = new MultipartFormDataContent
            {
        { new StreamContent(stream), "Image", newsDto.Image.FileName },
        { new StringContent(newsDto.Title), "Title" },
        { new StringContent(newsDto.AuthorId.ToString()), "AuthorId" },
        { new StringContent(newsDto.NewsContent), "NewsContent" },
        { new StringContent(newsDto.Id.ToString()), "Id" },
        { new StringContent(newsDto.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), "PublicationDate" }
            };

            var response = await client.PutAsync("News/Update", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["Authors"] = await GetAllAuthors();

            return View(newsDto);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await client.GetAsync($"News/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsDto>(content);
                return View(news);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessToken = GetAccessToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.DeleteAsync($"News/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
