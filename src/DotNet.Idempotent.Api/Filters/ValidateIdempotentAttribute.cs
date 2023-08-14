using DotNet.Idempotent.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNet.Idempotent.Api.Filters;

public class ValidateIdempotentAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var goNext = true;
        if (context.HttpContext.Request.Method != "GET")
        {
            var requestId = context.HttpContext.Request.Headers["X-Idempotency-Key"];

            if (string.IsNullOrEmpty(requestId) || !Guid.TryParse(requestId.ToString(), out Guid key))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult("X-Idempotency-Key is required in header");
                goNext = false;
            }
            else
            {
                var idempotencyService = context.HttpContext.RequestServices.GetService<IIdempotencyService>();
                if (idempotencyService == null) throw new InvalidOperationException();
                var requestIdGuid = Guid.Parse(requestId.ToString());

                var exists = await idempotencyService.RequestExistsAsync(requestIdGuid);

                if (exists)
                {
                    context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult("RequestId sended");
                    goNext = false;
                }
                else
                    await idempotencyService.CreateRequestAsync(requestIdGuid);
            }
        }

        if (goNext) await next();
    }
}