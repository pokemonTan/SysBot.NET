using NapCatScript.Core.Services;
using System.Diagnostics.CodeAnalysis;

namespace NapCatScript.Core.JsonFormat.Msgs;

/// <summary>
/// 回复消息的json
/// </summary>
public class ReplyJson : MsgJson
{
    #region ctor

    [JsonConstructor]
    private ReplyJson()
    {
        
    }
    
    public ReplyJson(long message_id)
    {
        Data = new ReplyMsgData(message_id);
    }

    #endregion
    
    [JsonPropertyName("data")]
    public ReplyMsgData Data { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = nameof(MsgType.reply);

    public class ReplyMsgData
    {
        [JsonConstructor]
        private ReplyMsgData()
        {
            
        }
        
        [SetsRequiredMembers]
        public ReplyMsgData(long message_id)
        {
            MessageId = message_id;
        }

        [JsonPropertyName("id")]
        public required long MessageId { get; set; }
    }
}


