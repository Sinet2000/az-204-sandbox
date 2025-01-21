using Dexlaris.Azf1HttpTrigger.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dexlaris.Azf1HttpTrigger;

public class PaymentWebhook
{
    /*
     * POST /webhook/payment
       Content-Type: application/json
       {
           "event": "payment_succeeded",
           "transaction_id": "12345",
           "amount": 100.0
       }
       
       POST /webhook/github
       Content-Type: application/json
       {
           "event": "push",
           "repository": "my-repo",
           "commits": [{ "id": "abc123", "message": "Added feature X" }]
       }
       
       POST /webhook/slack
       Content-Type: application/json
       {
           "event": "message_posted",
           "channel": "general",
           "text": "Hello, World!"
       }
     */
    [Function(nameof(PaymentWebhook))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "webhook/payment")]
        HttpRequest req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(PaymentWebhook));
        logger.LogInformation("Payment webhook received.");

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonHelper.Deserialize<dynamic>(requestBody);

            if (data!.event_type != "payment_succeeded") return new OkResult();

            var transactionId = (string)data.transaction_id;
            logger.LogInformation("Payment succeeded for transaction ID: {TransactionID}", transactionId);

            return new OkResult();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing payment webhook");

            return new BadRequestResult();
        }
    }

}