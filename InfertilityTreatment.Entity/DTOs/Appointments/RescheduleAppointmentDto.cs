using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class RescheduleAppointmentDto
    {
        public int DoctorScheduleId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
    }
}
