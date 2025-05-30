using System.Net.WebSockets;
using NanoTips.Services.Enums;

namespace NanoTips.Services.WebsocketHandler;

public interface IWebsocketHandlerService
{
    Task SendMessageToAll<TMessage>(MessageType type, TMessage content);
    Task AcceptConnection(WebSocket socket);
}