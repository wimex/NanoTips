using System.Runtime.Serialization;

namespace NanoTips.Services.Enums;

public enum MessageType
{
    [EnumMember(Value = "none")]
    None = 0,
    
    [EnumMember(Value = "getConversations")]
    GetConversations = 1,
}