namespace NanoTips.Services.Models;

public record MailboxEditorModel
{
    public required string OpenAiApiKey { get; init; }
    public required string PostmarkApiKey { get; init; }
}