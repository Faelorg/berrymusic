using System;
using System.Collections.Generic;

namespace BerryMusicV1.Repos;

public partial class MusicFile
{
    public string Id { get; set; } = null!;

    public bool IsUpload { get; set; }

    public string Title { get; set; } = null!;

    public string Artist { get; set; } = null!;

    public string Album { get; set; } = null!;

    public string Year { get; set; } = null!;

    public int CountLike { get; set; }

    public virtual ICollection<PlaylistMusicFile> PlaylistMusicFiles { get; set; } = new List<PlaylistMusicFile>();
}
