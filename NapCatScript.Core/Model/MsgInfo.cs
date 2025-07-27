namespace NapCatScript.Core.Model;
/// <summary>
/// 数据信息 仅适用于接收消息 产生于 <see cref="MsgHandle.ReceiveMsg"/>
/// </summary>
public class MsgInfo
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement, SQLite.Column("Key")]
    public long Key { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    [SQLite.Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 机器人QQ
    /// </summary>
    [SQLite.Column("bot_qq")]
    public long BotQQ { get; set; }

    /// <summary>
    /// 群组ID
    /// </summary>
    [SQLite.Column("group_id")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// 用户名称
    /// </summary>
    [SQLite.Column("user_name")]
    public string UserName { get; set; } = "";

    /// <summary>
    /// 消息内容
    /// </summary>
    [SQLite.Column("message_content")]
    public string MessageContent { get; set; } = string.Empty;


    /// <summary>
    /// 去掉空格后的消息内容
    /// </summary>
    [SQLite.Column("first_plain")]
    public string FirstPlain { get; set; } = string.Empty;

    /// <summary>
    /// 群员权限
    /// </summary>
    [SQLite.Column("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 群员昵称
    /// </summary>
    [SQLite.Column("card")]
    public string SenderMemberName { get; set; } = string.Empty;

    /// <summary>
    /// QQ昵称
    /// </summary>
    [SQLite.Column("nickname")]
    public string SenderNickName { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型
    /// </summary>
    [SQLite.Column("message_type")]
    public string MessageType { get; set; } = string.Empty;

    [SQLite.Column("time")]
    public double Time { get; set; }
    
    [SQLite.Column("message_id")]
    public long MessageId { get; set; }

    /// <summary>
    /// 是否有@机器人
    /// </summary>
    [SQLite.Column("is_at_robot")]
    public bool IsAtRobot { get; set; } = false;

    public string GetId()
    {
        //if (UserId != string.Empty) return UserId;
        //else return GroupId;
        if (MessageType == "group")
            return GroupId.ToString();
        else
            return UserId.ToString();
    }

    public override string ToString()
    {

        return
            "\r\n------------------------------------------------\r\n" +
            UserId + $": {UserName} :" +
            MessageContent + " ," +
            MessageType + "\r\n" +
            "------------------------------------------------\r\n";
    }
}
