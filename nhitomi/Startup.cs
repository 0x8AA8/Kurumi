using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nhitomi.Core;
using nhitomi.Discord;
using nhitomi.Interactivity;
using Newtonsoft.Json;

namespace nhitomi;

public static class Startup
{
    public static void Configure(IConfigurationBuilder config, IHostEnvironment environment)
    {
        config.SetBasePath(environment.ContentRootPath);
        config.AddJsonFile("appsettings.json", optional: false);
        config.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
        config.AddEnvironmentVariables();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<AppSettings>(configuration);
        var settings = configuration.Get<AppSettings>() ?? new AppSettings();

        // Logging
        services.AddLogging(l => l
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddConsole()
            .AddDebug());

        // Database
        var connectionString = configuration.GetConnectionString("nhitomi");
        services.AddScoped<IDatabase>(s => s.GetRequiredService<nhitomiDbContext>());

        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContextPool<nhitomiDbContext>(d => d
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        }
        else
        {
            // Fallback to SQLite for development
            services.AddDbContextPool<nhitomiDbContext>(d => d
                .UseSqlite("Data Source=nhitomi.db"));
        }

        // Discord services
        services.AddSingleton<DiscordSocketClient>(_ => new DiscordSocketClient(settings.Discord.GetSocketConfig()));
        services.AddSingleton<InteractionService>(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<DiscordService>();
        services.AddSingleton<GalleryUrlDetector>();
        services.AddSingleton<InteractiveManager>();
        services.AddSingleton<GuildSettingsCache>();
        services.AddSingleton<DiscordErrorReporter>();

        // Hosted services
        services.AddHostedInjectableService<MessageHandlerService>();
        services.AddHostedInjectableService<ReactionHandlerService>();
        services.AddHostedInjectableService<StatusUpdateService>();
        services.AddHostedInjectableService<LogHandlerService>();
        services.AddHostedInjectableService<GuildSettingsSyncService>();
        services.AddHostedInjectableService<FeedChannelUpdateService>();
        services.AddHostedInjectableService<GuildWelcomeMessageService>();

        // Other services
        services.AddHttpClient();
        services.AddTransient<IHttpClient, HttpClientWrapper>();
        services.AddTransient(_ => JsonSerializer.Create(new nhitomiSerializerSettings()));
        services.AddHostedInjectableService<ForcedGarbageCollector>();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedInjectableService<TService>(this IServiceCollection collection)
        where TService : class, IHostedService
    {
        return collection
            .AddSingleton<TService>()
            .AddSingleton<IHostedService, TService>(s => s.GetRequiredService<TService>());
    }
}
