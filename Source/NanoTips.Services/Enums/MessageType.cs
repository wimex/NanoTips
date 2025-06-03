using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NanoTips.Services.Enums;

public enum MessageType
{
    [EnumMember(Value = "none")]
    [JsonStringEnumMemberName("none")]
    None = 0,
    
    [EnumMember(Value = "getConversations")]
    [JsonStringEnumMemberName("getConversations")]
    GetConversations = 1,
    
    [EnumMember(Value = "getConversation")]
    [JsonStringEnumMemberName("getConversation")]
    GetConversation = 2,
    
    [EnumMember(Value = "getArticles")]
    [JsonStringEnumMemberName("getArticles")]
    GetArticles = 3,
    
    [EnumMember(Value = "getArticle")]
    [JsonStringEnumMemberName("getArticle")]
    GetArticle = 4,
    
    [EnumMember(Value = "editArticle")]
    [JsonStringEnumMemberName("editArticle")]
    EditArticle = 5,
    
    [EnumMember(Value = "replyConversation")]
    [JsonStringEnumMemberName("replyConversation")]
    ReplyConversation = 6,
}