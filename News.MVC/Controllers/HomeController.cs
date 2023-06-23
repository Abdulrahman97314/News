using Microsoft.AspNetCore.Mvc;
using News.Core.Specifications;
using News.Dtos;
using News.MVC.Models;
using NewsApi.Helpers;
using Newtonsoft.Json;

namespace News.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        public HomeController(HttpClient client)
        {
            this.client = client;

            this.client.BaseAddress = new Uri("https://localhost:7229/api/");
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
    }
}
