using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class PrescriptionRepository : BaseRepository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
