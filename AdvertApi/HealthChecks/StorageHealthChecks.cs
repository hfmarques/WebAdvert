using AdvertApi.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdvertApi.HealthChecks;

public class StorageHealthChecks : IHealthCheck
{
    private readonly IAdvertStorageService advertStorageService;

    public StorageHealthChecks(IAdvertStorageService advertStorageService)
    {
        this.advertStorageService = advertStorageService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var isStorageOk = await advertStorageService.CheckHealthAsync();

        return new HealthCheckResult(isStorageOk ? HealthStatus.Healthy : HealthStatus.Unhealthy);
    }
}