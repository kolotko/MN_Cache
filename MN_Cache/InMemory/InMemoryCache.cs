using InMemory.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace InMemory;

public class InMemoryCache(IHostApplicationLifetime appLifetime, ILongCalcSimulationService longCalcSimulationService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await WorkingStrategy();
        appLifetime.StopApplication();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private async Task WorkingStrategy()
    {
        while (true)
        {
            Console.WriteLine("Wybierz tryb pracy:");
            Console.WriteLine("1. Manualna");
            Console.WriteLine("2. Wielowątkowa nie poprawna implementacja");
            Console.WriteLine("3. Wielowątkowa poprawna implementacja");
            Console.WriteLine("4. Zakończ");

            var userInput = Console.ReadLine() ?? "";
            var userStrategy = ConvertAndValidateUserInputDuringChoosingStrategy(userInput);
            Console.Clear();
            switch (userStrategy)
            {
                case 1:
                    await ManualCacheImplementation();
                    break;
                case 2:
                    InvalidParallelCacheImplementation();
                    break;
                case 3:
                    continue;
                case 4:
                    return;
                default:
                    Console.WriteLine("Nie poprawna wartość");
                    break;
            }
            
        
            // await longCalcSimulationService.CalculateAsync(1);
        }
    }

    private int ConvertAndValidateUserInputDuringChoosingStrategy(string userInput)
    {
        var success = int.TryParse(userInput, out int userInteager);
        if (success)
            return userInteager;
        return -1;
    }

    private async Task ManualCacheImplementation()
    {
        Console.WriteLine("Program działa w następujący sposób:");
        Console.WriteLine("Podanie wartości spowoduje wygenerowanie dla niej numeru po określonym czasie.");
        Console.WriteLine("Każde następne podanie tego samego numeru spowoduje natuchmiastowy zwrot wartości z cache");
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
    
    private void InvalidParallelCacheImplementation()
    {
        Console.WriteLine("Program działa w następujący sposób:");
        Console.WriteLine("Program Wykonuje 50 powtórzeń i numer pętli próbuje umieścić w cache pod tym samym kluczem");
        Console.WriteLine("Biblioteka jest oparta na kolekcji ConcurrentDictionary która jest thread safe, ale nie na insercie");
        Console.WriteLine("Program powinien wyświetlić duplikaty liczb");
        
        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Clear();
        Parallel.ForEach(Enumerable.Range(1, 50), loopCounter =>
        {
            var cachedItem = cache.GetOrCreate("key", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                return loopCounter;
            });
            Console.Write($"{cachedItem}");
        });
        
        Console.WriteLine("");
    }
}