using System;
using System.ComponentModel;
using System.Linq;

namespace SysBot.Pokemon
{
    public class QQSettings
    {
        private const string Startup = nameof(Startup);
        private const string Operation = nameof(Operation);
        private const string Messages = nameof(Messages);
        public override string ToString() => "QQ Integration Settings";

        // Startup

        [Category(Startup), Description("Mirai机器人IP地址:端口号")]
        public string Address { get; set; } = string.Empty;

        [Category(Startup), Description("Mirai机器人的VerifyKey")]
        public string VerifyKey { get; set; } = string.Empty;

        [Category(Startup), Description("QQ机器人的号码")]
        public QQBotList QQ { get; set; } = QQBotList.Robot1097586712;

        [Category(Startup), Description("要发送消息的QQ群ID列表，用,号分隔")]
        public string GroupIdList { get; set; } = string.Empty;

        [Category(Startup), Description("测试机器人是否还在的消息")]
        public string AliveMsg { get; set; } = "你好";

        [Category(Operation), Description("打开机器人时发送的消息")]
        public string MessageStart { get; set; } = string.Empty;
    }

    public enum QQBotList : long
    {
        Robot1097586712 = 1097586712,
        Robot2670219618 = 2670219618,
        Robot2785910460 = 2785910460,
        Robot3263787871 = 3263787871,
        Robot3861914692 = 3861914692,
    }

      
}
