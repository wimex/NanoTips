using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Web.Components.Controllers;

namespace NanoTips.Web.Controllers;

public class WebhookController(IMongoDatabase database) : NanoTipsController
{
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        return this.Ok();
    }
}