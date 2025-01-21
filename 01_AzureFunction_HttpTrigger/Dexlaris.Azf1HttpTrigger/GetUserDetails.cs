using Dexlaris.Azf1HttpTrigger.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dexlaris.Azf1HttpTrigger;

public class GetUserDetails // (ILogger<GetUserDetails> logger)
{
    [Function(nameof(GetUserDetails))]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id:int}")]
        HttpRequest req,
        string id,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(GetUserDetails));
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var user = new User(id, "Johny Depp", "Actor");

        return new OkObjectResult(user);
    }

}