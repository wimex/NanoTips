using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Jobs.Webhook;
using NanoTips.Services.WebhookData;
using NanoTips.Web.Components.Controllers;

namespace NanoTips.Web.Controllers;

public class WebhookController(ILogger<WebhookController> logger, IServiceProvider serviceProvider, IBackgroundJobClient backgroundJobClient) : NanoTipsController
{
    /// <summary>
    /// Saves the incoming webhook data to the database and kicks off any necessary processing.
    /// </summary>
    /// <returns></returns>
    [HttpPost("{mailboxId}")]
    public async Task<ActionResult> Index([FromRoute] string mailboxId)
    {
        //TODO: Authorize incoming webhook requests
        if (!this.Request.ContentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            return this.BadRequest("The incoming webhook data must be in JSON format.");
       
        ObjectId messageId = ObjectId.GenerateNewId();
        ObjectId threadId = ObjectId.GenerateNewId();
        ObjectId conversationId = ObjectId.GenerateNewId();
        ObjectId outgoingId = ObjectId.GenerateNewId();
        
        using StreamReader reader = new StreamReader(this.Request.Body);
        string data = await reader.ReadToEndAsync();
        
        if (string.IsNullOrEmpty(data))
            return this.BadRequest("Webhook data cannot be null or empty.");

        ThreadPool.QueueUserWorkItem(_ =>
        {
            IServiceScope scope = serviceProvider.CreateScope();
            IJobCancellationToken cancellationToken = new JobCancellationToken(false);
            DataSaverJob dataSaverJob = scope.ServiceProvider.GetRequiredService<DataSaverJob>();
            DataCategorizerJob dataCategorizerJob = scope.ServiceProvider.GetRequiredService<DataCategorizerJob>();

            string dataSaverJobId = backgroundJobClient
                .Enqueue(() => dataSaverJob.Execute(mailboxId, messageId.ToString(), data, cancellationToken));

            string dataCategorizerJobId = backgroundJobClient
                .ContinueJobWith(dataSaverJobId, () => dataCategorizerJob.Execute(mailboxId, messageId.ToString(), threadId.ToString(), conversationId.ToString(), cancellationToken), JobContinuationOptions.OnlyOnSucceededState);

            logger.LogInformation("Incoming message processing started: {DataSaverJobId} => {DataCategorizerJobId}", dataSaverJobId, dataCategorizerJobId);
        });

        return this.Ok("Your data is being processed.");
    }
}