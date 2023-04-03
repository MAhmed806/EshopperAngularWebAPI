using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopperAngular.Migrations
{
    public partial class testing123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "ChargeAgainstOrder",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeAgainstOrder_OrderId",
                table: "ChargeAgainstOrder",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChargeAgainstOrder_Order_OrderId",
                table: "ChargeAgainstOrder",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargeAgainstOrder_Order_OrderId",
                table: "ChargeAgainstOrder");

            migrationBuilder.DropIndex(
                name: "IX_ChargeAgainstOrder_OrderId",
                table: "ChargeAgainstOrder");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "ChargeAgainstOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
