using System.Net.WebSockets;
using NanoTips.Services.Enums;

namespace NanoTips.Services.WebsocketHandler;

public interface IWebsocketHandlerService
{
    Task SendMessageToAll<TMessage>(string mailboxId, MessageType type, TMessage content);
    Task AcceptConnection(string mailboxId, WebSocket socket);
}