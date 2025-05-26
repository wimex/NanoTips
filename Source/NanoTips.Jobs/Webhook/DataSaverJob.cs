using System.ComponentModel;
using Hangfire;
using Hangfire.Dashboard.Management.v2.Support;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using NanoTips.Services.WebhookData;

namespace NanoTips.Jobs.Webhook;

public class DataSaverJob(IServiceProvider serviceProvider) : IJob
{
    [DisplayName("Save Webhook Data")]
    [Description("Saves raw webhook data to the database.")]
    [DisableConcurrentExecution(30)]
    [AutomaticRetry(Attempts = 0)]
    public async Task Execute(string messageId, string data, IJobCancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(messageId);
        ArgumentException.ThrowIfNullOrEmpty(data);
        ArgumentNullException.ThrowIfNull(cancellation);

        IWebhookDataService webhookDataService = serviceProvider.GetRequiredService<IWebhookDataService>();
        await webhookDataService.SaveIncomingWebhookData(ObjectId.Parse(messageId), data);
    }
}