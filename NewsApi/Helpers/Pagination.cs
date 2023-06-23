using News.Core.Entities;

namespace NewsApi.Helpers
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageIndex, int totalCount, int pageSize, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            TotalCount = totalCount;
            PageSize = pageSize;
            Data = data;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        }
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
