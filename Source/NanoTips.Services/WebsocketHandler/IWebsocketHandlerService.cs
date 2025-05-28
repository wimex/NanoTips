using System.Net.WebSockets;

namespace NanoTips.Services.WebsocketHandler;

public interface IWebsocketHandlerService
{
    Task AcceptConnection(WebSocket socket);
}