using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class updaterecordinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RecordInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RecordInfo",
                nullable: false,
                defaultValue: new DateTime(2017, 12, 21, 15, 17, 11, 140, DateTimeKind.Utc));            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RecordInfo");

            migrationBuilder.DropColumn(
               name: "UpdatedAt",
               table: "RecordInfo");
        }
    }
}
