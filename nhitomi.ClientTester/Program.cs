using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using nhitomi.Core;
using nhitomi.Core.Clients.nhentai;
using nhitomi.Core.Clients.Hitomi;

Console.WriteLine("========================================");
Console.WriteLine("  nhitomi Client Tester");
Console.WriteLine("========================================");
Console.WriteLine();

// Create logging factory
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

// Create HTTP client
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("nhitomi/3.4 (Client Tester)");
var http = new SimpleHttpClient(httpClient);
var json = JsonSerializer.Create(new nhitomiSerializerSettings());

// Create clients
var nhentaiLogger = loggerFactory.CreateLogger<nhentaiClient>();
var hitomiLogger = loggerFactory.CreateLogger<HitomiClient>();
var nhentai = new nhentaiClient(http, json, nhentaiLogger);
var hitomi = new HitomiClient(http, json, hitomiLogger);

// Test menu
while (true)
{
    Console.WriteLine("Select an option:");
    Console.WriteLine("  1. Test nhentai client (fetch single doujin)");
    Console.WriteLine("  2. Test Hitomi client (fetch single doujin)");
    Console.WriteLine("  3. List recent nhentai IDs");
    Console.WriteLine("  4. List recent Hitomi IDs");
    Console.WriteLine("  5. Exit");
    Console.WriteLine();
    Console.Write("Choice: ");

    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1":
                await TestNhentaiAsync();
                break;
            case "2":
                await TestHitomiAsync();
                break;
            case "3":
                await ListNhentaiIdsAsync();
                break;
            case "4":
                await ListHitomiIdsAsync();
                break;
            case "5":
                return;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        Console.ResetColor();
    }

    Console.WriteLine();
}

async Task TestNhentaiAsync()
{
    Console.Write("Enter nhentai gallery ID (e.g., 177013): ");
    var id = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("No ID provided.");
        return;
    }

    Console.WriteLine($"Fetching nhentai #{id}...");

    var doujin = await nhentai.GetAsync(id);

    if (doujin == null)
    {
        Console.WriteLine("Doujin not found.");
        return;
    }

    PrintDoujinInfo(doujin);
}

async Task TestHitomiAsync()
{
    Console.Write("Enter Hitomi gallery ID (e.g., 1234567): ");
    var id = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(id))
    {
        Console.WriteLine("No ID provided.");
        return;
    }

    Console.WriteLine($"Fetching Hitomi #{id}...");

    var doujin = await hitomi.GetAsync(id);

    if (doujin == null)
    {
        Console.WriteLine("Doujin not found (may be anime type which is filtered).");
        return;
    }

    PrintDoujinInfo(doujin);
}

async Task ListNhentaiIdsAsync()
{
    Console.WriteLine("Fetching recent nhentai IDs...");

    var ids = await nhentai.EnumerateAsync(null);
    var count = 0;

    foreach (var id in ids)
    {
        Console.WriteLine($"  ID: {id}");
        if (++count >= 10)
        {
            Console.WriteLine("  ... (showing first 10)");
            break;
        }
    }
}

async Task ListHitomiIdsAsync()
{
    Console.WriteLine("Fetching recent Hitomi IDs (this may take a moment)...");

    var ids = await hitomi.EnumerateAsync(null);
    var count = 0;

    foreach (var id in ids)
    {
        Console.WriteLine($"  ID: {id}");
        if (++count >= 10)
        {
            Console.WriteLine("  ... (showing first 10)");
            break;
        }
    }
}

void PrintDoujinInfo(DoujinInfo doujin)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("=== Doujin Info ===");
    Console.ResetColor();

    Console.WriteLine($"  Pretty Name:   {doujin.PrettyName}");
    Console.WriteLine($"  Original Name: {doujin.OriginalName}");
    Console.WriteLine($"  Source:        {doujin.Source?.Name ?? "N/A"}");
    Console.WriteLine($"  Source ID:     {doujin.SourceId}");
    Console.WriteLine($"  Upload Time:   {doujin.UploadTime}");
    Console.WriteLine($"  Page Count:    {doujin.PageCount}");
    Console.WriteLine($"  Artist:        {doujin.Artist ?? "N/A"}");
    Console.WriteLine($"  Group:         {doujin.Group ?? "N/A"}");
    Console.WriteLine($"  Language:      {doujin.Language ?? "N/A"}");
    Console.WriteLine($"  Parody:        {doujin.Parody ?? "N/A"}");
    Console.WriteLine($"  Characters:    {string.Join(", ", doujin.Characters ?? Enumerable.Empty<string>())}");
    Console.WriteLine($"  Categories:    {string.Join(", ", doujin.Categories ?? Enumerable.Empty<string>())}");
    Console.WriteLine($"  Tags:          {string.Join(", ", doujin.Tags ?? Enumerable.Empty<string>())}");
}

// Simple HTTP client wrapper
class SimpleHttpClient : IHttpClient
{
    public HttpClient Http { get; }

    public SimpleHttpClient(HttpClient client) => Http = client;

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
}
