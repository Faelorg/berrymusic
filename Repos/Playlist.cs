using System;
using System.Collections.Generic;

namespace BerryMusicV1.Repos;

public partial class Playlist
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string IdUser { get; set; } = null!;

    public int IdStatus { get; set; }

    public virtual Status IdStatusNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<PlaylistMusicFile> PlaylistMusicFiles { get; set; } = new List<PlaylistMusicFile>();
}
