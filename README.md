# Dotnet Idempotent

This project was developed to test idempotency in a dotnet application and using redis to store the keys used.

## Resources used

To develop this project, was used:

- DotNet 7
- Redis

## Whats in idempotency?

Idempotency is a concept often used in software development and distributed systems to ensure that a specific action can be safely repeated multiple times without causing unintended side effects. An operation is considered idempotent if performing it multiple times produces the same result as performing it once. In other words, subsequent identical operations should have no additional effect beyond the first one.

This concept is particularly important in scenarios where network failures, timeouts, or other issues might cause the same request to be sent multiple times. If the operation is idempotent, even if the request is duplicated, the system's state remains consistent and the result remains the same.

In the context of developing APIs or web services, ensuring idempotency is crucial to prevent unintended or duplicate actions. For example, consider an API endpoint that processes payments. If the client sends a payment request and it times out, the client might retry the request. If the payment processing operation is idempotent, it won't result in double-charging the user's account, even if the request was duplicated.

Implementing idempotency often involves generating a unique identifier for each request and associating it with the action being performed. The server can then use this identifier to track whether the action has already been performed and respond accordingly. This approach ensures that even if a request is sent multiple times, the server knows to only process it once.

## HTTP Methods description

| **HTTP Method** | **Idempotent** | **Safe** | **Description**                                                                                                                                                                                      |
| --------------- | -------------- | -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| GET             | Yes            | Yes      | Safe HTTP methods do not modify resources. Thus, multiple calls with this method will always return the same response.                                                                               |
| OPTIONS         | Yes            | Yes      | Same as the previous HTTP method.                                                                                                                                                                    |
| HEAD            | Yes            | Yes      | Same as the previous HTTP method.                                                                                                                                                                    |
| PUT             | Yes            | No       | The PUT HTTP method is idempotent because calling this HTTP method multiple times (with the same request data) will update the same resource and not change the outcome.                             |
| DELETE          | Yes            | No       | The DELETE HTTP method is idempotent because calling this HTTP method multiple times will only delete the resource once. Thus, numerous calls of the DELETE HTTP method will not change the outcome. |
| POST            | **No**         | No       | Calling the POST method multiple times can have different results and will create multiple resources. For that reason, the POST method is **not** idempotent.                                        |
| PATCH           | **No**         | No       | The PATCH method can be idempotent depending on the implementation, but it isn’t required to be. For that reason, the PATCH method is **not** idempotent.                                            |

## Test

To run this project you need docker installed on your machine, see the docker documentation [here](https://www.docker.com/).

Having all the resources installed, run the command in a terminal from the root folder of the project and wait some seconds to build project image and download the resources:
`docker-compose up -d`

In terminal show this:

```console
 ✔ Network dotnet-idempotent_default    Created          0.8s
 ✔ Container dotnet-idempotent-redis-1  Started          1.4s
 ✔ Container idempotent_app             Started          2.4s
```

After this, access the link below:

- Swagger project [click here](http://localhost:5000/swagger)

### Stop Application

To stop, run: `docker-compose down`

## How implement

To implement idempotency there are several approaches, however for this case, an attribute was created to be able to give more flexibility to the `endpoints` that want to add this feature.

```c#
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
```

In controller, apply:

```c#
[HttpPost]
[ValidateIdempotent]
public IActionResult Create()
{
    return Ok("Request OK");
}
```
