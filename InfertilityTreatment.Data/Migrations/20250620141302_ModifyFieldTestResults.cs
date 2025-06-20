using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFieldTestResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "TestResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_AppointmentId",
                table: "TestResults",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Appointments_AppointmentId",
                table: "TestResults",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Appointments_AppointmentId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_AppointmentId",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "TestResults");
        }
    }
}
