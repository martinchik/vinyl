using System;
using Microsoft.EntityFrameworkCore;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer
{
    public partial class VinylShopContext : DbContext
    {
        public virtual DbSet<RecordArt> RecordArt { get; set; }
        public virtual DbSet<RecordInfo> RecordInfo { get; set; }
        public virtual DbSet<RecordInShopLink> RecordInShopLink { get; set; }
        public virtual DbSet<RecordLinks> RecordLinks { get; set; }
        public virtual DbSet<ShopInfo> ShopInfo { get; set; }
        public virtual DbSet<ShopParseStrategyInfo> ShopParseStrategyInfo { get; set; }
        public virtual DbSet<SearchItem> SearchItem { get; set; }

        public VinylShopContext(DbContextOptions<VinylShopContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecordArt>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.Property(e => e.FullViewUrl)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.PreviewUrl)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.RecordArt)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordArt_RecordInfo");
            });

            modelBuilder.Entity<RecordInfo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Album).HasMaxLength(255);

                entity.Property(e => e.Artist).HasMaxLength(255);

                entity.Property(e => e.Info).HasMaxLength(2000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            modelBuilder.Entity<RecordInShopLink>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Barcode).HasMaxLength(255);

                entity.Property(e => e.CountInPack).HasMaxLength(255);

                entity.Property(e => e.Country).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.Property(e => e.Currency).HasMaxLength(255);

                entity.Property(e => e.Label).HasMaxLength(1000);                

                entity.Property(e => e.ShopInfo).HasMaxLength(2000);

                entity.Property(e => e.ShopUrl).HasMaxLength(1000);

                entity.Property(e => e.State).HasMaxLength(255);

                entity.Property(e => e.Style).HasMaxLength(255);

                entity.Property(e => e.ViewType).HasMaxLength(255);

                entity.Property(e => e.YearRecorded).HasMaxLength(255);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.RecordInShopLink)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordInShopLink_RecordInfo");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.RecordInShopLink)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordInShopLink_ShopInfo");

                entity.HasOne(d => d.Strategy)
                    .WithMany(p => p.RecordInShopLink)
                    .HasForeignKey(d => d.StrategyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordInShopLink_ShopParseStrategyInfo");
            });

            modelBuilder.Entity<RecordLinks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt);

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Text).HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.RecordLinks)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordLinks_RecordInfo");
            });

            modelBuilder.Entity<ShopInfo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.CountryCode).HasMaxLength(255);

                entity.Property(e => e.Emails).HasMaxLength(255);

                entity.Property(e => e.Phones).HasMaxLength(255);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<ShopParseStrategyInfo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ClassName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.Property(e => e.StartUrl)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopParseStrategyInfo)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShopParseStrategyInfo_ShopInfo");
            });

            modelBuilder.Entity<SearchItem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();                

                entity.Property(e => e.TextLine1)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.TextLine2).HasMaxLength(1000);

                entity.Property(e => e.CountryCode).HasMaxLength(255);
            });   
        }
    }
}
