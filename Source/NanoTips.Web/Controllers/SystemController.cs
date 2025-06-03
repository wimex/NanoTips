using Microsoft.AspNetCore.Mvc;
using NanoTips.Web.Components.Controllers;

namespace NanoTips.Web.Controllers;

public class SystemController : NanoTipsController
{
    [Route("/system/health")]
    public IActionResult HealthCheck()
    {
        return this.Ok("OK");
    }
}