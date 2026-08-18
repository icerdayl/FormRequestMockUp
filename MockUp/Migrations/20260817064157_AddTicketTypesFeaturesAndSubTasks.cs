using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChangeRequest.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketTypesFeaturesAndSubTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketTypeId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    FeatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.FeatureId);
                    table.ForeignKey(
                        name: "FK_Features_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    TicketTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.TicketTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SubTasks",
                columns: table => new
                {
                    SubTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedManDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualManDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultAttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTasks", x => x.SubTaskId);
                    table.ForeignKey(
                        name: "FK_SubTasks_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "FeatureId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubTasks_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TicketTypes",
                columns: new[] { "TicketTypeId", "TicketTypeName" },
                values: new object[,]
                {
                    { 1, "Bug" },
                    { 2, "Feature Request" },
                    { 3, "Enhancement" },
                    { 4, "Maintenance" },
                    { 5, "Technical Support" },
                    { 6, "Change Request" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TicketTypeId",
                table: "Requests",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_RequestId",
                table: "Features",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_FeatureId",
                table: "SubTasks",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_RequestId",
                table: "SubTasks",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_TicketTypes_TicketTypeId",
                table: "Requests",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "TicketTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_TicketTypes_TicketTypeId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "SubTasks");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Requests_TicketTypeId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TicketTypeId",
                table: "Requests");
        }
    }
}
