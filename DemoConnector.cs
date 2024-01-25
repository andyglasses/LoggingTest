using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace LoggingTest;

public class DemoConnector(ILogger<DemoConnector> logger)
{
    public async Task<string?> DoAction(long participants)
    {
        using (HttpClient client = new HttpClient())
        {
            
            HttpResponseMessage response = await client.GetAsync($"http://www.boredapi.com/api/activity?participants={participants}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                try
                {
                    var modelResult = JsonSerializer.Deserialize<Result>(result);
                    if (modelResult?.key == null)
                    {
                        var exception = new Exception("Unable to deserialize");
                        exception.Data.Add("Response Content", result);
                        throw exception;
                    }
                    return modelResult?.activity;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error parsing response");
                    return null;
                }
            }
            
            // Handle error
            logger.LogError($"API Error: {response.StatusCode}");
            return null;
            
        }
    }

}

public class Result
{
    public string? activity { get; set; }
    public string? type { get; set; }
    public int? participants { get; set; }
    public float? price { get; set; }
    public string? link { get; set; }
    public string? key { get; set; }
    public float? accessibility { get; set; }
}
