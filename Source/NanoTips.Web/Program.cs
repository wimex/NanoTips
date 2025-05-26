using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;
using NanoTips.Services.WebhookData;
using NanoTips.Web.Components.Settings;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IWebhookDataService, WebhookDataService>();
builder.Services.AddControllers();

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
app.UseHangfireServer();
app.UseHangfireDashboard();
app.MapControllers();
app.Run();