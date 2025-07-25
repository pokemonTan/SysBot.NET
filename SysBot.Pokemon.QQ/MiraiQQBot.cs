using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using static NapCatScript.Core.MsgHandle.ReceiveMsg;
using static NapCatScript.Core.MsgHandle.Utils;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NapCatScript.Core.JsonFormat.Msgs;
using NapCatScript.Core.JsonFormat;
using NapCatScript.Core.Model;
using NapCatScript.Core.MsgHandle;
using NapCatScript.Core;
using System.Data;

namespace SysBot.Pokemon.QQ;

public class MiraiQQBot<T> where T : PKM, new()
{
    private static PokeTradeHub<T> Hub = default!;
    public static PokeBotRunner<T> Runner { get; private set; } = default!;
    internal static TradeQueueInfo<T> Info => Hub.Queues.Info;
    internal static QQSettings Settings = default!;

    public static string SocketUri => CoreConfigValueAndObject.SocketUri;
    public static string HttpUri => CoreConfigValueAndObject.HttpUri;
    public static string RootId => CoreConfigValueAndObject.RootId;
    public static string BotId => CoreConfigValueAndObject.BotId;
    public static ClientWebSocket Socket { get; private set; } = new ClientWebSocket();
    public static CancellationTokenSource Cts { get; } = new CancellationTokenSource();
    public static Send SendObject => CoreConfigValueAndObject.SendObject;
    public static List<MsgInfo> NoPMesgList { get; } = new List<MsgInfo>();
    public static Random Rand { get; } = new Random();
    public static List<PluginType> Plugins => CoreConfigValueAndObject.Plugins;
    public static long LifeTime { get; private set; } = 0;
    public static long OldLifeTime { get; private set; } = 0;
    public static long Seconds { get; private set; } = 0;
    public static ConnectionState State { get; private set; } = ConnectionState.Open;

    private readonly TaskCompletionSource<bool> _reset = new TaskCompletionSource<bool>();
    private static readonly object _msgListLock = new object();

    public MiraiQQBot(QQSettings settings, PokeTradeHub<T> hub, PokeBotRunner<T> runner)
    {
        Runner = runner;
        Settings = settings;
        Hub = hub;
        _ = InitializeAsync(); // 启动异步初始化，不阻塞UI
    }

    private async Task InitializeAsync()
    {
        try
        {
            var receiveTask = Task.Run(ReceiveAsync, Cts.Token);
            var sendTask = Task.Run(SendAsync, Cts.Token);
            var lifeCycleTask = Task.Run(LifeCycleAsync, Cts.Token);
            await Task.WhenAll(receiveTask, sendTask, lifeCycleTask);
        }
        catch (Exception e)
        {
            LogUtil.LogText($"初始化失败: {e.Message}\r\n{e.StackTrace}");
            _reset.TrySetException(e);
        }
        finally
        {
            _reset.TrySetResult(true);
        }
    }

    public Task WaitForCompletionAsync() => _reset.Task;

    public static bool InArray(string str, string[] strArray)
    {
        return Array.IndexOf(strArray, str) != -1;
    }

