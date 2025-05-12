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

        // 1. Configure logging (optional)
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        }); 

        // 2. Add DbContext with correct connection string + split query
        services.AddDbContext<BankAppDataContext>(options =>
            options.UseSqlServer("Server=.;Database=BankAppData;Trusted_Connection=True;MultipleActiveResultSets=true;")

        );

        // 3. Register your service
        services.AddScoped<ISuspiciousTransactionService, SuspiciousTransactionService>();

        // 4. Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // 5. Run the detection
        var suspiciousService = serviceProvider.GetRequiredService<ISuspiciousTransactionService>();
        var suspiciousList = await suspiciousService.DetectSuspiciousTransactionsAsync();

        Console.WriteLine($"Found {suspiciousList.Count} suspicious transactions.");

        // 6. Build reports folder (under project root)
        var now = DateTime.Now;
        string baseFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reports");
        string reportFolder = Path.Combine(baseFolder, now.ToString("yyyy-MM-dd_HHmmss"));
        Directory.CreateDirectory(reportFolder);

        // 7. Load previously reported TransactionIds
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

        // 8. Generate reports
        var groupedByCountry = suspiciousList.GroupBy(s => s.Country);
        foreach (var group in groupedByCountry)
        {
            var country = group.Key;
            var newOnes = group.Where(t => !reportedIds.Contains(t.TransactionId)).ToList();
            var reportFile = Path.Combine(reportFolder, $"SuspiciousReport_{country}.txt");

            if (newOnes.Any())
            {
                var lines = newOnes.Select(s =>
                   $"CustomerName: {s.FullName}, accountId: {s.AccountId}, TransactionId: {s.TransactionId}, Amount {s.Amount},  Country: {s.Country}, DetectedAt: {s.DetectedAt}"
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

        // 9. Open the folder in Explorer
        Process.Start("explorer.exe", Path.GetFullPath(reportFolder));
    }
}
