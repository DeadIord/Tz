using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<double>(type: "float", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Salt", "Token", "Username" },
                values: new object[,]
                {
                    { 1, "tPczQr3sG8EJCrwh/k7oQewjcUWwMUwPZAwWcgc/HA9Tb6TECuonXxx8FbHuvzJJNBRlF3+4cGSnzOsRMA6Y9g==", new byte[] { 235, 154, 183, 150, 113, 158, 160, 75, 105, 189, 56, 115, 13, 122, 186, 65, 139, 191, 203, 44, 104, 200, 247, 250, 251, 89, 202, 96, 209, 65, 49, 230 }, "someTokenValue1", "Пользователь1" },
                    { 2, "ZpJHq9bQs4t/QVetz2MQeMxDspKSQqzOfAb51Tn0vjEHWoj47tBvjgyJEPX6AhDP43ZkyYwcmSRN8dzVZ5/ZSg==", new byte[] { 37, 201, 81, 198, 64, 166, 55, 38, 250, 66, 201, 57, 104, 49, 148, 129, 217, 159, 30, 194, 44, 209, 194, 171, 19, 90, 132, 181, 241, 166, 18, 129 }, "someTokenValue2", "Пользователь2" },
                    { 3, "741v76BR4K1bIeslDJEDskP+czADNkPZNyw5L4XcPTZ6AxRoK5N7GXHKVlnxV6gxF0I0/o7L88+0agAczZqTBg==", new byte[] { 99, 237, 207, 89, 70, 21, 68, 97, 174, 138, 231, 87, 162, 61, 14, 224, 152, 41, 115, 55, 177, 91, 161, 88, 197, 101, 244, 244, 115, 110, 90, 191 }, "someTokenValue3", "zz" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "OrderDate", "TotalCost", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 28, 23, 51, 15, 368, DateTimeKind.Local).AddTicks(3285), 134000.0, 1 },
                    { 2, new DateTime(2024, 1, 29, 23, 51, 15, 368, DateTimeKind.Local).AddTicks(3306), 130000.0, 1 },
                    { 3, new DateTime(2024, 1, 30, 23, 51, 15, 368, DateTimeKind.Local).AddTicks(3307), 105000.0, 2 }
                });

            migrationBuilder.InsertData(
                table: "OrderItem",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 1, 2 },
                    { 2, 1, 2, 1 },
                    { 3, 2, 4, 1 },
                    { 4, 2, 5, 1 },
                    { 5, 3, 3, 3 },
                    { 6, 3, 6, 1 },
                    { 7, 3, 7, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
