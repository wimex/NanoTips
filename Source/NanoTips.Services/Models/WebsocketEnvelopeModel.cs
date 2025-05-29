using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NanoTips.Services.Enums;

namespace NanoTips.Services.Models;

public record WebsocketEnvelopeModel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType Type { get; init; } = MessageType.None;

    public JsonNode? Data { get; init; } = null;
    
    public WebsocketEnvelopeModel<TContent> To<TContent>() where TContent : class
    {
        return new WebsocketEnvelopeModel<TContent>
        {
            Type = this.Type,
            Data = this.Data
        };
    }
}

public record WebsocketEnvelopeModel<TContent> : WebsocketEnvelopeModel where TContent : class
{
    public TContent? Content => this.Data.Deserialize(typeof(TContent)) as TContent;
}