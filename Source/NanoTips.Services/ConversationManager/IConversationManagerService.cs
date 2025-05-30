using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public interface IConversationManagerService
{
    Task<ConversationViewModel> GetConversation(string conversationId);
    Task<IList<ConversationListModel>> GetConversations();
}