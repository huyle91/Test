using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.Enums
{
    public enum ReviewType
    {
        [Description("Review Doctor")]
        reviewDoctor = 0,
        
        [Description("Review Service")]
        reviewService = 1
    }
}
