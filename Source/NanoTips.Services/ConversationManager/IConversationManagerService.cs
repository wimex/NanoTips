using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public interface IConversationManagerService
{
    Task<ConversationViewModel> ReplyToConversation(string mailboxId, ConversationEditorModel model);
    Task<ConversationViewModel> GetConversation(string conversationId);
    Task<IList<ConversationListModel>> GetConversations(string mailboxId);
}