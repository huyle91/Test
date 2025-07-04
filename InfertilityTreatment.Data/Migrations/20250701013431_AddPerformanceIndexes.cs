using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Create performance indexes for Week 6 analytics and payment features
            migrationBuilder.Sql(@"
                -- User performance indexes
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Users_Role' AND object_id = OBJECT_ID('Users'))
                    CREATE INDEX IX_Users_Role ON Users(Role);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Users_Email' AND object_id = OBJECT_ID('Users'))
                    CREATE INDEX IX_Users_Email ON Users(Email);

                -- Treatment cycle indexes for analytics
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TreatmentCycles_Status_StartDate' AND object_id = OBJECT_ID('TreatmentCycles'))
                    CREATE INDEX IX_TreatmentCycles_Status_StartDate ON TreatmentCycles(Status, StartDate);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TreatmentCycles_CustomerId_Status' AND object_id = OBJECT_ID('TreatmentCycles'))
                    CREATE INDEX IX_TreatmentCycles_CustomerId_Status ON TreatmentCycles(CustomerId, Status);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TreatmentCycles_DoctorId_Status' AND object_id = OBJECT_ID('TreatmentCycles'))
                    CREATE INDEX IX_TreatmentCycles_DoctorId_Status ON TreatmentCycles(DoctorId, Status);

                -- Appointment indexes for booking
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Appointments_DoctorId_ScheduledDateTime' AND object_id = OBJECT_ID('Appointments'))
                    CREATE INDEX IX_Appointments_DoctorId_ScheduledDateTime ON Appointments(DoctorId, ScheduledDateTime);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Appointments_Status_ScheduledDateTime' AND object_id = OBJECT_ID('Appointments'))
                    CREATE INDEX IX_Appointments_Status_ScheduledDateTime ON Appointments(Status, ScheduledDateTime);

                -- Test result indexes for analytics
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TestResults_CycleId_TestDate' AND object_id = OBJECT_ID('TestResults'))
                    CREATE INDEX IX_TestResults_CycleId_TestDate ON TestResults(CycleId, TestDate);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TestResults_TestType_Status' AND object_id = OBJECT_ID('TestResults'))
                    CREATE INDEX IX_TestResults_TestType_Status ON TestResults(TestType, Status);

                -- Review indexes for doctor statistics
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Reviews_DoctorId_IsApproved' AND object_id = OBJECT_ID('Reviews'))
                    CREATE INDEX IX_Reviews_DoctorId_IsApproved ON Reviews(DoctorId, IsApproved);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Reviews_Rating_CreatedAt' AND object_id = OBJECT_ID('Reviews'))
                    CREATE INDEX IX_Reviews_Rating_CreatedAt ON Reviews(Rating, CreatedAt);

                -- Notification indexes
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Notifications_UserId_IsRead' AND object_id = OBJECT_ID('Notifications'))
                    CREATE INDEX IX_Notifications_UserId_IsRead ON Notifications(UserId, IsRead);
                    
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Notifications_Type_ScheduledAt' AND object_id = OBJECT_ID('Notifications'))
                    CREATE INDEX IX_Notifications_Type_ScheduledAt ON Notifications(Type, ScheduledAt);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop performance indexes if migration is rolled back
            migrationBuilder.Sql(@"
                -- Drop notification indexes
                DROP INDEX IF EXISTS IX_Notifications_Type_ScheduledAt;
                DROP INDEX IF EXISTS IX_Notifications_UserId_IsRead;

                -- Drop review indexes
                DROP INDEX IF EXISTS IX_Reviews_Rating_CreatedAt;
                DROP INDEX IF EXISTS IX_Reviews_DoctorId_IsApproved;

                -- Drop test result indexes
                DROP INDEX IF EXISTS IX_TestResults_TestType_Status;
                DROP INDEX IF EXISTS IX_TestResults_CycleId_TestDate;

                -- Drop appointment indexes
                DROP INDEX IF EXISTS IX_Appointments_Status_ScheduledDateTime;
                DROP INDEX IF EXISTS IX_Appointments_DoctorId_ScheduledDateTime;

                -- Drop treatment cycle indexes
                DROP INDEX IF EXISTS IX_TreatmentCycles_DoctorId_Status;
                DROP INDEX IF EXISTS IX_TreatmentCycles_CustomerId_Status;
                DROP INDEX IF EXISTS IX_TreatmentCycles_Status_StartDate;

                -- Drop user indexes
                DROP INDEX IF EXISTS IX_Users_Email;
                DROP INDEX IF EXISTS IX_Users_Role;
            ");
        }
    }
}
