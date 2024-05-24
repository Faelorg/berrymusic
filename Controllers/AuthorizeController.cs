using BerryMusicV1.Containers;
using BerryMusicV1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BerryMusicV1.Controllers
{
    [Route("auth")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAuthorize service;
        public AuthorizeController(IAuthorize service)
        {
            this.service = service;
        }

        [HttpPost("file")]
        public async Task<IActionResult> AddFile(List<IFormFile> musicFiles)
        {
            var res = await service.AddMusic(musicFiles);

            return Ok(res);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMusicToPlaylist(string musicId, string playlistId)
        {
            var res = await service.AddMusicToPlaylist(musicId, playlistId, this.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Name").Value);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> GetJwtToken(string code)
        {
            var res = await service.GetJWT(code);

            return Ok(res);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut(string deviceId)
        {
            var res = await service.LogOut(deviceId, User.Identity.Name);

            return Ok(res);
        }
    }
}
