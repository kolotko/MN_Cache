using InMemory.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Services;

public class LongCalcSimulationServiceWithCache(ILongCalcSimulationService longCalcSimulationService, IMemoryCache memoryCache) : ILongCalcSimulationService
{
    public async Task<int> CalculateAsync(int calculationSeed)
    {
        return await memoryCache.GetOrCreateAsync(calculationSeed, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await longCalcSimulationService.CalculateAsync(calculationSeed);
        });
    }
}