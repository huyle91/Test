using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TreatmentCycles_TreatmentCycleId",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentPackageId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TreatmentPackageId",
                table: "Payments",
                column: "TreatmentPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TreatmentCycles_TreatmentCycleId",
                table: "Payments",
                column: "TreatmentCycleId",
                principalTable: "TreatmentCycles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TreatmentPackages_TreatmentPackageId",
                table: "Payments",
                column: "TreatmentPackageId",
                principalTable: "TreatmentPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TreatmentCycles_TreatmentCycleId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TreatmentPackages_TreatmentPackageId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TreatmentPackageId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TreatmentPackageId",
                table: "Payments");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TreatmentCycles_TreatmentCycleId",
                table: "Payments",
                column: "TreatmentCycleId",
                principalTable: "TreatmentCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
