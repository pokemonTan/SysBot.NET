using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SysBot.Pokemon
{
    // 保留原枚举（作为下拉选择的选项）
    public enum QQBotList : long
    {
        Robot1097586712 = 1097586712,
        Robot2670219618 = 2670219618,
        Robot2785910460 = 2785910460,
        Robot3263787871 = 3263787871,
        Robot3861914692 = 3861914692,
    }

    public class QQSettings
    {
        private const string Startup = nameof(Startup);
        private const string Operation = nameof(Operation);
        private const string Messages = nameof(Messages);
        public override string ToString() => "QQ 集成设置";

        // 1. 用枚举作为下拉选择项（替代直接存储QQ号）
        [Category(Startup), Description("选择要使用的QQ机器人")]
        public QQBotList SelectedBot { get; set; } = QQBotList.Robot2670219618; // 默认选中一个机器人

        // 2. 根据选中的枚举自动获取对应的配置（HTTP/WS地址）
        [Browsable(false)] // 不在属性面板显示，供代码使用
        public QQBotConfig? SelectedBotConfig => QQBotConfigs.GetConfigByBot(SelectedBot);

        [Category(Startup), Description("要发送消息的QQ群ID列表，用,号分隔")]
        public string GroupIdList { get; set; } = string.Empty;

        [Category(Startup), Description("测试机器人是否还在的消息")]
        public string AliveMsg { get; set; } = "你好";

        [Category(Operation), Description("打开机器人时发送的消息")]
        public string MessageStart { get; set; } = string.Empty;
    }

    // 单个机器人的配置（包含枚举对应的QQ号、HTTP、WS地址）
    public class QQBotConfig
    {
        // 对应枚举的机器人标识（如 Robot2670219618）
        public QQBotList Bot { get; set; }

        // 机器人的QQ号（枚举的数值）
        public long QQ => (long)Bot;

        // HTTP接口地址
        public string HttpUrl { get; set; } = string.Empty;

        // HTTP的接口凭证
        public string HttpAccessToken { get; set; } = string.Empty;

        // WebSocket地址
        public string WsUrl { get; set; } = string.Empty;

        // WebSocket的接口凭证
        public string WsAccessToken { get; set; } = string.Empty;
    }

    // 枚举与配置的映射（核心：将枚举转换为可选择的配置项）
    public static class QQBotConfigs
    {
        // 存储枚举对应的配置列表（每个枚举值对应一个配置）
        public static readonly List<QQBotConfig> BotConfigs = new List<QQBotConfig>
        {
            new QQBotConfig
            {
                Bot = QQBotList.Robot1097586712,
                HttpUrl = "http://8.148.29.236:3000/",
                HttpAccessToken = "p2?4UpMx9r",
                WsUrl = "ws://8.148.29.236:3001/",
                WsAccessToken = "G5.Zk.IqX."
            },
            new QQBotConfig
            {
                Bot = QQBotList.Robot2670219618,
                HttpUrl = "http://120.48.195.144:3000/",
                HttpAccessToken = "p2?4UpMx9r",
                WsUrl = "ws://120.48.195.144:3001/",
                WsAccessToken = "G5.Zk.IqX."
            },
            new QQBotConfig
            {
                Bot = QQBotList.Robot2785910460,
                HttpUrl = "http://14.152.49.93:3000/",
                HttpAccessToken = "p2?4UpMx9r",
                WsUrl = "ws://14.152.49.93:3001/",
                WsAccessToken = "G5.Zk.IqX."
            },
            new QQBotConfig
            {
                Bot = QQBotList.Robot3263787871,
                HttpUrl = "http://8.148.65.77:3000/",
                HttpAccessToken = "p2?4UpMx9r",
                WsUrl = "ws://8.148.65.77:3001/",
                WsAccessToken = "G5.Zk.IqX."
            },
            new QQBotConfig
            {
                Bot = QQBotList.Robot3861914692,
                HttpUrl = "http://39.108.178.212:3000/",
                HttpAccessToken = "p2?4UpMx9r",
                WsUrl = "ws://39.108.178.212:3001/",
                WsAccessToken = "G5.Zk.IqX."
            }
        };

        // 根据枚举值获取对应的配置
        public static QQBotConfig? GetConfigByBot(QQBotList bot)
        {
            return BotConfigs.FirstOrDefault(c => c.Bot == bot);
        }
    }
}
