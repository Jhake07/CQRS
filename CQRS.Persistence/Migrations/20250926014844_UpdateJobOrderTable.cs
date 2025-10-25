using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftQty",
                table: "JobOrders");

            migrationBuilder.DropColumn(
                name: "Line1Qty",
                table: "JobOrders");

            migrationBuilder.DropColumn(
                name: "Line2Qty",
                table: "JobOrders");

            migrationBuilder.RenameColumn(
                name: "JONo",
                table: "JobOrders",
                newName: "JoNo");

            migrationBuilder.RenameColumn(
                name: "ProcessOrderQtyLine2",
                table: "JobOrders",
                newName: "ProcessOrder");

            migrationBuilder.RenameColumn(
                name: "ProcessOrderQtyLine1",
                table: "JobOrders",
                newName: "Line");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JoNo",
                table: "JobOrders",
                newName: "JONo");

            migrationBuilder.RenameColumn(
                name: "ProcessOrder",
                table: "JobOrders",
                newName: "ProcessOrderQtyLine2");

            migrationBuilder.RenameColumn(
                name: "Line",
                table: "JobOrders",
                newName: "ProcessOrderQtyLine1");

            migrationBuilder.AddColumn<int>(
                name: "LeftQty",
                table: "JobOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Line1Qty",
                table: "JobOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Line2Qty",
                table: "JobOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
