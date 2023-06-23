using News.Core.Specifications;
using News.Dtos;
using NewsApi.Helpers;

namespace News.MVC.Models
{
    public class AuthorViewModel
    {
        public Pagination<AuthorDto> PaginationData { get; set; }
        public AuthorSpecParams AuthorSpecParams  { get; set; }
    }
}
