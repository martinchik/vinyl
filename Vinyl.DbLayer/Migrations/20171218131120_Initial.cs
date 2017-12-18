using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Album = table.Column<string>(maxLength: 255, nullable: true),
                    Artist = table.Column<string>(maxLength: 255, nullable: true),
                    Info = table.Column<string>(maxLength: 1000, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Year = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PriceFrom = table.Column<decimal>(nullable: true),
                    PriceTo = table.Column<decimal>(nullable: true),
                    RecordId = table.Column<Guid>(nullable: false),
                    Sell = table.Column<bool>(nullable: false),
                    TextLine1 = table.Column<string>(maxLength: 1000, nullable: false),
                    TextLine2 = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    City = table.Column<string>(maxLength: 255, nullable: true),
                    Country = table.Column<string>(maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 241, DateTimeKind.Utc)),
                    Emails = table.Column<string>(maxLength: 255, nullable: true),
                    Phones = table.Column<string>(maxLength: 255, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 241, DateTimeKind.Utc)),
                    Url = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordArt",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 222, DateTimeKind.Utc)),
                    FullViewUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    PreviewUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 224, DateTimeKind.Utc))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordArt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordArt_RecordInfo",
                        column: x => x.RecordId,
                        principalTable: "RecordInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 238, DateTimeKind.Utc)),
                    Link = table.Column<string>(maxLength: 1000, nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 1000, nullable: true),
                    ToType = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 238, DateTimeKind.Utc))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordLinks_RecordInfo",
                        column: x => x.RecordId,
                        principalTable: "RecordInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopParseStrategyInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClassName = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 244, DateTimeKind.Utc)),
                    DataLimit = table.Column<int>(nullable: false),
                    LastProcessedCount = table.Column<int>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    ProcessedAt = table.Column<DateTime>(nullable: true, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 245, DateTimeKind.Utc)),
                    ShopId = table.Column<Guid>(nullable: false),
                    StartUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UpdatePeriodInHours = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 245, DateTimeKind.Utc))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopParseStrategyInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopParseStrategyInfo_ShopInfo",
                        column: x => x.ShopId,
                        principalTable: "ShopInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordInShopLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Barcode = table.Column<string>(maxLength: 255, nullable: true),
                    CountInPack = table.Column<string>(maxLength: 255, nullable: true),
                    Country = table.Column<string>(maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 233, DateTimeKind.Utc)),
                    Currency = table.Column<string>(maxLength: 255, nullable: true),
                    Label = table.Column<string>(maxLength: 255, nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    RecordId = table.Column<Guid>(nullable: false),
                    ShopId = table.Column<Guid>(nullable: false),
                    ShopInfo = table.Column<string>(maxLength: 1000, nullable: true),
                    ShopUrl = table.Column<string>(maxLength: 1000, nullable: true),
                    State = table.Column<string>(maxLength: 255, nullable: true),
                    StrategyId = table.Column<Guid>(nullable: false),
                    Style = table.Column<string>(maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 12, 18, 13, 11, 20, 234, DateTimeKind.Utc)),
                    ViewType = table.Column<string>(maxLength: 255, nullable: true),
                    YearRecorded = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordInShopLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordInShopLink_RecordInfo",
                        column: x => x.RecordId,
                        principalTable: "RecordInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordInShopLink_ShopInfo",
                        column: x => x.ShopId,
                        principalTable: "ShopInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordInShopLink_ShopParseStrategyInfo",
                        column: x => x.StrategyId,
                        principalTable: "ShopParseStrategyInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordArt_RecordId",
                table: "RecordArt",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordInShopLink_RecordId",
                table: "RecordInShopLink",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordInShopLink_ShopId",
                table: "RecordInShopLink",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordInShopLink_StrategyId",
                table: "RecordInShopLink",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordLinks_RecordId",
                table: "RecordLinks",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopParseStrategyInfo_ShopId",
                table: "ShopParseStrategyInfo",
                column: "ShopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordArt");

            migrationBuilder.DropTable(
                name: "RecordInShopLink");

            migrationBuilder.DropTable(
                name: "RecordLinks");

            migrationBuilder.DropTable(
                name: "SearchItem");

            migrationBuilder.DropTable(
                name: "ShopParseStrategyInfo");

            migrationBuilder.DropTable(
                name: "RecordInfo");

            migrationBuilder.DropTable(
                name: "ShopInfo");
        }
    }
}
