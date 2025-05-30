using NanoTips.Data.Enums;

namespace NanoTips.Services.Models;

public class ConversationMessageViewModel
{
    public required string MessageId { get; init; }
    
    public required DateTime Created { get; init; }
    public required DateTime? Processed { get; init; }
    
    public required MessageDirection Direction { get; init; }
    
    public required string Sender { get; init; }
    public required string Recipient { get; init; }
    
    public required string Subject { get; init; }
    public required string Body { get; init; }
    
    public Dictionary<string, double> CategorySuggestions { get; init; } = new();
    
    public string? CategoryId { get; init; }
    public bool Sent => this.Processed != null;
}