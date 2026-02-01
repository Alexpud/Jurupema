using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Function;

public class JurupemaSecondHttpTrigger
{
    private readonly ILogger<JurupemaSecondHttpTrigger> _logger;

    public JurupemaSecondHttpTrigger(ILogger<JurupemaSecondHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("JurupemaSecondHttpTrigger")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
