using Distributed.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Distributed;

public class DistributedCache(IHostApplicationLifetime appLifetime, ILongCalcSimulationService longCalcSimulationService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await WorkingStrategy();
        appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    private async Task WorkingStrategy()
    {
        while (true)
        {
            Console.WriteLine("Wybierz tryb pracy:");
            Console.WriteLine("1. Rozproszony Cache");
            // Console.WriteLine("2. Wielowątkowa nie poprawna implementacja");
            // Console.WriteLine("3. Wielowątkowa poprawna implementacja");
            // Console.WriteLine("4. Zakończ");

            var userInput = Console.ReadLine() ?? "";
            var userStrategy = ConvertAndValidateUserInputDuringChoosingStrategy(userInput);
            Console.Clear();
            switch (userStrategy)
            {
                case 1:
                    await DistributedCacheImplementation();
                    break;
                // case 2:
                //     InvalidParallelCacheImplementation();
                //     break;
                // case 3:
                //     CorrectParallelCacheImplementation();
                //     break;
                // case 4:
                //     return;
                default:
                    Console.WriteLine("Nie poprawna wartość");
                    break;
            }
        }
    }
    
    private int ConvertAndValidateUserInputDuringChoosingStrategy(string userInput)
    {
        var success = int.TryParse(userInput, out int userInteager);
        if (success)
            return userInteager;
        return -1;
    }
    
    private async Task DistributedCacheImplementation()
    {
        Console.WriteLine("Program działa w następujący sposób:");
        Console.WriteLine("Podanie wartości spowoduje wygenerowanie dla niej numeru po określonym czasie.");
        Console.WriteLine("Każde następne podanie tego samego numeru spowoduje natuchmiastowy zwrot wartości z distributed cache");
        Console.WriteLine("Podanie wartości nie będącej liczbą spowoduje wyjście z trybu pracy");
        while (true)
        {
            var userInput = Console.ReadLine() ?? "";
            var success = int.TryParse(userInput, out int userInteager);
            if (success)
            {
                var calculatedValue = await longCalcSimulationService.CalculateAsync(userInteager);
                Console.Clear();
                Console.WriteLine("Wygenerowana wartość");
                Console.WriteLine(calculatedValue);
                Console.WriteLine("Podanie wartości nie będącej liczbą spowoduje wyjście z trybu pracy");
                continue;
            }
            
            Console.Clear();
            return;
        }
    }
}