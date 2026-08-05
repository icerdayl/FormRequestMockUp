using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChangeRequest.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HelpDeskRemarks",
                table: "Requests");

            migrationBuilder.CreateTable(
                name: "RequestApprovals",
                columns: table => new
                {
                    ApprovalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestApprovals", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_RequestApprovals_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovals_RequestId",
                table: "RequestApprovals",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestApprovals");

            migrationBuilder.AddColumn<string>(
                name: "HelpDeskRemarks",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
