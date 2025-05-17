using DatabaseLayer.DTOs;
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

        Console.WriteLine("Press any key to start checking it takes 30s");
        Console.ReadLine();
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // DbContext
        services.AddDbContext<BankAppDataContext>(options =>
            options.UseSqlServer    ("Server=localhost;Database=BankAppData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
        );

        // Service registration
        services.AddScoped<ISuspiciousTransactionService, SuspiciousTransactionService>();
        var serviceProvider = services.BuildServiceProvider();

        // Detect suspicious transactions
        var suspiciousService = serviceProvider.GetRequiredService<ISuspiciousTransactionService>();
        var suspiciousList = await suspiciousService.DetectSuspiciousTransactionsAsync();

        var now = DateTime.Now;
        string baseFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reports");
        string timestamp = now.ToString("yyyy-MM-dd_HHmm");
        string folderName = $"SuspiciousReports_{timestamp}";
        string reportFolder = Path.Combine(baseFolder, folderName);
        Directory.CreateDirectory(reportFolder);

        // Load reported IDs (from all previous .txt files)
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

        // Group by country
        var groupedByCountry = suspiciousList
            .GroupBy(s => s.Country)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Detect all countries from Customers table
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BankAppDataContext>();
        var allCountries = await context.Customers.Select(c => c.Country).Distinct().ToListAsync();

        foreach (var country in allCountries)
        {
            var reportFile = Path.Combine(reportFolder, $"SuspiciousReport_{country}.txt");

            var newOnes = groupedByCountry.ContainsKey(country)
                ? groupedByCountry[country].Where(t => !reportedIds.Contains(t.TransactionId)).ToList()
                : new List<SuspiciousTransactionDto>();

            if (newOnes.Any())
            {
                var lines = newOnes.Select(s =>
                   $"CustomerName: {s.FullName}, accountId: {s.AccountId}, TransactionId: {s.TransactionId}, TransactionDate: {s.Date}, Amount: {s.Amount}, Country: {s.Country}, DetectedAt: {s.DetectedAt}"
                );
                File.WriteAllLines(reportFile, lines);
            }
            else
            {
                File.WriteAllText(reportFile, $"{country} has no new suspicious transactions since last check.");
            }
        }

        Console.WriteLine("✅ Detection complete. Reports written to:");
        Console.WriteLine(reportFolder);

        Process.Start("explorer.exe", Path.GetFullPath(reportFolder));
    }

}
