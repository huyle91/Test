using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfertilityTreatment.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePerformanceViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create vw_TreatmentSuccessRates
            migrationBuilder.Sql(@"
                CREATE VIEW vw_TreatmentSuccessRates AS
                SELECT 
                    ts.Id as ServiceId,
                    ts.Name as TreatmentType,
                    COUNT(tc.Id) as TotalCycles,
                    SUM(CASE WHEN tc.Status = 3 THEN 1 ELSE 0 END) as SuccessfulCycles,
                    CASE 
                        WHEN COUNT(tc.Id) = 0 THEN 0
                        ELSE CAST(SUM(CASE WHEN tc.Status = 3 THEN 1 ELSE 0 END) * 100.0 / COUNT(tc.Id) AS DECIMAL(5,2))
                    END as SuccessRate
                FROM TreatmentServices ts
                LEFT JOIN TreatmentPackages tp ON ts.Id = tp.ServiceId  
                LEFT JOIN TreatmentCycles tc ON tp.Id = tc.PackageId
                GROUP BY ts.Id, ts.Name
                HAVING COUNT(tc.Id) > 0
            ");

            // Create vw_RevenueAnalytics
            migrationBuilder.Sql(@"
                CREATE VIEW vw_RevenueAnalytics AS
                SELECT 
                    ts.Id as ServiceId,
                    ts.Name as ServiceName,
                    YEAR(tc.CreatedAt) as Year,
                    MONTH(tc.CreatedAt) as Month,
                    COUNT(tc.Id) as TotalCycles,
                    SUM(tp.Price) as TotalRevenue,
                    CASE 
                        WHEN COUNT(tc.Id) = 0 THEN 0
                        ELSE AVG(tp.Price)
                    END as AvgRevenuePerCycle
                FROM TreatmentServices ts
                INNER JOIN TreatmentPackages tp ON ts.Id = tp.ServiceId
                INNER JOIN TreatmentCycles tc ON tp.Id = tc.PackageId  
                WHERE tc.CreatedAt IS NOT NULL
                GROUP BY ts.Id, ts.Name, YEAR(tc.CreatedAt), MONTH(tc.CreatedAt)
            ");

            // Create vw_DoctorPerformance
            migrationBuilder.Sql(@"
                CREATE VIEW vw_DoctorPerformance AS
                SELECT 
                    d.Id as DoctorId,
                    u.FullName as DoctorName,
                    COUNT(DISTINCT tc.CustomerId) as TotalPatients,
                    COUNT(tc.Id) as TotalCycles,
                    SUM(CASE WHEN tc.Status = 3 THEN 1 ELSE 0 END) as SuccessfulCycles,
                    CASE 
                        WHEN COUNT(tc.Id) = 0 THEN 0
                        ELSE CAST(SUM(CASE WHEN tc.Status = 3 THEN 1 ELSE 0 END) * 100.0 / COUNT(tc.Id) AS DECIMAL(5,2))
                    END as SuccessRate,
                    ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) as AvgRating,
                    COUNT(r.Id) as TotalReviews
                FROM Doctors d
                INNER JOIN Users u ON d.UserId = u.Id
                LEFT JOIN TreatmentCycles tc ON d.Id = tc.DoctorId
                LEFT JOIN Reviews r ON d.Id = r.DoctorId AND r.IsApproved = 1
                GROUP BY d.Id, u.FullName
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_TreatmentSuccessRates");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_RevenueAnalytics");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_DoctorPerformance");

        }
    }
}
