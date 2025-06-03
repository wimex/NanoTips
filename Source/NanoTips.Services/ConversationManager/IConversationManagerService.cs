using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public interface IConversationManagerService
{
    Task<ConversationViewModel> ReplyToConversation(ConversationEditorModel model);
    Task<ConversationViewModel> GetConversation(string conversationId);
    Task<IList<ConversationListModel>> GetConversations();
}