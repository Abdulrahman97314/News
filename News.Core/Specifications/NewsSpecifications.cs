
using System.Linq.Expressions;

namespace News.Core.Specifications
{
    public class NewsSpecifications : BaseSpecifications<Entities.News>
    {
        public NewsSpecifications(NewsSpecParams newsSpecParams)
            : base(news => (!newsSpecParams.AuthorId.HasValue || news.AuthorId == newsSpecParams.AuthorId) &&
                (string.IsNullOrEmpty(newsSpecParams.Search) || news.Title.ToLower().Contains(newsSpecParams.Search)))
        {
            AddInclude(news => news.Author);
            ApplayPaging(newsSpecParams.PageSize, newsSpecParams.PageSize * (newsSpecParams.PageIndex - 1));
            if (!string.IsNullOrEmpty(newsSpecParams.Sort))
            {
                switch (newsSpecParams.Sort)
                {
                    case "CDateAsc":
                        AddOrderBy(news => news.CreationDate);
                        break;
                    case "CDateDesc":
                        AddOrderDescending(news => news.CreationDate);
                        break;
                    case "PDateAsc":
                        AddOrderBy(news => news.PublicationDate);
                        break;
                    case "PDateDesc":
                        AddOrderDescending(news => news.PublicationDate);
                        break;
                }
            }
        }

        public NewsSpecifications(int id)
            : base(news => news.Id == id)
        {
            AddInclude(news => news.Author);
        }
    }

}
