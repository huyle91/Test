using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFieldAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Appointments_AppointmentId",
                table: "TestResults");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Appointments_AppointmentId",
                table: "TestResults",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Appointments_AppointmentId",
                table: "TestResults");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Appointments_AppointmentId",
                table: "TestResults",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
