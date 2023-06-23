using AutoMapper;
using News.Core.Entities;
using News.Dtos;
using NewsApi.Helpers;

namespace News.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<Core.Entities.News, NewsDto>().ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<NewsPictureUrlResolver>()).ReverseMap();
        }
    }
}
