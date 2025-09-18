using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADdJobOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JONo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatchSerial_ContractNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderQty = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Line1Qty = table.Column<int>(type: "int", nullable: false),
                    ProcessOrderQtyLine1 = table.Column<int>(type: "int", nullable: false),
                    Line2Qty = table.Column<int>(type: "int", nullable: false),
                    ProcessOrderQtyLine2 = table.Column<int>(type: "int", nullable: false),
                    ISNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeftQty = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOrders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobOrders");
        }
    }
}
