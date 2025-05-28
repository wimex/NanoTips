using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;
using NanoTips.Jobs.Webhook;
using NanoTips.Services.ConversationManager;
using NanoTips.Services.EmailResponder;
using NanoTips.Services.OpenAi;
using NanoTips.Services.WebhookData;
using NanoTips.Services.WebsocketHandler;
using NanoTips.Web.Components.Settings;
using OpenAI.Chat;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
if (File.Exists(".env"))
{
    foreach (string line in File.ReadAllLines(".env"))
    {
        if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
            continue;

        string[] parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            continue;
        
        string part1 = parts[0].Trim();
        string part2 = parts[1].Trim();
        if(string.IsNullOrEmpty(part1) || string.IsNullOrEmpty(part2))
            continue;
        
        builder.Configuration[part1] = part2;
    }
}

builder.Services
    .AddTransient<IWebhookDataService, WebhookDataService>()
    .AddTransient<IChatClientService, ChatClientService>()
    .AddTransient<IEmailResponderService, EmailResponderService>()
    .AddTransient<IWebsocketHandlerService, WebsocketHandlerService>()
    .AddTransient<IConversationManagerService, ConversationManagerService>();

builder.Services
    .AddTransient<DataSaverJob>()
    .AddTransient<DataCategorizerJob>();


builder.Services.AddControllers();

builder.Services.AddTransient<ChatClient>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    string? token = configuration["OPENAI_API_KEY"];
    if (string.IsNullOrEmpty(token))
        throw new InvalidOperationException("OpenAI API key is not configured.");

    return new ChatClient("gpt-4o", token);
});

builder.Services.AddSingleton<MongoClient>(_ =>
{
    MongoDbSettings settings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>();
    return new MongoClient(settings.ConnectionUri);
});

builder.Services.AddSingleton<IMongoDatabase>(provider =>
{
    MongoDbSettings settings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>();
    MongoClient client = provider.GetRequiredService<MongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddHangfireServer(options =>
{
    options.ServerName = "NanoTips";
});

builder.Services.AddHangfire(config =>
{
    MongoDbSettings settings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>();
    string server = $"{settings.ConnectionUri}/{settings.DatabaseName}?authSource=admin";

    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMongoStorage(server, new MongoStorageOptions
        {
            MigrationOptions = new(){MigrationStrategy = new MigrateMongoMigrationStrategy(), BackupStrategy = new CollectionMongoBackupStrategy()},
            Prefix = "hangfire",
            CheckConnection = true,
            CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.Poll,
        });
});

WebApplication app = builder.Build();
app.UseWebSockets();


app.UseHangfireServer();
app.UseHangfireDashboard("/hangfire", new DashboardOptions { IsReadOnlyFunc = _ => !builder.Environment.IsDevelopment() });
app.MapControllers();
app.Run();