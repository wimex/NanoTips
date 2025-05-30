using System.ComponentModel;
using Hangfire;
using Hangfire.Dashboard.Management.v2.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NanoTips.Services.ConversationManager;
using NanoTips.Services.EmailResponder;
using NanoTips.Services.Enums;
using NanoTips.Services.Models;
using NanoTips.Services.OpenAi;
using NanoTips.Services.WebhookData;
using NanoTips.Services.WebsocketHandler;

namespace NanoTips.Jobs.Webhook;

public class DataCategorizerJob(IServiceProvider serviceProvider) : IJob
{
    [DisplayName("Categorize Webhook Data")]
    [Description("Creates threads from incoming messages and tries to categorize them based on their content.")]
    [DisableConcurrentExecution(30)]
    [AutomaticRetry(Attempts = 0)]
    public async Task Execute(string webhookMessageId, string conversationId, string conversationMessageId, IJobCancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(webhookMessageId);
        ArgumentNullException.ThrowIfNull(conversationId);
        ArgumentNullException.ThrowIfNull(conversationMessageId);
        ArgumentNullException.ThrowIfNull(cancellation);
        
        ILogger<DataCategorizerJob> logger = serviceProvider.GetRequiredService<ILogger<DataCategorizerJob>>();
        IWebhookDataService webhookDataService = serviceProvider.GetRequiredService<IWebhookDataService>();
        IEmailResponderService emailResponderService = serviceProvider.GetRequiredService<IEmailResponderService>();
        IConversationManagerService conversationManagerService = serviceProvider.GetRequiredService<IConversationManagerService>();
        IWebsocketHandlerService websocketHandlerService = serviceProvider.GetRequiredService<IWebsocketHandlerService>();

        logger.LogInformation("Starting categorization for webhook message {WebhookMessageId} to conversation {ConversationId}", webhookMessageId, conversationId);
        await webhookDataService.CreateConversationFromMessage(ObjectId.Parse(webhookMessageId), ObjectId.Parse(conversationId), ObjectId.Parse(conversationMessageId));
        
        logger.LogInformation("Created conversation message {ConversationMessageId} for webhook message {WebhookMessageId}", conversationMessageId, webhookMessageId);
        await emailResponderService.TryRespondingToMail(ObjectId.Parse(conversationMessageId));
        
        IList<ConversationListModel> conversations = await conversationManagerService.GetConversations();
        ConversationViewModel conversation = await conversationManagerService.GetConversation(conversationId);
        await websocketHandlerService.SendMessageToAll(MessageType.GetConversations, conversations);
        await websocketHandlerService.SendMessageToAll(MessageType.GetConversation, conversation);
    }
}