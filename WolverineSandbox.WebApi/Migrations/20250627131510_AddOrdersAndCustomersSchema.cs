using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WolverineSandbox.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndCustomersSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email" },
                values: new object[,]
                {
                    { new Guid("676ff728-5853-f011-9a57-4074e0d1f019"), "Osborne_Green24@yahoo.com" },
                    { new Guid("686ff728-5853-f011-9a57-4074e0d1f019"), "Arno_Jerde32@hotmail.com" },
                    { new Guid("696ff728-5853-f011-9a57-4074e0d1f019"), "Hayley62@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerId", "OrderDate", "TotalAmount" },
                values: new object[,]
                {
                    { new Guid("7bca9135-5853-f011-9a57-4074e0d1f019"), new Guid("686ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2025, 2, 1, 22, 47, 55, 912, DateTimeKind.Unspecified).AddTicks(8825), new TimeSpan(0, 0, 0, 0, 0)), 604.45m },
                    { new Guid("7cca9135-5853-f011-9a57-4074e0d1f019"), new Guid("696ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2024, 9, 8, 6, 17, 59, 182, DateTimeKind.Unspecified).AddTicks(1994), new TimeSpan(0, 0, 0, 0, 0)), 524.09m },
                    { new Guid("7dca9135-5853-f011-9a57-4074e0d1f019"), new Guid("686ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2024, 8, 4, 13, 35, 14, 491, DateTimeKind.Unspecified).AddTicks(5294), new TimeSpan(0, 0, 0, 0, 0)), 680.11m },
                    { new Guid("7eca9135-5853-f011-9a57-4074e0d1f019"), new Guid("676ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2025, 4, 20, 8, 10, 41, 1, DateTimeKind.Unspecified).AddTicks(3700), new TimeSpan(0, 0, 0, 0, 0)), 515.47m },
                    { new Guid("7fca9135-5853-f011-9a57-4074e0d1f019"), new Guid("696ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2024, 7, 21, 0, 45, 17, 707, DateTimeKind.Unspecified).AddTicks(1993), new TimeSpan(0, 0, 0, 0, 0)), 77.89m },
                    { new Guid("ba1f2742-5853-f011-9a57-4074e0d1f019"), new Guid("686ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2025, 4, 6, 23, 49, 29, 334, DateTimeKind.Unspecified).AddTicks(901), new TimeSpan(0, 0, 0, 0, 0)), 553.36m },
                    { new Guid("bb1f2742-5853-f011-9a57-4074e0d1f019"), new Guid("696ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2025, 2, 10, 5, 34, 22, 219, DateTimeKind.Unspecified).AddTicks(5269), new TimeSpan(0, 0, 0, 0, 0)), 375.37m },
                    { new Guid("bc1f2742-5853-f011-9a57-4074e0d1f019"), new Guid("676ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2025, 1, 19, 17, 42, 10, 836, DateTimeKind.Unspecified).AddTicks(8003), new TimeSpan(0, 0, 0, 0, 0)), 93.10m },
                    { new Guid("bd1f2742-5853-f011-9a57-4074e0d1f019"), new Guid("696ff728-5853-f011-9a57-4074e0d1f019"), new DateTimeOffset(new DateTime(2024, 10, 8, 7, 54, 47, 809, DateTimeKind.Unspecified).AddTicks(1147), new TimeSpan(0, 0, 0, 0, 0)), 857.02m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
