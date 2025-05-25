using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Services.WebhookData;
using NanoTips.Web.Components.Controllers;

namespace NanoTips.Web.Controllers;

public class WebhookController(IWebhookDataService webhookDataService) : NanoTipsController
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
        await webhookDataService.SaveIncomingWebhookData(messageId, this.Request.Body);
        
        return this.Ok();
    }
}