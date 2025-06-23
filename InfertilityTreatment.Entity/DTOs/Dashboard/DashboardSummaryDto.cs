using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalAppointments { get; set; }
        public int ActiveCycles { get; set; }
        public int NewNotifications { get; set; }
    }
}
