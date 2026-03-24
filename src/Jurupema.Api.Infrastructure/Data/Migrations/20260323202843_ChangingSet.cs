using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jurupema.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangingSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductImage",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductImage");
        }
    }
}
