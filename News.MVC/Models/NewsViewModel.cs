using News.Core.Specifications;
using News.Dtos;
using NewsApi.Helpers;

namespace News.MVC.Models
{
    public class NewsViewModel
    {
        public Pagination<NewsDto> PaginationData  { get; set; }
        public NewsSpecParams NewsSpecParams { get; set; }

    }
}
