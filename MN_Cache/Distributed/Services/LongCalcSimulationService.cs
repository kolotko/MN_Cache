using Distributed.Abstractions;

namespace Distributed.Services;

public class LongCalcSimulationService : ILongCalcSimulationService
{
    public async Task<int> CalculateAsync(int calculationSeed)
    {
        await Task.Delay(2000);
        Random random = new Random(calculationSeed);
        return random.Next(1, 100);
    }
}