using AutoMapper;
using News.Dtos;

namespace NewsApi.Helpers
{
    public class NewsPictureUrlResolver : IValueResolver<News.Core.Entities.News, NewsDto, string>
    {
        private readonly IConfiguration configuration;

        public NewsPictureUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Resolve(News.Core.Entities.News source, NewsDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ImageUrl))
                return null;
            var ImageUrl = $"{configuration["BaseUrl"]}{source.ImageUrl}";

            return ImageUrl;
        }
    }
}
