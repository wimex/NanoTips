using System.ComponentModel;
using Hangfire;
using Hangfire.Dashboard.Management.v2.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using PostmarkDotNet;

namespace NanoTips.Jobs.Webhook;

public class MailSenderJob(IServiceProvider serviceProvider) : IJob
{
    [DisplayName("Send Email")]
    [Description("Sends an email based on the provided conversation message ID.")]
    [DisableConcurrentExecution(30)]
    [AutomaticRetry(Attempts = 0)]
    public async Task Execute(string conversationMessageId, IJobCancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(conversationMessageId);
        ArgumentNullException.ThrowIfNull(cancellation);

        ILogger<MailSenderJob> logger = serviceProvider.GetRequiredService<ILogger<MailSenderJob>>();
        IMongoDatabase database = serviceProvider.GetRequiredService<IMongoDatabase>();

        IMongoCollection<ConversationMessage> messages = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        IMongoCollection<SystemMailbox> mailboxes = database.GetCollection<SystemMailbox>(NanoTipsCollections.SystemMailboxes);

        ConversationMessage message = await messages.Find(m => m.Id == ObjectId.Parse(conversationMessageId)).FirstOrDefaultAsync(cancellation.ShutdownToken);
        if (message == null)
        {
            logger.LogError("Conversation message with ID {ConversationMessageId} not found.", conversationMessageId);
            return;
        }

        if (message.Processed != null)
        {
            logger.LogInformation("Conversation message {ConversationMessageId} has already been processed.", conversationMessageId);
            return;
        }

        SystemMailbox mailbox = await mailboxes.Find(m => m.Id == message.MailboxId).FirstOrDefaultAsync(cancellation.ShutdownToken);
        if (mailbox == null)
        {
            logger.LogError("Mailbox with ID {MailboxId} not found for conversation message {ConversationMessageId}.", message.MailboxId, conversationMessageId);
            return;
        }

        PostmarkMessage envelope = new PostmarkMessage
        {
            From = "postmarkapp@bbm.dev",
            To = message.Recipient,
            Subject = message.Subject,
            HtmlBody = message.Body,
            TextBody = message.Body,
        };

        PostmarkClient client = new PostmarkClient(mailbox.PostmarkApiKey);
        PostmarkResponse response = await client.SendMessageAsync(envelope);
        
        logger.LogInformation("Email sent with ID {PostmarkMessageId} for conversation message {ConversationMessageId}. {Status}, {Error}", response.MessageID, conversationMessageId, response.Status, response.ErrorCode);
        await messages.UpdateOneAsync(m => m.Id == message.Id, Builders<ConversationMessage>.Update.Set(m => m.Processed, DateTime.UtcNow), cancellationToken: cancellation.ShutdownToken);
    }
}