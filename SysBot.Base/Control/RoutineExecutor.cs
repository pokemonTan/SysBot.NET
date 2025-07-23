using System;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Base;

/// <summary>
/// Commands a Bot to a perform a routine asynchronously.
/// </summary>
public abstract class RoutineExecutor<T>(IConsoleBotManaged<IConsoleConnection, IConsoleConnectionAsync> Config)
    : IRoutineExecutor
    where T : class, IConsoleBotConfig
{
    public readonly IConsoleConnectionAsync Connection = Config.CreateAsynchronous();
    public readonly T Config = (T)Config;

    public string LastLogged { get; private set; } = "Not Started";
    public DateTime LastTime { get; private set; } = DateTime.Now;

    public void ReportStatus() => LastTime = DateTime.Now;

    public abstract string GetSummary();

    public void Log(string message)
    {
        message = message.Replace("Nothing to check, waiting for new users.", "队列为空,等待用户操作");
        message = message.Replace("Still searching, resetting bot position.", "仍在搜寻中,重置机器人位置");
        message = message.Replace("Opening Y-Comm menu.", "打开Y连接菜单");
        message = message.Replace("Reconnecting to Y-Comm", "重新连接Y连接菜单");
        message = message.Replace("Selecting Link Trade.", "正在选择连接交换");
        message = message.Replace("Selecting Link Trade code.", "选择连接交换-设置密码");
        message = message.Replace("Entering Link Trade code", "正在输入连接交换密码");
        message = message.Replace("Found Link Trade partner", "找到连接交换对象");
        message = message.Replace("User did not complete the trade.", "玩家没有完成交换");
        message = message.Replace("User completed the trade.", "玩家已完成交换");
        message = message.Replace("Speaking to Simona to start a trade", "跟佐雯对话开始交换");
        message = message.Replace("Waiting for trainer", "正在搜寻对象");
        message = message.Replace("Resetting bot position.", "重置机器人位置");
        message = message.Replace("Identifying trainer data of the host console.", "正在识别主机的初训家信息");
        message = message.Replace("No task assigned. Waiting for new task assignment.", "没有任务分配,等待新的任务分配 ");
        message = message.Replace("Found a user talking to us!", "发现有人跟我们谈话");
        message = message.Replace("Attempting to open the Y menu", "尝试打开Y菜单");
        message = message.Replace("Entering the Union Room.", "进入公共房间");
        message = message.Replace("Selecting Link Code room.", "选择连接密码房间");
        message = message.Replace("Connecting to internet.", "正在连接到互联网");
        message = message.Replace("Saving the game.", "保存游戏中");
        message = message.Replace("Trying to get out of the Union Room", "尝试回到公共房间");
        message = message.Replace("Exiting box", "关闭盒子");
        message = message.Replace("Entering the box", "打开盒子");
        message = message.Replace("Exiting Union Room", "退出公共房间");
        message = message.Replace("Error detected, restarting the game", "检测到有错误,正在重启游戏");
        message = message.Replace("Closed out of the game", "已关闭游戏");
        message = message.Replace("Restarting the game", "正在重启游戏中");
        message = message.Replace("Back in the overworld", "回到游戏界面");
        message = message.Replace("Soft ban detected, unbanning", "软禁令检测,没有被ban");
        message = message.Replace("Caching session offsets...", "正在缓存会话偏移量");
        message = message.Replace("Starting next", "正在开始下一个");
        message = message.Replace("Starting trade", "正在开始交换");
        message = message.Replace("for this session", "给这个会话");
        message = message.Replace("Bot Trade. Getting data", "机器人交换,获取数据中");
        message = message.Replace("Starting main", "正在开始主");
        message = message.Replace("Ending", "正在结束");
        message = message.Replace("loop", "循环");
        message = message.Replace("Initializing connection with console", "初始化控制台连接");
        message = message.Replace("Detaching on startup", "在启动的时候从机器人中删除虚拟手柄,允许物理手柄手动控制");
        message = message.Replace("Detaching controllers on routine exit", "在常规退出中删除虚拟手柄,允许物理手柄手动控制");
        message = message.Replace("Disconnected.", "已断开");
        message = message.Replace("identified as", "识别为");
        message = message.Replace("using ", "使用");
        message = message.Replace("Unexpected behavior, recovering position", "意外行为,恢复位置");
        message = message.Replace("Grabbing trainer data of host console", "正在抓取主机控制台的训练家信息");
        message = message.Replace("Turning off screen", "正在关闭屏幕以节省电力");
        message = message.Replace("Not Started", "没有已启动的");
        message = message.Replace("Failed to recover to overworld, rebooting the game", "恢复到野外失败，正在重启游戏");
        message = message.Replace("Failed to recover to Poké Portal.", "恢复到宝可梦入口站失败");
        message = message.Replace("Reorienting to Poké Portal.", "正在重定向到宝可梦入口站");
        Connection.Log(message);
        LastLogged = message;
        LastTime = DateTime.Now;
    }

    /// <summary>
    /// Connects to the console, then runs the bot.
    /// </summary>
    /// <param name="token">Cancel this token to have the bot stop looping.</param>
    public async Task RunAsync(CancellationToken token)
    {
        Connection.Connect();
        Log("Initializing connection with console...");
        await InitialStartup(token).ConfigureAwait(false);
        await MainLoop(token).ConfigureAwait(false);
        Connection.Disconnect();
    }

    public abstract Task MainLoop(CancellationToken token);
    public abstract Task InitialStartup(CancellationToken token);
    public abstract void SoftStop();
    public abstract Task HardStop();
}
