using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Function;

public class JurupemaHttpTrigger
{
    private readonly ILogger<JurupemaHttpTrigger> _logger;

    public JurupemaHttpTrigger(ILogger<JurupemaHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("JurupemaHttpTrigger")]
    public JurupemaHttpTriggerOutput Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new JurupemaHttpTriggerOutput
        {
            Message = new OkObjectResult("Welcome to Azure Functions!"),
            OutputMessageToQueue = "Welcome to Azure Functions!"
        };
    }
}

public class JurupemaHttpTriggerOutput
{
    [HttpResult]
    public IActionResult Message { get; set; }

    [QueueOutput("output-queue")]
    public string OutputMessageToQueue { get; set; }
}