using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameShoppingMvcUI.Migrations
{
    /// <inheritdoc />
    public partial class updatemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Genre",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ShoppingCart",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ShoppingCart");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Genre",
                newName: "id");
        }
    }
}
