using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChangeRequest.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "Requests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "RequestAssignments",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAssignments", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_RequestAssignments_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignments_RequestId",
                table: "RequestAssignments",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
