using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class UpdateAppointmentDto : CreateAppointmentDto
    {
        public AppointmentStatus Status { get; set; }
    }
}
