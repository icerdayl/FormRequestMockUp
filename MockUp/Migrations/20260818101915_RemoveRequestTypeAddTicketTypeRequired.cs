using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChangeRequest.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequestTypeAddTicketTypeRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert existing RequestType values into valid TicketType values
            // BEFORE removing the old RequestTypes table/column.
            migrationBuilder.Sql(@"
        UPDATE R
        SET R.TicketTypeId =
            CASE RT.RequestTypeName
                WHEN 'New Website' THEN 2
                WHEN 'Enhancement' THEN 3
                WHEN 'Bug Fix' THEN 1
                WHEN 'Maintenance' THEN 4
                ELSE 6
            END
        FROM Requests R
        INNER JOIN RequestTypes RT
            ON R.RequestTypeId = RT.RequestTypeId
        WHERE R.TicketTypeId IS NULL;
    ");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_RequestTypes_RequestTypeId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_TicketTypes_TicketTypeId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestTypeId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestTypeId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "TicketTypeId",
                table: "Requests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_TicketTypes_TicketTypeId",
                table: "Requests",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "TicketTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_TicketTypes_TicketTypeId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "TicketTypeId",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RequestTypes",
                columns: table => new
                {
                    RequestTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTypes", x => x.RequestTypeId);
                });

            migrationBuilder.InsertData(
                table: "RequestTypes",
                columns: new[] { "RequestTypeId", "RequestTypeName" },
                values: new object[,]
                {
                    { 1, "New Website" },
                    { 2, "Enhancement" },
                    { 3, "Bug Fix" },
                    { 4, "Maintenance" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestTypeId",
                table: "Requests",
                column: "RequestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_RequestTypes_RequestTypeId",
                table: "Requests",
                column: "RequestTypeId",
                principalTable: "RequestTypes",
                principalColumn: "RequestTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_TicketTypes_TicketTypeId",
                table: "Requests",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "TicketTypeId");
        }
    }
}
