using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Specifications
{
    public class AuthorSpecParams
    {
        public string? Sort { set; get; }
        private const int MAXPAGESIZE = 50;
        private int pageSize = 6;
        public int PageSize { get => pageSize; set => pageSize = value < MAXPAGESIZE ? value : MAXPAGESIZE; }
        public int PageIndex { set; get; } = 1;
        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }
    }
}
