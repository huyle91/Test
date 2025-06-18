using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameNotificationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

        }
    }
}
