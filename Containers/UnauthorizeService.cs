using AutoMapper;
using BerryMusicV1.Helpers;
using BerryMusicV1.Modals;
using BerryMusicV1.Repos;
using BerryMusicV1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BerryMusicV1.Containers
{
    public class UnauthorizeService : IUnauthorize
    {
        private readonly BMContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<User> logger;
        private readonly JwtSettings jwtSettings;
        private readonly IMemoryCache memoryCache;

        public UnauthorizeService(BMContext context, IMapper mapper, ILogger<User> logger, IOptions<JwtSettings> options, IMemoryCache memoryCache)
        {
            this._context = context;
            this.mapper = mapper;
            this.logger = logger;
            this.jwtSettings = options.Value;
            this.memoryCache = memoryCache;
        }

        #region Регистрация
        public async Task RegistrationSendCode(UserCred userCred)
        {
            logger.Log(LogLevel.Information, $"Пользователь {userCred.Email} начал регистрацию в {DateTime.Now}");
            await SendCode(userCred);
        }
        public async Task<object> RegistrationAcceptCode(string code)
        {
            var userCred = memoryCache.Get<UserCred>(code);

            if (userCred != null)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userCred.Email,
                    IdRole = 2,
                    Password = userCred.Password,
                    Login = userCred.Login,
                    IsBanned = false,
                    IsPremium = false,
                };

                var playlist = new Playlist()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Избранное",
                    IdStatus = 1,
                    IdUser = user.Id,
                };

                user.Playlists = new List<Playlist>() { playlist };
                string jwtToken = JwtToken.GenerateJwtToken(user, jwtSettings);
                string refresh = await GenerateRefreshToken(user.Id);
                await _context.Users.AddAsync(user);
                await _context.Playlists.AddAsync(playlist);
                await _context.SaveChangesAsync();

                logger.Log(LogLevel.Information, $"Пользователь {userCred.Email} завершил регистрацию в {DateTime.Now}");
                return new TokenResponse()
                {
                    Token = jwtToken,
                    Refresh = refresh,
                };
            }
            return new APIResponse { Code = 400, Message = "Неверный код" };
        }
        #endregion

        #region Авторизация
        public async Task<object> AuthorizeSendCode(UserCred userCred)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Login == userCred.Login || userCred.Email == u.Email) && userCred.Password == u.Password);
            if (user != null)
            {
                var device = await _context.Tokens.FindAsync(userCred.DeviceId);
                if (device == null)
                {
                    logger.Log(LogLevel.Information, $"Пользователь {user.Email} начал попытку авторизации в {DateTime.Now}");
                    await SendCode(user);
                    return null;
                }
                logger.Log(LogLevel.Information, $"Пользователь {user.Email} был авторизован на устройстве {device.IdDevice} в {DateTime.Now}");
                return new APIResponse
                {
                    Code = 409,
                    Message = "С данного устройства уже был выполнен вход"
                };
            }
            logger.Log(LogLevel.Information, $"Пользователю {user.Email} не удалось выполнить вход в {DateTime.Now}");
            return new APIResponse
            {
                Code = 401,
                Message = "Неверный логин или пароль"
            };
        }
        public async Task<object> AuthorizeAcceptCode(string code)
        {
            var user = memoryCache.Get<User>(code);

            string jwtToken = JwtToken.GenerateJwtToken(user, jwtSettings);
            string refresh = await GenerateRefreshToken(user.Id);

            if (user != null)
            {
                return new TokenResponse()
                {
                    Refresh = refresh,
                    Token = jwtToken
                };
            }

            return new APIResponse()
            {
                Code = 400,
                Message = "Неверный код"
            };
        }
        #endregion
        private async Task SendCode(UserCred userCred)
        {
            string from = "berrystudio@yandex.ru";
            string code = "";
            for (int i = 0; i < 6; i++)
            {
                var s = Math.Round(new Random().NextInt64(0, 9) * DateTime.Now.Ticks / 100d, 0).ToString();

                code += s[new Random().Next(0, s.Length)];
            }

            SmtpClient client = new SmtpClient("smtp.yandex.ru", 587);
            client.Credentials = new NetworkCredential("berrystudio", "gzyemynlfnblucth");
            client.EnableSsl = true;
            await client.SendMailAsync(new MailMessage(from, userCred.Email, "Код для подтверждения почты", code));

            memoryCache.Set(code, userCred, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }
        private async Task SendCode(User user)
        {
            string from = "berrystudio@yandex.ru";
            string code = "";
            for (int i = 0; i < 6; i++)
            {
                var s = Math.Round(new Random().NextInt64(0, 9) * DateTime.Now.Ticks / 100d, 0).ToString();

                code += s[new Random().Next(0, s.Length)];
            }

            SmtpClient client = new SmtpClient("smtp.yandex.ru", 587);
            client.Credentials = new NetworkCredential("berrystudio", "gzyemynlfnblucth");
            client.EnableSsl = true;
            await client.SendMailAsync(new MailMessage(from, user.Email, "Код для подтверждения почты", code));

            memoryCache.Set(code, user, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }

        //JWT tokens
        public async Task<string> GenerateRefreshToken(string userId)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
                await this._context.Tokens.AddAsync(new Token()
                {
                    Token1 = refreshToken,
                    IdUser = userId,
                    IdDevice = Guid.NewGuid().ToString()
                });
                await this._context.SaveChangesAsync();
                return refreshToken;
            }
        }

        //todo Сдклать Хэширование пароля
        public string HashedPassword(string password)
        {
            return "";
        }
    }
}
