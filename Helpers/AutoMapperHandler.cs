using AutoMapper;
using BerryMusicV1.Modals;
using BerryMusicV1.Repos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BerryMusicV1.Helpers
{
    public class AutoMapperHandler:Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<User, UserModal>().
                ForMember(item=>item.Role, opt=>opt.MapFrom(item=> item.IdRole==1?"Admin":"User")).
                ForMember(item=>item.Premium, opt=>opt.MapFrom(item=>item.IsPremium==true?"Премиум":"Стандартная")).
                ForMember(item => item.Status, opt => opt.MapFrom(item => item.IsBanned == true ? "Заблокирован" : "Активен"));
            CreateMap<Playlist, PlaylistModal>().
                ForMember(item => item.Status, opt => opt.MapFrom(item => item.IdStatus == 1 ? "Приватный" : "Публичный"));
        }
    }

    public class JwtToken
    {
        public static string GenerateJwtToken(User user, JwtSettings jwtSettings)
        {
            var claims = new List<Claim>()
           {
            new Claim("Name", user.Id),
            new Claim("Role", user.IdRole+"")
           };
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokendesc = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtSettings.issuer,
                Audience = jwtSettings.audience,
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.securityKey)), SecurityAlgorithms.HmacSha256)

            };

            var jwt = tokenhandler.CreateToken(tokendesc);
            return tokenhandler.WriteToken(jwt);
        }
    }
}
