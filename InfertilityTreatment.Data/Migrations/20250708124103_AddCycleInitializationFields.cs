using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCycleInitializationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartDate",
                table: "TreatmentPhases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartDate",
                table: "TreatmentPhases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartDate",
                table: "TreatmentCycles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorNotes",
                table: "TreatmentCycles",
                type: "nvarchar(1000)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedCompletionDate",
                table: "TreatmentCycles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialInstructions",
                table: "TreatmentCycles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentPlan",
                table: "TreatmentCycles",
                type: "nvarchar(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualStartDate",
                table: "TreatmentPhases");

            migrationBuilder.DropColumn(
                name: "ScheduledStartDate",
                table: "TreatmentPhases");

            migrationBuilder.DropColumn(
                name: "ActualStartDate",
                table: "TreatmentCycles");

            migrationBuilder.DropColumn(
                name: "DoctorNotes",
                table: "TreatmentCycles");

            migrationBuilder.DropColumn(
                name: "EstimatedCompletionDate",
                table: "TreatmentCycles");

            migrationBuilder.DropColumn(
                name: "SpecialInstructions",
                table: "TreatmentCycles");

            migrationBuilder.DropColumn(
                name: "TreatmentPlan",
                table: "TreatmentCycles");
        }
    }
}
