using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public List<MonthlyRevenueDto> MonthlyBreakdown { get; set; }
        public List<ServiceRevenueDto> ServiceBreakdown { get; set; }
        public DateTime ReportDate { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
    }

    public class ServiceRevenueDto
    {
        public string ServiceName { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
        public double Percentage { get; set; }
    }
}
