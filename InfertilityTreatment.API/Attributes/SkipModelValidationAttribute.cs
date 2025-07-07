using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfertilityTreatment.API.Attributes
{
    /// <summary>
    /// Attribute to skip automatic model validation for specific endpoints
    /// Useful for external callbacks where we can't control the data format
    /// </summary>
    public class SkipModelValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Clear model state to bypass validation
            context.ModelState.Clear();
            base.OnActionExecuting(context);
        }
    }
}
