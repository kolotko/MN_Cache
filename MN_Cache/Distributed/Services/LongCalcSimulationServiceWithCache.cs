using Distributed.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Distributed.Services;

public class LongCalcSimulationServiceWithCache(IDistributedCache distributedCache, LongCalcSimulationService longCalcSimulationService) : ILongCalcSimulationService
{
    public async Task<int> CalculateAsync(int calculationSeed)
    {
        string? cachedMember = await distributedCache.GetStringAsync($"calculation:{calculationSeed}");

        if (string.IsNullOrEmpty(cachedMember))
        {
            var calculatedValue = await longCalcSimulationService.CalculateAsync(calculationSeed);
            await distributedCache.SetStringAsync($"calculation:{calculationSeed}", calculatedValue.ToString());
            return calculatedValue;
        }

        var success = Int32.TryParse(cachedMember, out int cachedCalculatedResult);
        if (success)
        {
            return cachedCalculatedResult;
        }

        throw new NotImplementedException();
    }
}