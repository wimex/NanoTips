using MongoDB.Driver;
using NanoTips.Web.Components.Settings;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
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