using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class RemoveDefaultTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShopParseStrategyInfo",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAt",
                table: "ShopParseStrategyInfo",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ShopParseStrategyInfo",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShopInfo",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ShopInfo",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordLinks",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordLinks",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordInShopLink",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordInShopLink",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordInfo",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordInfo",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordArt",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordArt",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShopParseStrategyInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAt",
                table: "ShopParseStrategyInfo",
                nullable: true,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc),
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ShopParseStrategyInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShopInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 142, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ShopInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordLinks",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordLinks",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordInShopLink",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 141, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordInShopLink",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordArt",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordArt",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc),
                oldClrType: typeof(DateTime));
        }
    }
}
