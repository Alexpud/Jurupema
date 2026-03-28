using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jurupema.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndOrderLineSnapshotPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Line items cannot be mapped to new Order headers without a data script; clear before reshape and OrderId FK.
            migrationBuilder.Sql("DELETE FROM [ProductOrder];");

            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "PaymentLinkExpiration",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "PaymentLinkQrCode",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "PaymentLinkQrCodeExpiration",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductImage");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "ProductOrder",
                newName: "Price");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductOrder",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "ProductOrder",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Pending"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentLink = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PaymentLinkExpiration = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PaymentLinkQrCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PaymentLinkQrCodeExpiration = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrder_OrderId",
                table: "ProductOrder",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrder_Order_OrderId",
                table: "ProductOrder",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrder_Order_OrderId",
                table: "ProductOrder");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrder_OrderId",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductOrder");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProductOrder",
                newName: "TotalPrice");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductOrder",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "ProductOrder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkExpiration",
                table: "ProductOrder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkQrCode",
                table: "ProductOrder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkQrCodeExpiration",
                table: "ProductOrder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "ProductOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "ProductOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductOrder",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProductOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductImage",
                type: "datetime2",
                nullable: true);
        }
    }
}
