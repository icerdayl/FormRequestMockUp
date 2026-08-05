using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChangeRequest.Migrations
{
    /// <inheritdoc />
    public partial class AddHelpDeskRemarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HelpDeskRemarks",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HelpDeskRemarks",
                table: "Requests");
        }
    }
}
