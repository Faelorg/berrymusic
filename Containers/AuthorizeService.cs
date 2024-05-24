using BerryMusicV1.Helpers;
using BerryMusicV1.Modals;
using BerryMusicV1.Repos;
using BerryMusicV1.Services;
using Id3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BerryMusicV1.Containers
{
    public class AuthorizeService : IAuthorize
    {
        //todo Доделать весь контроллер, сделать отлавливание ошибок
        private BMContext context;
        private readonly ILogger<User> logger;
        private readonly JwtSettings jwtSettings;
        private readonly IWebHostEnvironment environment;
        public AuthorizeService(BMContext context, ILogger<User> logger, IOptions<JwtSettings> options, IWebHostEnvironment environment)
        {
            this.context = context;
            jwtSettings = options.Value;
            this.logger = logger;
            this.environment = environment;
        }

        public async Task<APIResponse> AddMusic(List<IFormFile> musicFiles)
        {
            if (musicFiles.Count > 0)
            {

                foreach (var musicFile in musicFiles)
                {
                    MusicFile music = new MusicFile()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IsUpload = false,
                        CountLike = 0
                    };

                    using (var fs = new FileStream($"{environment.WebRootPath}\\music\\{music.Id}.mp3", FileMode.Create, FileAccess.ReadWrite))
                    {
                        await musicFile.CopyToAsync(fs);
                        using (var mp3 = new Mp3(fs))
                        {
                            var tags = mp3.GetTag(Id3TagFamily.Version2X);

                            music.Album = tags.Album + "";
                            music.Artist = tags.Artists + "";
                            music.Title = tags.Title + "";
                            music.Year = tags.Year + "";


                        }
                    }

                    await context.MusicFiles.AddAsync(music);
                    await context.SaveChangesAsync();
                }

                return new APIResponse()
                {
                    Code = 201,
                    Message = "Файлы успешно отправлены"
                };
            }

            return new APIResponse()
            {
                Code = 404,
                Message = "Не было отпралено ни одного файла"
            };
        }

        public async Task<APIResponse> AddMusicToPlaylist(string musicId, string playlistId, string userId)
        {
            logger.LogWarning(userId);
            var playlist = await context.Playlists.FindAsync(playlistId);
            if (playlist.IdUser == userId)
            {

                var music = await context.MusicFiles.FindAsync(musicId);

                var musicHasFiles = new PlaylistMusicFile()
                {
                    IdMusicFile = musicId,
                    IdPlaylist = playlistId,
                };

                music.PlaylistMusicFiles.Add(musicHasFiles);
                playlist.PlaylistMusicFiles.Add(musicHasFiles);

                await context.PlaylistMusicFiles.AddAsync(musicHasFiles);

                await context.SaveChangesAsync();

                return new APIResponse()
                {

                    Code = 200,
                    Message = "Музыка успешно добавлена в плейлист"

                };
            }
            return new APIResponse()
            {
                Code=403,
                Message="Отказано в доступе"
            };
        }

        public Task<APIResponse> AddPlaylist(PlaylistModal playlistModal, string userId)
        {
            throw new NotImplementedException();
        }
        public async Task<string> GetJWT(string refreshToken)
        {
            var token = await context.Tokens.FirstOrDefaultAsync(x => x.Token1 == refreshToken);
            var user = await context.Users.FindAsync(token.IdUser);
            return JwtToken.GenerateJwtToken(user, jwtSettings);
        }

        public async Task<APIResponse> LogOut(string deviceId, string userId)
        {
            context.Tokens.Remove(await context.Tokens.FindAsync(deviceId));

            await context.SaveChangesAsync();

            return new APIResponse()
            {
                Code = 200,
                Message = "Пользователь успешно вышел"
            };
        }

        public Task<APIResponse> RemoveMusicFromPlaylist(string musicId, string playlistId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> RemovePlaylist(string playlistId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
