using NapCatScript.Core.JsonFormat.EventJson;
using NapCatScript.Core.Model;
using System.Buffers;
using System.Diagnostics;

namespace NapCatScript.Core.MsgHandle;
public static class ReceiveMsg
{
    public static async Task<(MsgInfo?, string)?> ReceiveMsgInfo(this ClientWebSocket socket)
    {
        if (socket.State != WebSocketState.Open)
            return null;

        ArraySegment<byte>? bytes = null;
        MemoryStream? memResult = null;
        WebSocketReceiveResult? result = null;
        try {
            bytes = new ArraySegment<byte>(new byte[1024 * 200]);  //创建分片数组
            memResult = new MemoryStream();    //创建内存流
            result = await socket.ReceiveAsync(bytes.Value, CancellationToken.None); //使用分片数组存储消息 每次调用。分片数组的内容会被重置
            do {
                memResult.Write(bytes.Value.Array, bytes.Value.Offset, result.Count); //写入
            } while (!result.EndOfMessage);

            memResult.Seek(0, SeekOrigin.Begin); //头
            StreamReader fStream = new StreamReader(memResult, Encoding.UTF8);
            string mesgString = fStream.ReadToEnd();
            fStream.Dispose();
            fStream.Close();
            memResult.Dispose();
            memResult.Close();
            if (ValidData(mesgString, out var json)) {
                return (json?.GetMesgInfo(), mesgString)/*?.ToString()*/;
            }
            return null;
        } catch (Exception e) {
            Debug.WriteLine(e.Message);
            if(memResult is not null) {
                memResult.Dispose();
                memResult.Close();
            }
            return null;
        }

    }
    
    public static async Task<MsgInfo?> Receive(this ClientWebSocket socket, CancellationToken CToken)
    {
        (MsgInfo? msgInfo, string msgString)? msg = await socket.ReceiveMsgInfo();
        if(msg is null)
            return null;
        
        return msg.Value.msgInfo;
    }

    public static async Task<(MsgInfo?, ArrayMsg?)?> ReceiveMsgInfoAndJson(this ClientWebSocket socket)
    {
        (MsgInfo? msgInfo, string msgString)? info = await socket.ReceiveMsgInfo();
        if (info is null)
            return null;

        var value = info.Value;
        
        if(value.msgInfo is null) return null;
        ArrayMsg? msg;
        try {
            msg = JsonSerializer.Deserialize<ArrayMsg>(value.msgString);
        }
        catch (Exception e) {
            Debug.WriteLine(e.Message);
            msg = null;
        }
        
        return (value.msgInfo, msg);
    }

