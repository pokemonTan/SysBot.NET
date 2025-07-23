﻿using System.Net.WebSockets;
using static NapCatScript.Core.MsgHandle.ReceiveMsg;
using static NapCatScript.Core.MsgHandle.Utils;
using System.Net.Http.Headers;
using Config = NapCatScript.Core.Services.Config;
using System.Reflection;
using System.Data;
using NapCatScript.Core;
using System.Threading.Tasks;

namespace NapCatScript.Start;

/// <summary>
/// 
/// </summary>
public class Main_
{
    /// <summary>
    /// 用户配置的Uri，这个决定WebSocket链接
    /// </summary>
    public static string SocketUri => CoreConfigValueAndObject.SocketUri;
    /// <summary>
    /// 基础请求uri http://127.0.0.1:6666
    /// </summary>
    public static string HttpUri => CoreConfigValueAndObject.HttpUri;
    public static string RootId /*{ get; set; } = "";*/ => CoreConfigValueAndObject.RootId;
    public static string BotId /*{ get; set; } = "";*/ => CoreConfigValueAndObject.BotId;
    public static ClientWebSocket Socket { get; private set; } = new ClientWebSocket();
    public static CancellationToken CTokrn { get; } = new CancellationToken();
    public static Send SendObject => CoreConfigValueAndObject.SendObject;
    public static List<MsgInfo> NoPMesgList { get; } = [];
    public static bool IsConnection = false;
    public static Random rand = new Random();
    public static List<PluginType> Plugins => CoreConfigValueAndObject.Plugins;
    public static long lifeTime = 0;
    public static long oldLifeTime = 0;
    public static long seconds = 0;

    /// <summary>
    /// ws状态
    /// </summary>
    public static ConnectionState state = ConnectionState.Open;

    private static ManualResetEvent _reset = new ManualResetEvent(false);
    static void Main(string[] args)
    {
        try {
            //接收消息 并将有效消息存放到NoPMesgList
            _ = Task.Run(Receive);

            //发送消息
            _ = Task.Run(Send);

            //心跳
            _ = Task.Run(LifeCycle);

            _reset.WaitOne();
        } catch (Exception e) {
            InstanceLog.Erro(e.Message + "\r\n" + e.StackTrace);
        }
    }

    /// <summary>
    /// 建立链接并接受消息
    /// </summary>
    private static async void Receive()
    {
        await 建立连接(Socket, SocketUri);
        while (true) {
            await Task.Delay(1);
            try {
                MsgInfo? mesg = await Socket.Receive(CTokrn); //收到的消息
                if (mesg is not null) {
                    NoPMesgList.Add(mesg);
                    //Console.WriteLine(mesg);
                }
            } catch (Exception e) {
                InstanceLog.Erro("消息接收发生错误: ", e.Message, e.StackTrace);
            }

        }
    }

    /// <summary>
    /// 每收到消息 就发送
    /// </summary>
    private static async void Send()
    {
        Send sned = new Send(HttpUri);
        while (true) {
            await Task.Delay(1);
            if (NoPMesgList.Count <= 0)
                continue;
            MsgInfo mesg = NoPMesgList.First();
            //interfaceTest(sned); // Test
            NoPMesgList.RemoveAt(0);
            InstanceLog.Info(mesg);
            //MService.SetAsync(mesg);
            foreach (var pType in Plugins) {
                try {
                    _ = pType.Run(mesg, HttpUri);
                } catch (Exception e) {
                    InstanceLog.Erro($"插件:{pType.GetType().FullName}", e.Message, e.StackTrace);
                }
            }
        }
    }

    private static void interfaceTest(Send send)
    {
        var contents = new List<MsgJson>()
        {
            new AtJson("qqid"),
            new TextJson("sendMsgText"),
        };
        send.SendMsg("qqid", MsgTo.user, contents);
    }

    /// <summary>
    /// 往sbuilder添加字符串
    /// </summary>
    public static void AddString(StringBuilder sbuilder, params IEnumerable<string>[] ies)
    {
        //string.Join(", ", ies);
        foreach (var ie in ies) {
            sbuilder.Append(string.Join(", ", ie) + " | ");
            //foreach (var str in ie) {
            //    sbuilder.Append(str + ", ");
            //}
        }
    }

    private static async Task 建立连接(ClientWebSocket socket, string uri)
    {
        try {
            await socket.ConnectAsync(new Uri(uri), CTokrn);
            Console.WriteLine("连接成功");
            IsConnection = true;
        } catch (Exception e){
            InstanceLog.Erro("建立连接: 请检查URI是否有效，服务是否正常可访问", e.Message, e.StackTrace);
            Environment.Exit(0);
        }
        //SetConf(URI, uri);
    }

    /// <summary>
    /// 重新链接
    /// </summary>
    /// <returns>成功返回true</returns>
    private static async Task<bool> ReConnect(string uri)
    {
        try {
            Socket.Abort();
            //await Socket.CloseAsync(WebSocketCloseStatus.Empty, "", CTokrn);
            Socket = new ClientWebSocket();
            await Socket.ConnectAsync(new Uri(uri), CTokrn);
            return true;
        } catch (Exception e) {
            return false;
        }
    }


    /// <summary>
    /// 设置心跳时间
    /// </summary>
    private static void SetLifeTime(long time)
    {
        oldLifeTime = lifeTime;
        lifeTime = time;
    }

    /// <summary>
    /// 心跳
    /// </summary>
    private static async Task LifeCycle()
    {
        while (true) {
            //1000是一秒
            await Task.Delay(1000); //500秒
            seconds++;
            if (!IsConnection)
                continue;
            //DateTime.Now.Ticks
            if(seconds % 9 == 0 && (Socket.State == WebSocketState.Closed || Socket.State == WebSocketState.CloseSent || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.Aborted)) {
                var temp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("重新连接");
                Console.ForegroundColor = temp;
                if (await ReConnect(SocketUri))
                    state = ConnectionState.Open;
                else
                    state = ConnectionState.Closed;
            }
        }
    }
}
