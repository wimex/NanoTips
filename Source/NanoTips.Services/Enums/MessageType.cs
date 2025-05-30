using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NanoTips.Services.Enums;

public enum MessageType
{
    [EnumMember(Value = "none")]
    [JsonStringEnumMemberName("none")]
    None = 0,
    
    [EnumMember(Value = "reqConversations")]
    [JsonStringEnumMemberName("reqConversations")]
    ReqConversations = 1,
    
    [EnumMember(Value = "reqConversation")]
    [JsonStringEnumMemberName("reqConversation")]
    ReqConversation = 2,
    
    [EnumMember(Value = "getConversations")]
    [JsonStringEnumMemberName("getConversations")]
    GetConversations = 3,
}