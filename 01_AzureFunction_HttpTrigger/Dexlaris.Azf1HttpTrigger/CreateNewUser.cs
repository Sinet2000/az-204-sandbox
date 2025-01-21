using Dexlaris.Azf1HttpTrigger.Models;
using Dexlaris.Azf1HttpTrigger.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dexlaris.Azf1HttpTrigger;

public static class CreateNewUser
{
    [Function("CreateNewUser")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequest req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(CreateNewUser));
        logger.LogInformation("Creating a new user ...");

        try
        {
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newUser = JsonHelper.Deserialize<User>(reqBody);

            var response = new { Message = "User created successfully", Data = newUser };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a new user.");

            return new BadRequestResult();
        }
    }

}