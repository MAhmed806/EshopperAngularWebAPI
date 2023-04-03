using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopperAngular.Migrations
{
    public partial class hehelloworld : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefundStatus",
                table: "ChargeAgainstOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "ChargeAgainstOrder");
        }
    }
}
