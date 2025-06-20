using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Appointments",
                newName: "DoctorScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorScheduleId",
                table: "Appointments",
                column: "DoctorScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorSchedules_DoctorScheduleId",
                table: "Appointments",
                column: "DoctorScheduleId",
                principalTable: "DoctorSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorSchedules_DoctorScheduleId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorScheduleId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "DoctorScheduleId",
                table: "Appointments",
                newName: "Duration");
        }
    }
}
