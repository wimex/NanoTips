using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NanoTips.Data.Enums;

public enum MessageDirection
{
    [EnumMember(Value = "unknown")]
    [JsonStringEnumMemberName("unknown")]
    Unknown = 0,
    
    [EnumMember(Value = "incoming")]
    [JsonStringEnumMemberName("incoming")]
    Incoming = 1,
    
    [EnumMember(Value = "outgoing")]
    [JsonStringEnumMemberName("outgoing")]
    Outgoing = 2,
}