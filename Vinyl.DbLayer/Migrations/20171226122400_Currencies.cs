using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class Currencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "ShopParseStrategyInfo",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBy",
                table: "RecordInShopLink",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "ShopParseStrategyInfo");

            migrationBuilder.DropColumn(
                name: "PriceBy",
                table: "RecordInShopLink");
        }
    }
}
