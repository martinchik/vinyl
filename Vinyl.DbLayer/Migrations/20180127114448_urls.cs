using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class urls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordUrl",
                table: "SearchItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordUrl",
                table: "RecordInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordUrl",
                table: "SearchItem");

            migrationBuilder.DropColumn(
                name: "RecordUrl",
                table: "RecordInfo");
        }
    }
}
