using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.Constants
{
    public static class TreatmentPackageMessages
    {
        public const string UnknowError = "An error unknow. ";
        public const string NotFound = "Treatment package not found.";
        public const string DeletedSuccess = "Treatment package deleted successfully.";
        public const string DeleteError = "An error occurred while deleting the treatment package.";
        public const string ServiceIdNotFound = "Service Id not Found";

        public const string GetSuccess = "Fetched treatment package successfully.";
        public const string GetAllSuccess = "Fetched all treatment packages successfully.";
        public const string CreateSuccess = "Treatment package created successfully.";
        public const string UpdateSuccess = "Treatment package updated successfully.";
        public const string InvalidInput = "Invalid input.";
        public const string NameHasAlreadyExist = "Name has already existed";
    }
}
