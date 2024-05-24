using BerryMusicV1.Modals;
using BerryMusicV1.Repos;

namespace BerryMusicV1.Services
{
    public interface IAuthorize
    {
        public Task<string> GetJWT(string refreshToken);
        public Task<APIResponse> AddMusic(List<IFormFile> musicFiles);
        public Task<APIResponse> AddMusicToPlaylist(string musicId, string playlistId, string userId);
        public Task<APIResponse> RemoveMusicFromPlaylist(string musicId, string playlistId, string userId);
        public Task<APIResponse> AddPlaylist(PlaylistModal playlistModal, string userId);
        public Task<APIResponse> RemovePlaylist(string playlistId, string userId);
        public Task<APIResponse> LogOut(string deviceId, string userId);
    }
}
