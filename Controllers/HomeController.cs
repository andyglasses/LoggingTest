using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LoggingTest.Controllers;
public class HomeController(DemoConnector demoConnector) : Controller
{
    [HttpGet("ok")]
    public IActionResult Success()
    {
        return Json("Hello World");
    }

    [HttpGet("call-api/{number}")]
    public async Task<IActionResult> CallApi(long number)
    {
        var result = await demoConnector.DoAction(number);
        return Json(result);
    }

    [HttpGet("exception")]
    public IActionResult Exception()
    {
        throw new Exception("Example Error");
    }

    [HttpGet("inner-exception")]
    public IActionResult InnerException()
    {
        var demoStackGenerator = new DemoStackGenerator();
        demoStackGenerator.DoAction();
        return Ok();
    }

    [HttpGet("other/{type}")]
    public IActionResult Other(string type)
    {
        switch (type.ToLowerInvariant())
        {
            case "notfound":
                return NotFound();
            case "badrequest":
                return BadRequest();
            case "unauthorized":
                return Unauthorized();
            case "forbidden":
                return Forbid();
            default:
                return Ok($"Unknown type:{type}");
        }
    }

    [HttpGet("log-error/{message}")]
    public IActionResult LogError(string? message)
    {
        Log.Error(message??"Default Error Message");
        return Ok($"Called 'Log.Error(\"{message ?? "Default Error Message"}\")'");
    }

    [HttpGet("log-warning/{message}")]
    public IActionResult LogWarning(string? message)
    {
        Log.Warning(message ?? "Default Warning Message");
        return Ok($"Called 'Log.Warning(\"{message ?? "Default Warning Message"}\")'");
    }

    [HttpGet("log-info/{message}")]
    public IActionResult LogInfo(string? message)
    {
        Log.Information(message ?? "Default Info Message");
        return Ok($"Called 'Log.Information(\"{message ?? "Default Info Message"}\")'");
    }

    [HttpGet("log-debug/{message}")]
    public IActionResult LogDebug(string? message)
    {
        Log.Debug(message ?? "Default Debug Message");
        return Ok($"Called 'Log.Debug(\"{message ?? "Default Debug Message"}\")'");
    }

    [HttpGet("log-fatal/{message}")]
    public IActionResult LogFatal(string? message)
    {
        Log.Fatal(message ?? "Default Fatal Message");
        return Ok($"Called 'Log.Fatal(\"{message ?? "Default Fatal Message"}\")'");
    }

    [HttpGet("log-multiple/{message}")]
    public IActionResult LogMultipleFatal(string? message)
    {
        Log.Error((message ?? "Default Fatal Message") + " 1");
        Log.Error((message ?? "Default Fatal Message") + " 2");
        Log.Error((message ?? "Default Fatal Message") + " 3");
        Log.Error((message ?? "Default Fatal Message") + " 4");
        return Ok($"Called 'Log.Error(\"{message ?? "Default Fatal Message"}\")' 4 times");
    }




}
