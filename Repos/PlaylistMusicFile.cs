using System;
using System.Collections.Generic;

namespace BerryMusicV1.Repos;

public partial class PlaylistMusicFile
{
    public int Id { get; set; }

    public string IdPlaylist { get; set; } = null!;

    public string IdMusicFile { get; set; } = null!;

    public virtual MusicFile IdMusicFileNavigation { get; set; } = null!;

    public virtual Playlist IdPlaylistNavigation { get; set; } = null!;
}
