namespace BerryMusicV1.Modals
{
    public class UserModal
    {
        public string Email { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;
        public string Premium { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
    public class PlaylistModal
    {
        public string Name { get; set; } = null!;

        public string IdUser { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
    public class TokenResponse
    {
        public string Token { get; set; }

        public string Refresh { get; set; }
    }
    public class JwtSettings
    {
        public string securityKey { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
    }
    public class UserCred
    {
        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string DeviceId { get; set; }
    }
    public class APIResponse
    {
        public int Code { get;set; }
        public string Message { get; set; }
    }
}
