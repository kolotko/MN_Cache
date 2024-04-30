namespace Distributed.Abstractions;

public interface ILongCalcSimulationService
{
    Task<int> CalculateAsync(int calculationSeed);
}