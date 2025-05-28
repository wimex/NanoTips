using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using NanoTips.Services.Enums;
using NanoTips.Services.Models;

namespace NanoTips.Services.WebsocketHandler;

public class WebsocketHandlerService(IServiceProvider serviceProvider) : IWebsocketHandlerService
{
    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };
    
    private static ConcurrentDictionary<string, WebSocket> Connections { get; } = new();

    public async Task AcceptConnection(WebSocket socket)
    {
        string connectionId = Guid.NewGuid().ToString();
        Connections.TryAdd(connectionId, socket);
        await this.ManageConnection(connectionId);
    }
    
    private async Task ManageConnection(string connectionId)
    {
        if (!Connections.TryGetValue(connectionId, out WebSocket socket))
            throw new InvalidOperationException("Unable to locate connection");
        
        byte[] buffer = new byte[1024 * 4];
        bool closed = false;
        
        while (!closed)
        {
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            closed = result.CloseStatus.HasValue;
            
            string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            this.ReceiveMessage(connectionId, message);
        }

        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        Connections.TryRemove(connectionId, out WebSocket _);
    }
    
    private async Task ReceiveMessage(string connectionId, string content)
    {
        WebsocketMessageModel message = JsonSerializer.Deserialize<WebsocketMessageModel>(content, JsonOptions) ?? throw new InvalidOperationException("Invalid message format");
        switch (message.Type)
        {
            case MessageType.GetConversations:
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    // Get messages and send response
                });
                break;
            default:
                throw new NotSupportedException($"Message type {message.Type} is not supported.");
        }
    }
}