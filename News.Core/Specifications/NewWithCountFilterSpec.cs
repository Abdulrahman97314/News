using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Specifications
{
    public class NewsWithCountFilterSpec : BaseSpecifications<Entities.News>
    {
        public NewsWithCountFilterSpec(NewsSpecParams newsSpecParams)
            : base(news =>
            (!newsSpecParams.AuthorId.HasValue || news.AuthorId == newsSpecParams.AuthorId)&&
            (string.IsNullOrEmpty(newsSpecParams.Search)||news.Title.ToLower().Contains(newsSpecParams.Search))
            )
        {
        }
    }
}
