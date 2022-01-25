using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Books.Data.Migrations
{
    public partial class AddStatusOrdersToOrderHeaderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderDate",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "OrderHeader");
        }
    }
}
