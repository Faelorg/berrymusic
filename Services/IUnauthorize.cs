using BerryMusicV1.Modals;
using System.Globalization;

namespace BerryMusicV1.Services
{
    public interface IUnauthorize
    {
        Task RegistrationSendCode(UserCred userCred);
        Task<object> RegistrationAcceptCode(string code);
        Task<object> AuthorizeSendCode(UserCred userCred);
        Task<object> AuthorizeAcceptCode(string code);
        Task<string> GenerateRefreshToken(string userId);
    }
}
