using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BerryMusicV1.Repos;

public partial class BMContext : DbContext
{
    public BMContext()
    {
    }

    public BMContext(DbContextOptions<BMContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MusicFile> MusicFiles { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistMusicFile> PlaylistMusicFiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MusicFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("MusicFile");

            entity.Property(e => e.Id)
                .HasMaxLength(250)
                .HasColumnName("ID");
            entity.Property(e => e.Album).HasMaxLength(250);
            entity.Property(e => e.Artist).HasMaxLength(250);
            entity.Property(e => e.CountLike).HasColumnType("int(11)");
            entity.Property(e => e.IsUpload).HasColumnName("isUpload");
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Year).HasMaxLength(250);
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Playlist");

            entity.HasIndex(e => e.IdStatus, "id_Status");

            entity.HasIndex(e => new { e.IdUser, e.IdStatus }, "id_User");

            entity.Property(e => e.Id)
                .HasMaxLength(250)
                .HasColumnName("ID");
            entity.Property(e => e.IdStatus)
                .HasColumnType("int(11)")
                .HasColumnName("id_Status");
            entity.Property(e => e.IdUser)
                .HasMaxLength(250)
                .HasColumnName("id_User");
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.IdStatus)
                .HasConstraintName("playlist_ibfk_2");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("playlist_ibfk_1");
        });

        modelBuilder.Entity<PlaylistMusicFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Playlist_MusicFile");

            entity.HasIndex(e => e.IdMusicFile, "id_MusicFile");

            entity.HasIndex(e => new { e.IdPlaylist, e.IdMusicFile }, "id_Playlist");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.IdMusicFile)
                .HasMaxLength(250)
                .HasColumnName("id_MusicFile");
            entity.Property(e => e.IdPlaylist)
                .HasMaxLength(250)
                .HasColumnName("id_Playlist");

            entity.HasOne(d => d.IdMusicFileNavigation).WithMany(p => p.PlaylistMusicFiles)
                .HasForeignKey(d => d.IdMusicFile)
                .HasConstraintName("playlist_musicfile_ibfk_2");

            entity.HasOne(d => d.IdPlaylistNavigation).WithMany(p => p.PlaylistMusicFiles)
                .HasForeignKey(d => d.IdPlaylist)
                .HasConstraintName("playlist_musicfile_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Status");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.IdDevice).HasName("PRIMARY");

            entity.ToTable("Token");

            entity.Property(e => e.IdDevice)
                .HasMaxLength(250)
                .HasColumnName("id_Device");
            entity.Property(e => e.IdUser)
                .HasMaxLength(250)
                .HasColumnName("id_User");
            entity.Property(e => e.Token1)
                .HasMaxLength(1000)
                .HasColumnName("Token");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("User");

            entity.HasIndex(e => e.IdRole, "id_Role");

            entity.Property(e => e.Id)
                .HasMaxLength(250)
                .HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.IdRole)
                .HasColumnType("int(11)")
                .HasColumnName("id_Role");
            entity.Property(e => e.Login).HasMaxLength(250);
            entity.Property(e => e.Password).HasMaxLength(250);

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .HasConstraintName("user_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
