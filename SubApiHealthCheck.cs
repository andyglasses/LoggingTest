using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LoggingTest;

public class SubApiHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://www.boredapi.com/api/";
                if (DateTime.Now.Minute % 3 == 0)
                {
                    url = "https://www.boredapiigotthewrongurl.com/api/";
                }
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("OK");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("API Error");
                }
            }
        }
        catch (Exception e)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: e);
        }
    }
}
