namespace NanoTips.Web.Components.Settings;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";
    
    public required string ConnectionUri { get; set; }
    public required string DatabaseName { get; set; }
}