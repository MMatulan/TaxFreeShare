using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxFreeShareAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSellerIdToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderDate", "SellerId" },
                values: new object[] { new DateTime(2025, 5, 3, 17, 38, 41, 380, DateTimeKind.Utc).AddTicks(4032), null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 3, 17, 38, 41, 380, DateTimeKind.Utc).AddTicks(3872));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 3, 17, 38, 41, 380, DateTimeKind.Utc).AddTicks(3878));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 3, 17, 38, 41, 380, DateTimeKind.Utc).AddTicks(4013));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2025, 3, 17, 8, 54, 15, 446, DateTimeKind.Utc).AddTicks(9456));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 17, 8, 54, 15, 446, DateTimeKind.Utc).AddTicks(9307));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 17, 8, 54, 15, 446, DateTimeKind.Utc).AddTicks(9313));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 17, 8, 54, 15, 446, DateTimeKind.Utc).AddTicks(9439));
        }
    }
}
