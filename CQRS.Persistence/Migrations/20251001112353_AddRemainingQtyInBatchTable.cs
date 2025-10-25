using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingQtyInBatchTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingQty",
                table: "BatchSerials",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingQty",
                table: "BatchSerials");
        }
    }
}
