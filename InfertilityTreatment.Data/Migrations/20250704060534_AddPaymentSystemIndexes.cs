using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentSystemIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add specific indexes required by Issue #68 - with IF NOT EXISTS checks
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payments_CustomerId' AND object_id = OBJECT_ID('Payments'))
                    CREATE INDEX IX_Payments_CustomerId ON Payments(CustomerId)
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payments_Status' AND object_id = OBJECT_ID('Payments'))
                    CREATE INDEX IX_Payments_Status ON Payments(Status)
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TreatmentCycles_Status_StartDate' AND object_id = OBJECT_ID('TreatmentCycles'))
                    CREATE INDEX IX_TreatmentCycles_Status_StartDate ON TreatmentCycles(Status, StartDate)
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_DoctorId_ScheduledDateTime' AND object_id = OBJECT_ID('Appointments'))
                    CREATE INDEX IX_Appointments_DoctorId_ScheduledDateTime ON Appointments(DoctorId, ScheduledDateTime)
            ");
            
            // Additional performance indexes for Payment system
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_CustomerId' AND object_id = OBJECT_ID('Invoices'))
                    CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId)
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_Status' AND object_id = OBJECT_ID('Invoices'))
                    CREATE INDEX IX_Invoices_Status ON Invoices(Status)
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_IssueDate' AND object_id = OBJECT_ID('Invoices'))
                    CREATE INDEX IX_Invoices_IssueDate ON Invoices(IssueDate)
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InvoiceItems_InvoiceId' AND object_id = OBJECT_ID('InvoiceItems'))
                    CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes in reverse order
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_InvoiceItems_InvoiceId");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Invoices_IssueDate");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Invoices_Status");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Invoices_CustomerId");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Appointments_DoctorId_ScheduledDateTime");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_TreatmentCycles_Status_StartDate");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Payments_Status");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Payments_CustomerId");
        }
    }
}
