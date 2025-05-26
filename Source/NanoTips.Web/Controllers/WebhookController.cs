using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Jobs.Webhook;
using NanoTips.Services.WebhookData;
using NanoTips.Web.Components.Controllers;

namespace NanoTips.Web.Controllers;

public class WebhookController(IServiceProvider serviceProvider, IBackgroundJobClient backgroundJobClient) : NanoTipsController
{
    /// <summary>
    /// Saves the incoming webhook data to the database and kicks off any necessary processing.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Index()
    {
        //TODO: Authorize incoming webhook requests
        if (!this.Request.ContentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            return this.BadRequest("The incoming webhook data must be in JSON format.");
       
        ObjectId messageId = ObjectId.GenerateNewId();
        
        using StreamReader reader = new StreamReader(this.Request.Body);
        string data = await reader.ReadToEndAsync();
        
        if (string.IsNullOrEmpty(data))
            return this.BadRequest("Webhook data cannot be null or empty.");

        ThreadPool.QueueUserWorkItem(_ =>
        {
            IJobCancellationToken cancellationToken = new JobCancellationToken(false);
            DataSaverJob dataSaverJob = serviceProvider.GetRequiredService<DataSaverJob>();

            string dataSaverJobId = backgroundJobClient
                .Enqueue(() => dataSaverJob.Execute(messageId, data, cancellationToken));

            string messageCategorizerJobId = backgroundJobClient
                .ContinueJobWith(dataSaverJobId, () => dataSaverJob.Execute(messageId, data, cancellationToken), JobContinuationOptions.OnlyOnSucceededState);
        });

        return this.Ok("Your data is being processed.");
    }
}