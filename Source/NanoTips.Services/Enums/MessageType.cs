using System.Runtime.Serialization;

namespace NanoTips.Services.Enums;

public enum MessageType
{
    [EnumMember(Value = "none")]
    None = 0,
    
    [EnumMember(Value = "reqConversations")]
    ReqConversations = 1,
    
    [EnumMember(Value = "reqConversation")]
    ReqConversation = 2,
    
    [EnumMember(Value = "getConversations")]
    GetConversations = 3,
}