    // 修正 ReceiveAsync 方法中的消息接收逻辑
    private static async Task ReceiveAsync()
    {
        try
        {
            await ConnectToWebsocket(Socket, SocketUri);
            State = ConnectionState.Open;

            while (!Cts.Token.IsCancellationRequested)
            {
                try
                {
                    // 关键修正：为 Receive 方法传递 CancellationToken 参数（Cts.Token）
                    MsgInfo? mesg = await ReceiveMsg.Receive(Socket, Cts.Token);
                    if (mesg is not null)
                    {
                        lock (_msgListLock)
                            NoPMesgList.Add(mesg);
                        if(mesg.MessageType == "group" && InArray(mesg.GroupId, Settings.GroupIdList.Split(",")))
                        {
                            if (!string.IsNullOrEmpty(mesg.MessageContent))
                            {
                                Debug.WriteLine($"{mesg.GroupId}-{mesg.UserId}({mesg.SenderMemberName}): {mesg.MessageContent}");

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (Cts.Token.IsCancellationRequested)
                        break;
                    LogUtil.LogError(e.Message, "消息接收错误");
                    await Task.Delay(1000, Cts.Token);
                }
            }
        }
        finally
        {
            State = ConnectionState.Closed;
            // 关闭连接时也使用 Cts.Token
            await Socket.CloseAsync((WebSocketCloseStatus)1006, "接收任务结束", Cts.Token);
        }
    }

    private static async Task SendAsync()
    {
        var sender = new Send(HttpUri);
        while (!Cts.Token.IsCancellationRequested)
        {
            try
            {
                MsgInfo? mesg = null;
                lock (_msgListLock)
                {
                    if (NoPMesgList.Count > 0)
                    {
                        mesg = NoPMesgList.First();
                        NoPMesgList.RemoveAt(0);
                    }
                }

                if (mesg is not null)
                {
                    NapCatScript.Core.Services.Log.InstanceLog.Info(mesg);
                    var pluginTasks = Plugins.Select(p => Task.Run(() => p.Run(mesg, HttpUri), Cts.Token));
                    await Task.WhenAll(pluginTasks);
                }
                else
                {
                    await Task.Delay(10, Cts.Token);
                }
            }
            catch (Exception e)
            {
                if (Cts.Token.IsCancellationRequested)
                    break;
                LogUtil.LogError(e.Message, "消息发送错误");
                await Task.Delay(1000, Cts.Token);
            }
        }
    }

    private static async Task LifeCycleAsync()
    {
        while (!Cts.Token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, Cts.Token);
                Seconds++;

                if (!IsConnectionActive())
                {
                    LogUtil.LogText("连接断开，尝试重连...");

                    if (await ReConnectAsync(SocketUri))
                    {
                        State = ConnectionState.Open;
                        LogUtil.LogText("重连成功");
                    }
                    else
                    {
                        State = ConnectionState.Closed;
                        LogUtil.LogText("重连失败，10秒后重试");
                        await Task.Delay(10000, Cts.Token);
                    }
                }
                else if (Seconds % 30 == 0)
                {
                    Debug.WriteLine($"心跳正常，当前状态: {State}");
                }
            }
            catch (Exception e)
            {
                if (Cts.Token.IsCancellationRequested)
                    break;
                LogUtil.LogError(e.Message, "心跳检测错误");
            }
        }
    }

    private static bool IsConnectionActive()
    {
        return Socket.State == WebSocketState.Open && State == ConnectionState.Open;
    }

    private static async Task ConnectToWebsocket(ClientWebSocket socket, string uri)
    {
        if (socket.State == WebSocketState.Open)
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "重新连接", Cts.Token);

        try
        {
            await socket.ConnectAsync(new Uri(uri), Cts.Token);
            Debug.WriteLine("WebSocket连接成功");
            State = ConnectionState.Open;
        }
        catch (Exception e)
        {
            LogUtil.LogError(e.Message, "WebSocket连接失败: 请检查URI和服务状态");
            State = ConnectionState.Closed;
            throw;
        }
    }

    private static async Task<bool> ReConnectAsync(string uri)
    {
        try
        {
            if (Socket.State != WebSocketState.Closed)
                // 修正：使用1006作为异常关闭状态码
                await Socket.CloseAsync((WebSocketCloseStatus)1006, "重连", Cts.Token);

            Socket = new ClientWebSocket();
            await ConnectToWebsocket(Socket, uri);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void SetLifeTime(long time)
    {
        OldLifeTime = LifeTime;
        LifeTime = time;
    }

    private static void InterfaceTest(Send send)
    {
        var contents = new List<MsgJson>
        {
            new AtJson("qqid"),
            new TextJson("sendMsgText"),
        };
        send.SendMsg("qqid", MsgTo.user, contents);
    }

    public static void AddString(StringBuilder sbuilder, params IEnumerable<string>[] ies)
    {
        foreach (var ie in ies)
            sbuilder.AppendJoin(", ", ie).Append(" | ");
    }

    public void Dispose()
    {
        Cts.Cancel();
        _reset.TrySetResult(true);
        Socket.Dispose();
        Cts.Dispose();
    }
}
