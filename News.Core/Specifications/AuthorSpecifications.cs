using News.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Specifications
{
    public class AuthorSpecifications : BaseSpecifications<Author>
    {
        public AuthorSpecifications(AuthorSpecParams authorSpecParams )
            : base(author =>(string.IsNullOrEmpty(authorSpecParams.Search) || author.Name.ToLower().Contains(authorSpecParams.Search)))
        {
            ApplayPaging(authorSpecParams.PageSize, authorSpecParams.PageSize * (authorSpecParams.PageIndex - 1));

                switch (authorSpecParams.Sort)
                {
                    case "NameAsc":
                        AddOrderBy(news => news.Name); break;
                    case "NameDes":
                        AddOrderDescending(news => news.Name); break;
                    default:
                        AddOrderBy(news => news.Id);
                        break;
                }
        }
    }
}
