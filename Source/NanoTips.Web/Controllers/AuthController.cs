using Microsoft.AspNetCore.Mvc;
using NanoTips.Services.MailboxManager;
using NanoTips.Services.Models;
using NanoTips.Web.Components.Controllers;

namespace NanoTips.Web.Controllers;

public class AuthController(IMailboxManagerService mailboxManagerService) : NanoTipsController
{
    [HttpPost("register")]
    public async Task<ActionResult<MailboxViewModel>> Register(MailboxEditorModel model)
    {
        MailboxViewModel result = await mailboxManagerService.RegisterMailbox(model);
        return this.Ok(result);
    }
}