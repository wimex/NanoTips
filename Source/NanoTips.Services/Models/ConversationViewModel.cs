namespace NanoTips.Services.Models;

public class ConversationViewModel
{
    public required string ConversationId { get; init; }
    public required string Subject { get; init; }
    
    public IList<ConversationMessageViewModel> Messages { get; init; } = new List<ConversationMessageViewModel>();
}