using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News.Controllers;
using News.Core.Services;
using News.Errors;
using NewsApi.Dtos;
using NuGet.Common;

namespace NewsApi.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ITokenService tokenService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401, "The email address is incorrect."));
            var password = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!password.Succeeded)
                return Unauthorized(new ApiResponse(401, "The password is incorrect."));
            var token = await tokenService.CreateTokenAsync(user, userManager);
            return Ok(new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            });
        }
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = await userManager.FindByEmailAsync(registerDto.Email);
            if (user is not null)
                return BadRequest(new ApiResponse(400, "This email is already registered"));
            var appUser = new IdentityUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
            };
            var result = await userManager.CreateAsync(appUser, registerDto.Password);
            if (result.Succeeded)
            {
                return Ok(new UserDto
                {
                    UserName = appUser.UserName,
                    Email = appUser.Email,
                    Token = await tokenService.CreateTokenAsync(appUser, userManager)
                });
            }
            else return BadRequest(new ApiResponse(400, "Failed to register user"));
        }
    }
}
