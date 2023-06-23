using News.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Specifications
{
    public class AuthorWithCountFilterSpec : BaseSpecifications<Author>
    {
        public AuthorWithCountFilterSpec(AuthorSpecParams authorSpecParams) 
            : base(author => (string.IsNullOrEmpty(authorSpecParams.Search) || author.Name.ToLower().Contains(authorSpecParams.Search)))
        {
        }
    }
}