    /// <summary>
    /// 使用已经过滤的Json主体，获取MesgInfo
    /// </summary>
    private static MsgInfo? GetMesgInfo(this JsonElement json)
    {
        // 1. 提取核心必选字段（message_type、user_id、raw_message），缺失则返回null
        bool hasMessageType = json.TryGetProperty("message_type", out JsonElement messageType);
        bool hasUserId = json.TryGetProperty("user_id", out JsonElement userId);
        bool hasRawMessage = json.TryGetProperty("raw_message", out JsonElement rawMessage);

        // 必选字段缺失，直接返回null（避免后续空引用）
        if (!hasMessageType || !hasUserId || !hasRawMessage)
            return null;

        // 2. 初始化变量（避免默认空字符串""，用null更符合“未获取”的语义）
        string? userName = null;       // 基础昵称（sender.nickname）
        string? memberNickname = null; // 群名片（sender.card，仅群消息有效）
        string? role = null;           // 用户角色（仅群消息有效）
        long botQq = 0;                // 机器人QQ号
        string messageTypeStr = messageType.GetString() ?? string.Empty; // 消息类型（group/private等）

        // 3. 提取机器人QQ号（self_id）
        if (json.TryGetProperty("self_id", out JsonElement botQqValue))
        {
            botQq = botQqValue.GetInt64();
        }

        // 4. 提取发送者信息（sender节点）
        if (json.TryGetProperty("sender", out JsonElement sender))
        {
            // 4.1 优先获取基础昵称（nickname）
            if (sender.TryGetProperty("nickname", out JsonElement nicknameValue))
            {
                userName = nicknameValue.GetString();
            }

            // 4.2 仅“群消息”（message_type=group）时，处理群名片和角色
            if (messageTypeStr.Equals("group", StringComparison.OrdinalIgnoreCase))
            {
                // 获取群名片（card）
                if (sender.TryGetProperty("card", out JsonElement cardValue))
                {
                    memberNickname = cardValue.GetString();
                }

                // 核心逻辑：群名片为空（null/空字符串）时，使用基础昵称
                if (string.IsNullOrEmpty(memberNickname))
                {
                    memberNickname = userName;
                }

                // 获取用户角色（role）
                if (sender.TryGetProperty("role", out JsonElement roleValue))
                {
                    role = roleValue.GetString();
                }
            }
        }

        // 5. 解析消息链（判断是否@机器人、提取首个纯文本）
        bool isAtRobot = false;
        string firstPlain = string.Empty;
        if (json.TryGetProperty("message", out JsonElement message))
        {
            MessageChainResult messageChainResult = new MessageChainResult();
            messageChainResult.AnalysisMessageChain(botQq, message);
            firstPlain = messageChainResult.FirstPlain ?? string.Empty;
            isAtRobot = messageChainResult.IsAtRobot;
        }

        // 6. 提取消息时间（time）
        double messageTime = 0d;
        if (json.TryGetProperty("time", out JsonElement timeValue))
        {
            messageTime = timeValue.GetDouble();
        }

        // 7. 提取消息ID（message_id）
        long messageId = 0L;
        if (json.TryGetProperty("message_id", out JsonElement messageIdValue))
        {
            messageId = messageIdValue.GetInt64();
        }

        // 8. 提取群ID（group_id，非必选，仅群消息有值）
        string groupId = string.Empty;
        if (json.TryGetProperty("group_id", out JsonElement groupIdValue))
        {
            groupId = groupIdValue.GetInt64().ToString();
        }

        // 9. 构造并返回MsgInfo对象（统一处理null为空字符串，避免外部空引用）
        return new MsgInfo()
        {
            BotQQ = botQq,
            MessageContent = rawMessage.GetString() ?? string.Empty, // 避免null
            MessageType = messageTypeStr,
            UserId = userId.GetUInt64().ToString(),
            SenderId = userId.GetUInt64().ToString(),
            GroupId = groupId,
            UserName = userName ?? string.Empty, // 基础昵称：null→空字符串
            SenderMemberName = memberNickname ?? string.Empty, // 群名片：null→空字符串（已确保群消息时非空则用昵称）
            Time = messageTime,
            MessageId = messageId,
            Role = role ?? string.Empty, // 角色：null→空字符串
            IsAtRobot = isAtRobot,
            FirstPlain = firstPlain
        };
    }

    /// <summary>
    /// 判断数据是否是消息，返回json主体
    /// </summary>
    private static bool ValidData(string data, out JsonElement? json)
    {
        json = null;
        Utf8JsonReader read = new Utf8JsonReader(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(data)));
        //整个json对象
        //JsonDocument jsonObject = JsonSerializer.SerializeToDocument(data); //这样得到的是json字符串
        JsonDocument.TryParseValue(ref read, out JsonDocument? jsonObject);
        JsonElement? jsonRoot = jsonObject?.RootElement;
        try
        {
            //post_type
            if (jsonRoot is not null)
            {
                if (jsonRoot.Value.TryGetProperty("post_type", out JsonElement type))
                {//此属性决定是不是消息
                    if (type.ToString() == "message")
                    {
                        json = jsonRoot;
                        return true;
                    }

                    //上报自身消息
                    if (type.ToString() == "message_sent")
                    {
                        json = jsonRoot;
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;

                }
            }
            else
            {
                return false;

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + "\n" + e.StackTrace);
            return false;
        }
    }
    //关于分片数组
    //  1. 指向给定数组的内存
    //  2. 修改分片数组，源值也会更改
    //  3. 分片数组只拥有给定范围
}

