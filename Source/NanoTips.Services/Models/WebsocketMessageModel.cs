using System.Text.Json.Serialization;
using NanoTips.Services.Enums;

namespace NanoTips.Services.Models;

public record WebsocketMessageModel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType Type { get; init; } = MessageType.None;
}