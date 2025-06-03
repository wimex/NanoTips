using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using NanoTips.Services.WebsocketHandler;

namespace NanoTips.Web.Controllers;

public class WebsocketController(IWebsocketHandlerService websocketHandlerService) : ControllerBase
{
    [Route("/ws/{mailboxId}")]
    public async Task Get([FromRoute] string mailboxId)
    {
        if (!this.HttpContext.WebSockets.IsWebSocketRequest)
        {
            this.HttpContext.Response.StatusCode = 400;
        }
        else
        {
            WebSocket socket = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
            await websocketHandlerService.AcceptConnection(mailboxId, socket);
        }
    }
}