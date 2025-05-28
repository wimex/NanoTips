using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public interface IConversationManagerService
{
    Task<IList<ConversationListModel>> GetConversations();
}