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

WebApplication app = builder.Build();
app.MapControllers();
app.Run();