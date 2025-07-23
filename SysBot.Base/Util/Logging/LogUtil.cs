using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SysBot.Base;

/// <summary>
/// Logic wrapper to handle logging (via NLog).
/// </summary>
public static class LogUtil
{
    static LogUtil()
    {
        if (!LogConfig.LoggingEnabled)
            return;

        var config = new LoggingConfiguration();
        Directory.CreateDirectory("logs");
        var WorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath)!;
        var logfile = new FileTarget("logfile")
        {
            FileName = Path.Combine(WorkingDirectory, "logs", "SysBotLog.txt"),
            ConcurrentWrites = true,

            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveNumbering = ArchiveNumberingMode.Date,
            ArchiveFileName = Path.Combine(WorkingDirectory, "logs", "SysBotLog.{#}.txt"),
            ArchiveDateFormat = "yyyy-MM-dd",
            ArchiveAboveSize = 104857600, // 100MB (never)
            MaxArchiveFiles = LogConfig.MaxArchiveFiles,
            Encoding = Encoding.Unicode,
            WriteBom = true,
        };
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
        LogManager.Configuration = config;
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static void LogText(string message) => Logger.Log(LogLevel.Info, message);

    // hook in here if you want to forward the message elsewhere???
    public static readonly List<ILogForwarder> Forwarders = [];

    public static DateTime LastLogged { get; private set; } = DateTime.Now;

    public static void LogError(string message, string identity)
    {
        message = message.Replace("The distribution folder was not found. Please verify that it exists!", "没有找到分发文件夹，请确保它已经创建");
        message = message.Replace("Nothing to distribute for Empty Trade Queues!", "空的分发队列中没有找到要分发的宝可梦");
        Logger.Log(LogLevel.Error, $"{identity} {message}");
        Log(message, identity);
    }

    public static void LogInfo(string message, string identity)
    {
        message = message.Replace("Connecting to device", "正在连接到设备");
        message = message.Replace("Connected", "已连接");
        message = message.Replace("All bots have been issued a command to Start", "所有机器人都已发出启动命令");
        message = message.Replace("(FlexTrade) has been issued a command to Start", "灵活交换已发出启动命令");
        message = message.Replace("Starting all bots", "正在启动所有机器人");
        message = message.Replace("Disconnecting from device", "正在断开设备连接");
        message = message.Replace("Disconnected! Resetting Socket", "已断开连接！重置Socket");
        message = message.Replace("Disconnected", "已断开");
        message = message.Replace("All bots have been issued a command to Stop", "所有机器人都已发出停止命令");
        message = message.Replace("Removing", "正在从队列中移除");
        message = message.Replace("Surprise trading will fail; failed to load any compatible files", "魔法交换将会失败，没能加载到任何兼容的文件");
        Logger.Log(LogLevel.Info, $"{identity} {message}");
        Log(message, identity);
    }

    private static void Log(string message, string identity)
    {
        foreach (var fwd in Forwarders)
        {
            try
            {
                fwd.Forward(message, identity);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Failed to forward log from {identity} - {message}");
                Logger.Log(LogLevel.Error, ex);
            }
        }

        LastLogged = DateTime.Now;
    }

    public static void LogSafe(Exception exception, string identity)
    {
        Logger.Log(LogLevel.Error, $"Exception from {identity}:");
        Logger.Log(LogLevel.Error, exception);

        var err = exception.InnerException;
        while (err is not null)
        {
            Logger.Log(LogLevel.Error, err);
            err = err.InnerException;
        }
    }
}
