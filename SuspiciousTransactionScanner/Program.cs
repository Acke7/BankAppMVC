using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Transactions;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // DbContext
        services.AddDbContext<BankAppDataContext>(options =>
            options.UseSqlServer("Server=.;Database=BankAppData;Trusted_Connection=True;MultipleActiveResultSets=true;")
        );

        // Service registration
        services.AddScoped<ISuspiciousTransactionService, SuspiciousTransactionService>();
        var serviceProvider = services.BuildServiceProvider();

        // Choose one country
        var countries = new[] { "Sweden", "Finland", "Norway", "Denmark" };
        string selectedCountry = ShowSingleCountryMenu(countries);

        // Detect suspicious transactions
        var suspiciousService = serviceProvider.GetRequiredService<ISuspiciousTransactionService>();
        var suspiciousList = await suspiciousService.DetectSuspiciousTransactionsAsync();

        // Filter by selected country
        suspiciousList = suspiciousList
            .Where(s => s.Country.Equals(selectedCountry, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"Found {suspiciousList.Count} suspicious transactions in {selectedCountry}.");

        // Folder setup: country + timestamp
        var now = DateTime.Now;
        string baseFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reports");
        string timestamp = now.ToString("yyyy-MM-dd_HHmm");
        string folderName = $"{selectedCountry}_{timestamp}";
        string reportFolder = Path.Combine(baseFolder, folderName);
        Directory.CreateDirectory(reportFolder);

        // Load reported IDs
        var reportedIds = new HashSet<int>();
        if (Directory.Exists(baseFolder))
        {
            foreach (var folder in Directory.GetDirectories(baseFolder))
            {
                foreach (var file in Directory.GetFiles(folder, "*.txt"))
                {
                    var lines = File.ReadAllLines(file);
                    foreach (var line in lines)
                    {
                        if (line.Contains("TransactionId:"))
                        {
                            var parts = line.Split("TransactionId:");
                            if (parts.Length > 1 && int.TryParse(parts[1].Trim().Split(',')[0], out int id))
                            {
                                reportedIds.Add(id);
                            }
                        }
                    }
                }
            }
        }

        // Generate single report
        var newOnes = suspiciousList.Where(t => !reportedIds.Contains(t.TransactionId)).ToList();
        var reportFile = Path.Combine(reportFolder, $"SuspiciousReport_{selectedCountry}.txt");

        if (newOnes.Any())
        {
            var lines = newOnes.Select(s =>
               $"CustomerName: {s.FullName}, accountId: {s.AccountId}, TransactionId: {s.TransactionId},TransactionDate:  {s.Date}, Amount: {s.Amount}, Country: {s.Country}, DetectedAt: {s.DetectedAt}"
            );
            File.WriteAllLines(reportFile, lines);
        }
        else
        {
            File.WriteAllText(reportFile, $"{selectedCountry} has no new suspicious transactions since last check.");
        }

        Console.WriteLine("✅ Detection complete. Report written to:");
        Console.WriteLine(reportFolder);

        Process.Start("explorer.exe", Path.GetFullPath(reportFolder));
    }

    static string ShowSingleCountryMenu(string[] countries)
    {
        int index = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("⬇️  Use ↑/↓ to select a country, ENTER to confirm:\n");

            for (int i = 0; i < countries.Length; i++)
            {
                var highlight = i == index ? ">> " : "   ";
                Console.WriteLine($"{highlight}{countries[i]}");
            }

            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    index = (index - 1 + countries.Length) % countries.Length;
                    break;
                case ConsoleKey.DownArrow:
                    index = (index + 1) % countries.Length;
                    break;
            }
        } while (key != ConsoleKey.Enter);

        return countries[index];
    }
}
