using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.Constants
{
    public static  class TreatmentServiceMessages
    {
        public const string UnknowError = "An error unknow. ";
        public const string InvalidInput = "Invalid input.";
        public const string NotFound = "Treatment service not found.";
        public const string CreatedSuccessfully = "Treatment service created successfully.";
        public const string UpdatedSuccessfully = "Treatment service updated successfully.";
        public const string DeletedSuccessfully = "Treatment service deleted successfully.";
        public const string InternalError = "An error occurred while processing the treatment service.";
        public const string GetSuccess = "Fetched treatment service successfully.";
        public const string GetAllSuccess = "Fetched all treatment service successfully.";
        public const string NameHasAlreadyExist = "Name has already existed";
    }
}
