using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class ShopExtendedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShopImageUrl",
                table: "RecordInShopLink",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopRecordTitle",
                table: "RecordInShopLink",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopImageUrl",
                table: "RecordInShopLink");

            migrationBuilder.DropColumn(
                name: "ShopRecordTitle",
                table: "RecordInShopLink");
        }
    }
}
