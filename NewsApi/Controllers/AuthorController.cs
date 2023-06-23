using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using News.Core;
using News.Core.Entities;
using News.Core.Specifications;
using News.Dtos;
using News.Errors;
using NewsApi.Helpers;

namespace News.Controllers
{
    public class AuthorController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AuthorController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet("GetAllAuthors")]
        public async Task<ActionResult<IReadOnlyList<AuthorDto>>> GetAll([FromQuery] AuthorSpecParams authorSpecParams)
        {
            var spec = new AuthorSpecifications(authorSpecParams);
            var authors = await unitOfWork.Repository<Author>().GetEntitiesWithSpecifications(spec);
            var mappedData = mapper.Map<IReadOnlyList<AuthorDto>>(authors); 
            var countSpec = new AuthorWithCountFilterSpec(authorSpecParams);
            var totalCount = await unitOfWork.Repository<Author>().CountWithSpecAsync(countSpec);
            var paginatedData = new Pagination<AuthorDto>(authorSpecParams.PageIndex, totalCount, authorSpecParams.PageSize, mappedData);
            return Ok(paginatedData);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetById(int id)
        {
            var author = await unitOfWork.Repository<Author>().GetByIdAsync(id);
            if (author == null)
            {
                return NotFound(new ApiResponse(404, "Author not Found"));
            }
            var authorDto = mapper.Map<AuthorDto>(author);
            return Ok(authorDto);
        }
        [Authorize]
        [HttpPost("AddAuthor")]
        public async Task<ActionResult<AuthorDto>> Create(AuthorDto authorDto)
        {
            var author = mapper.Map<Author>(authorDto);
            await unitOfWork.Repository<Author>().AddAsync(author);
            await unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Author has been added"));
        }
        [Authorize]
        [HttpPut("UpdateAuthor")]
        public async Task<ActionResult> Update(AuthorDto authorDto)
        {
            var author = await unitOfWork.Repository<Author>().GetByIdAsync(authorDto.Id);
            if (author == null)
            {
                return NotFound(new ApiResponse(400, "there are no user with this id"));
            }
            var mappedAuthor = mapper.Map<Author>(authorDto);
            unitOfWork.Repository<Author>().Update(mappedAuthor);
            await unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Author has been updated"));
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var author = await unitOfWork.Repository<Author>().GetByIdAsync(id);
            if (author == null)
            {
                return NotFound(new ApiResponse(404, "there are no Author With this id"));
            }
            unitOfWork.Repository<Author>().Delete(author);
            await unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Author has been deleted"));
        }
    }
}
