using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using News.Core;
using News.Core.Entities;
using News.Core.Specifications;
using News.Dtos;
using News.Errors;
using News.Helpers;
using NewsApi.Helpers;

namespace News.Controllers
{
    public class NewsController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public NewsController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet("GetAllNews")]
        public async Task<ActionResult<Pagination<NewsDto>>> GetAllNews([FromQuery]NewsSpecParams newsSpecParams)
        {
            var spec = new NewsSpecifications(newsSpecParams);
            var news = await unitOfWork.Repository<Core.Entities.News>().GetEntitiesWithSpecifications(spec);
            var mappedNews = mapper.Map<IReadOnlyList<NewsDto>>(news);
            var countSpec = new NewsWithCountFilterSpec(newsSpecParams);
            var totalCount = await unitOfWork.Repository<Core.Entities.News>().CountWithSpecAsync(countSpec);
            var paginatedData = new Pagination<NewsDto>(newsSpecParams.PageIndex,totalCount,newsSpecParams.PageSize,mappedNews);
            return Ok(paginatedData);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetNewsById(int id)
        {
            var spec = new NewsSpecifications(id);
            var news = await unitOfWork.Repository<Core.Entities.News>().GetEntityWithSpecifications(spec);
            if (news is null)
                return NotFound(new ApiResponse(404,"news not found"));

            var mappedNews = mapper.Map<NewsDto>(news);
            return Ok(mappedNews);
        }
        [Authorize]
        [HttpPost("Create")]
        public async Task<ActionResult<NewsDto>> Create([FromForm] NewsDto news)
        {
            var author = await unitOfWork.Repository<Author>().GetByIdAsync(news.AuthorId);
            if(author is null)
            return NotFound(new ApiResponse(404, "Author not Found"));
            news.ImageUrl = PictureSettings.UploadFile(news.Image, "news");
            var mappedNews = mapper.Map<Core.Entities.News>(news);
            await unitOfWork.Repository<Core.Entities.News>().AddAsync(mappedNews);
            await unitOfWork.CompleteAsync();
            return Ok(news);
        }
        [Authorize]
        [HttpPut("Update")]
        public async Task<ActionResult<NewsDto>> Update([FromForm] NewsDto newsDto)
        {
            var author = await unitOfWork.Repository<Author>().GetByIdAsync(newsDto.AuthorId);
            if (author is null)
                return NotFound(new ApiResponse(404, "Author not Found"));
            var news = await unitOfWork.Repository<Core.Entities.News>().GetByIdAsync(newsDto.Id);
            if (news is null)
            {
                return NotFound(new ApiResponse(404, "News not found"));
            }
            newsDto.ImageUrl = PictureSettings.UploadFile(newsDto.Image, "news");
            var mappedNews = mapper.Map<Core.Entities.News>(newsDto);
            unitOfWork.Repository<Core.Entities.News>().Update(mappedNews);
            await unitOfWork.CompleteAsync();
            return Ok(news);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var news = await unitOfWork.Repository<Core.Entities.News>().GetByIdAsync(id);
            if (news is null)
                return NotFound(new ApiResponse(404, "news not found"));
            unitOfWork.Repository<Core.Entities.News>().Delete(news);
            PictureSettings.DeleteFile(news.ImageUrl);
            await unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200,"news have been deleted"));
        }
    }
}
