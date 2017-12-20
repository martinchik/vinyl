using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Migrations
{
    public partial class FirstData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.AlterColumn<int>(
                name: "DataLimit",
                table: "ShopParseStrategyInfo",
                nullable: true,
                oldClrType: typeof(int));            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.AlterColumn<int>(
                name: "DataLimit",
                table: "ShopParseStrategyInfo",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
