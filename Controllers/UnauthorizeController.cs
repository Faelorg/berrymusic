using BerryMusicV1.Modals;
using BerryMusicV1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BerryMusicV1.Controllers
{
    [Route("unauth")]
    [ApiController]
    public class UnauthorizeController : ControllerBase
    {
        private readonly IUnauthorize service;
        public UnauthorizeController(IUnauthorize service)
        {
            this.service = service;
        }

        [HttpPost("reg")]
        public async Task<IActionResult> RegistrationCodeSend(UserCred userCred)
        {
            await service.RegistrationSendCode(userCred);

            return Ok();
        }    

        [HttpPost("reg/code")]
        public async Task<IActionResult> RegistrationCodeAccept(string code)
        {
            return Ok(await service.RegistrationAcceptCode(code));
        }

        [HttpPost("auth")]
        public async Task<IActionResult> AuthorizationCodeSend(UserCred userCred)
        {
            var res = await service.AuthorizeSendCode(userCred);
            return Ok(res);
        }

        [HttpPost("auth/code")]
        public async Task<IActionResult> AuthorizationCodeAccept(string code)
        {
            var res = await service.AuthorizeAcceptCode(code);

            return Ok(res);
        }


    }
}
