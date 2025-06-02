using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoTips.Services.ArticleManager;
using NanoTips.Services.ConversationManager;
using NanoTips.Services.Enums;
using NanoTips.Services.Models;

namespace NanoTips.Services.WebsocketHandler;

public class WebsocketHandlerService(ILogger<WebsocketHandlerService> logger, IServiceProvider serviceProvider) : IWebsocketHandlerService
{
    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };
    
    private static ConcurrentDictionary<string, WebSocket> Connections { get; } = new();

    public async Task SendMessageToAll<TMessage>(MessageType type, TMessage content)
    {
        IList<string> connectionIds = Connections.Keys.ToList();
        foreach (string connectionId in connectionIds)
        {
            try
            {
                await this.SendMessage(connectionId, type, content);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send message to connection {ConnectionId}", connectionId);
                Connections.TryRemove(connectionId, out _);
            }
        }
    }


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
            await this.ReceiveMessage(connectionId, message);
        }

        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        Connections.TryRemove(connectionId, out WebSocket _);
    }
    
    private async Task ReceiveMessage(string connectionId, string content)
    {
        _ = Task.Run(async () =>
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            WebsocketEnvelopeModel envelope = JsonSerializer.Deserialize<WebsocketEnvelopeModel>(content, JsonOptions) ?? throw new InvalidOperationException("Invalid message format");
            
            switch (envelope.Type)
            {
                case MessageType.GetConversations:
                {
                    IConversationManagerService conversationManager = scope.ServiceProvider.GetRequiredService<IConversationManagerService>();
                    IList<ConversationListModel> conversations = await conversationManager.GetConversations();
                    await this.SendMessage(connectionId, MessageType.GetConversations, conversations);
                    break;
                }
                case MessageType.GetConversation:
                {
                    WebsocketEnvelopeModel<ConversationViewModel> request = envelope.To<ConversationViewModel>();
                    IConversationManagerService conversationManager = scope.ServiceProvider.GetRequiredService<IConversationManagerService>();
                    ConversationViewModel conversation = await conversationManager.GetConversation(request.Content.ConversationId);
                    await this.SendMessage(connectionId, MessageType.GetConversation, conversation);
                    break;
                }
                case MessageType.GetArticles:
                {
                    IArticleManagerService articleManager = scope.ServiceProvider.GetRequiredService<IArticleManagerService>();
                    IList<ArticleListViewModel> articles = await articleManager.GetArticles();
                    await this.SendMessage(connectionId, MessageType.GetArticles, articles);
                    break;
                }
                case MessageType.EditArticle:
                {
                    WebsocketEnvelopeModel<ArticleEditorModel> request = envelope.To<ArticleEditorModel>();
                    IArticleManagerService articleManager = scope.ServiceProvider.GetRequiredService<IArticleManagerService>();
                    ArticleViewModel article = await articleManager.CreateOrEditArticle(request.Content);
                    ArticleListViewModel list = new()
                    {
                        ArticleId = article.ArticleId,
                        Slug = article.Slug,
                        Title = article.Title,
                    };
                    
                    await this.SendMessage(connectionId, MessageType.GetArticles, list);
                    await this.SendMessage(connectionId, MessageType.GetArticle, article);
                    break;
                }
                default:
                    logger.LogError($"Received unsupported message type: {envelope.Type}");
                    return;
            }
        });
    }
    
    private async Task SendMessage<T>(string connectionId, MessageType type, T content)
    {
        if (!Connections.TryGetValue(connectionId, out WebSocket socket))
            throw new InvalidOperationException("Unable to locate connection");

        WebsocketEnvelopeModel envelope = new WebsocketEnvelopeModel
        {
            Type = type,
            Data = JsonSerializer.SerializeToNode(content, JsonOptions)
        };
        
        string json = JsonSerializer.Serialize(envelope, JsonOptions);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
        
        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}