using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class ChangeTextLine2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopsCount",
                table: "SearchItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "States",
                table: "SearchItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopsCount",
                table: "SearchItem");

            migrationBuilder.DropColumn(
                name: "States",
                table: "SearchItem");
        }
    }
}
