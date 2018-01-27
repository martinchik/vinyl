﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using Vinyl.DbLayer;

namespace Vinyl.DbLayer.Migrations
{
    [DbContext(typeof(VinylShopContext))]
    [Migration("20180127114448_urls")]
    partial class urls
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordArt", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("FullViewUrl")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<string>("PreviewUrl")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<Guid>("RecordId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("RecordId");

                    b.ToTable("RecordArt");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordInfo", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("Album")
                        .HasMaxLength(255);

                    b.Property<string>("Artist")
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Info")
                        .HasMaxLength(2000);

                    b.Property<string>("RecordUrl");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int?>("Year");

                    b.HasKey("Id");

                    b.ToTable("RecordInfo");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordInShopLink", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("Barcode")
                        .HasMaxLength(255);

                    b.Property<string>("CountInPack")
                        .HasMaxLength(255);

                    b.Property<string>("Country")
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Currency")
                        .HasMaxLength(255);

                    b.Property<string>("Label")
                        .HasMaxLength(1000);

                    b.Property<decimal?>("Price");

                    b.Property<decimal?>("PriceBy");

                    b.Property<Guid>("RecordId");

                    b.Property<Guid>("ShopId");

                    b.Property<string>("ShopImageUrl")
                        .HasMaxLength(1000);

                    b.Property<string>("ShopInfo")
                        .HasMaxLength(2000);

                    b.Property<string>("ShopRecordTitle")
                        .HasMaxLength(1000);

                    b.Property<string>("ShopUrl")
                        .HasMaxLength(1000);

                    b.Property<string>("State")
                        .HasMaxLength(255);

                    b.Property<int>("Status");

                    b.Property<Guid>("StrategyId");

                    b.Property<string>("Style")
                        .HasMaxLength(255);

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("ViewType")
                        .HasMaxLength(255);

                    b.Property<string>("YearRecorded")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("RecordId");

                    b.HasIndex("ShopId");

                    b.HasIndex("StrategyId");

                    b.ToTable("RecordInShopLink");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordLinks", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<Guid>("RecordId");

                    b.Property<string>("Text")
                        .HasMaxLength(1000);

                    b.Property<int>("ToType");

                    b.Property<string>("Tracks");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Videos");

                    b.HasKey("Id");

                    b.HasIndex("RecordId");

                    b.ToTable("RecordLinks");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.SearchItem", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("CountryCode")
                        .HasMaxLength(255);

                    b.Property<decimal?>("PriceFrom");

                    b.Property<decimal?>("PriceTo");

                    b.Property<Guid>("RecordId");

                    b.Property<string>("RecordUrl");

                    b.Property<bool>("Sell");

                    b.Property<int>("ShopsCount");

                    b.Property<string>("States");

                    b.Property<string>("TextLine1")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("TextLine2")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("SearchItem");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.ShopInfo", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("City")
                        .HasMaxLength(255);

                    b.Property<string>("CountryCode")
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Emails")
                        .HasMaxLength(255);

                    b.Property<string>("Phones")
                        .HasMaxLength(255);

                    b.Property<int>("ShopType");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("Url")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("ShopInfo");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.ShopParseStrategyInfo", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("DataLimit");

                    b.Property<string>("DefaultCurrency");

                    b.Property<int?>("LastProcessedCount");

                    b.Property<string>("Parameters");

                    b.Property<DateTime?>("ProcessedAt");

                    b.Property<Guid>("ShopId");

                    b.Property<string>("StartUrl")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<int>("Status");

                    b.Property<int>("UpdatePeriodInHours");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("ShopParseStrategyInfo");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordArt", b =>
                {
                    b.HasOne("Vinyl.DbLayer.Models.RecordInfo", "Record")
                        .WithMany("RecordArt")
                        .HasForeignKey("RecordId")
                        .HasConstraintName("FK_RecordArt_RecordInfo");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordInShopLink", b =>
                {
                    b.HasOne("Vinyl.DbLayer.Models.RecordInfo", "Record")
                        .WithMany("RecordInShopLink")
                        .HasForeignKey("RecordId")
                        .HasConstraintName("FK_RecordInShopLink_RecordInfo");

                    b.HasOne("Vinyl.DbLayer.Models.ShopInfo", "Shop")
                        .WithMany("RecordInShopLink")
                        .HasForeignKey("ShopId")
                        .HasConstraintName("FK_RecordInShopLink_ShopInfo");

                    b.HasOne("Vinyl.DbLayer.Models.ShopParseStrategyInfo", "Strategy")
                        .WithMany("RecordInShopLink")
                        .HasForeignKey("StrategyId")
                        .HasConstraintName("FK_RecordInShopLink_ShopParseStrategyInfo");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.RecordLinks", b =>
                {
                    b.HasOne("Vinyl.DbLayer.Models.RecordInfo", "Record")
                        .WithMany("RecordLinks")
                        .HasForeignKey("RecordId")
                        .HasConstraintName("FK_RecordLinks_RecordInfo");
                });

            modelBuilder.Entity("Vinyl.DbLayer.Models.ShopParseStrategyInfo", b =>
                {
                    b.HasOne("Vinyl.DbLayer.Models.ShopInfo", "Shop")
                        .WithMany("ShopParseStrategyInfo")
                        .HasForeignKey("ShopId")
                        .HasConstraintName("FK_ShopParseStrategyInfo_ShopInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
