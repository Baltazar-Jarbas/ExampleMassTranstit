using ExampleMassTransit.Client.Api.Infrastructure.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExampleMassTransit.Client.Api.Infrastructure.CustomAttributes
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new CustomProblemDetailsFactory().CreateValidationProblemDetails(context.HttpContext, context.ModelState));
            }
        }
    }
}
