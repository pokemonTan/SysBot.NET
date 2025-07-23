using Manganese.Text;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.WebSockets;
using static NapCatScript.Core.MsgHandle.ReceiveMsg;
using static NapCatScript.Core.MsgHandle.Utils;
using System.Net.Http.Headers;
using Config = NapCatScript.Core.Services.Config;
using System.Reflection;
using System.Data;
using NapCatScript.Core;
using NapCatScript.Core.Model;
using NapCatScript.Core.MsgHandle;
using System.Threading;
using NapCatScript.Core.JsonFormat.Msgs;
using NapCatScript.Core.JsonFormat;
using System.Linq;
using System.Text;


namespace SysBot.Pokemon.QQ;
public class MiraiQQBot<T> where T : PKM, new()
{
    private static PokeTradeHub<T> Hub = default!;
    public static PokeBotRunner<T> Runner { get; private set; } = default!;
    internal static TradeQueueInfo<T> Info => Hub.Queues.Info;
    private readonly MiraiBot Client;

    internal static QQSettings Settings = default!;


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

    public MiraiQQBot(QQSettings settings, PokeTradeHub<T> hub, PokeBotRunner<T> runner)
    {
        Runner = runner;
        Settings = settings;
        Hub = hub;
        Client = new MiraiBot
        {
            Address = settings.Address,
            QQ = Convert.ToString((long)settings.QQ),
            VerifyKey = settings.VerifyKey
        };
        Console.WriteLine("111111111111111111");
        try
        {
            //接收消息 并将有效消息存放到NoPMesgList
            _ = Task.Run(Receive);

            //发送消息
            _ = Task.Run(Send);

            //心跳
            _ = Task.Run(LifeCycle);

            _reset.WaitOne();
        }
        catch (Exception e)
        {
            NapCatScript.Core.Services.Log.InstanceLog.Erro(e.Message + "\r\n" + e.StackTrace);
        }

        //        var modules = new List<IModule>()
        //        {
        //            new RemoteControlModule<T>(),
        //            new CommandModule<T>(),
        //            new FileModule<T>(),
        //            new PsModule<T>()
        //        };

        //        Common.GroupIdList = settings.GroupIdList.Split(',');

        //        //监听消息触发对应事件
        //        Client.MessageReceived.SubscribeGroupMessage(receiver => {
        //            if (!IsBotOrNotTargetGroup(receiver)) {
        //                modules.Raise(receiver);
        //            }
        //        });

        //        Task.Run(async () =>
        //        {
        //            try
        //            {
        //                await Client.LaunchAsync();

        //                if (!string.IsNullOrWhiteSpace(Settings.MessageStart))
        //                {
        //                    //向每个群通报一声机器人来了
        //                    for (int i = 0; i < Common.GroupIdList.Length; i++)
        //                    {
        //                        await MessageManager.SendGroupMessageAsync(Common.GroupIdList[i], Settings.MessageStart);
        //                    }
        //                    await Task.Delay(1_000).ConfigureAwait(false);
        //                }
        //                DateTime now = DateTime.Now;
        //                string formattedDateTime = now.ToString("MM月dd日HH:mm");
        //                if (typeof(T) == typeof(PK8))
        //                {
        //                    LogUtil.LogInfo("当前版本为剑盾", "测试");
        //                    for (int i = 0; i < Common.GroupIdList.Length; i++)
        //                    {
        //                        LogUtil.LogInfo($"修改[{Convert.ToString((long)settings.QQ)}]在[{Common.GroupIdList[i]}]的昵称", "测试");
        //                        await GroupManager.SetMemberInfoAsync(Convert.ToString((long)settings.QQ), Common.GroupIdList[i], $"剑盾机器人-{formattedDateTime}");
        //                    }
        //                }
        //                else if (typeof(T) == typeof(PB8))
        //                {
        //                    LogUtil.LogInfo("当前版本为晶灿钻石明亮珍珠", "测试");
        //                    for (int i = 0; i < Common.GroupIdList.Length; i++)
        //                    {
        //                        LogUtil.LogInfo($"修改[{Convert.ToString((long)settings.QQ)}]在[{Common.GroupIdList[i]}]的昵称", "测试");
        //                        await GroupManager.SetMemberInfoAsync(Convert.ToString((long)settings.QQ), Common.GroupIdList[i], $"珍钻机器人-{formattedDateTime}");
        //                    }
        //                }
        //                else if (typeof(T) == typeof(PA8))
        //                {
        //                    LogUtil.LogInfo("当前版本为阿尔宙斯", "测试");
        //                    for (int i = 0; i < Common.GroupIdList.Length; i++)
        //                    {
        //                        LogUtil.LogInfo($"修改[{Convert.ToString((long)settings.QQ)}]在[{Common.GroupIdList[i]}]的昵称", "测试");
        //                        await GroupManager.SetMemberInfoAsync(Convert.ToString((long)settings.QQ), Common.GroupIdList[i], $"阿尔宙斯机器人-{formattedDateTime}");
        //                    }
        //                }
        //                else if (typeof(T) == typeof(PK9))
        //                {
        //                    LogUtil.LogInfo("当前版本为朱紫", "测试");
        //                    for (int i = 0; i < Common.GroupIdList.Length; i++)
        //                    {
        //                        LogUtil.LogInfo($"修改[{Convert.ToString((long)settings.QQ)}]在[{Common.GroupIdList[i]}]的昵称", "测试");
        //                        await GroupManager.SetMemberInfoAsync(Convert.ToString((long)settings.QQ), Common.GroupIdList[i], $"朱紫机器人-{formattedDateTime}");
        //                    }
        //                }

        //                await Task.Delay(1_000).ConfigureAwait(false);
        //            }
        //#pragma warning disable CA1031 // Do not catch general exception types
        //            catch (Exception ex)
        //#pragma warning restore CA1031 // Do not catch general exception types
        //            {
        //                LogUtil.LogError(ex.Message, nameof(MiraiQQBot<T>));
        //            }
        //        });
    }

    public async static void SendGroupMessage(MessageChain mc, string groupId)
    {
        if (string.IsNullOrEmpty(groupId)) return;
        string messageId = await MessageManager.SendGroupMessageAsync(groupId, mc);
        if (messageId == "-1")
        {
            string plainText = mc.GetPlainMessage();
            // 使用 TrimStart 方法去掉开头的换行符
            if (plainText.StartsWith("\n"))
            {
                plainText = plainText.TrimStart('\n');
            }
            plainText = plainText.TrimStart();
            string qq = mc.GetAtMessage();
            string imageBase64 = mc.GetImageBase64Message();
            
            if (!string.IsNullOrEmpty(qq))
            {
                MessageChainBuilder builder = new MessageChainBuilder().Plain(plainText);
                if (!string.IsNullOrEmpty(imageBase64))
                {
                    string[] base64Array = imageBase64.Split('#', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var base64 in base64Array)
                    {
                        builder.ImageFromBase64(base64);
                    }
                    MessageChain finalMessageChain = builder.Build();
                    LogUtil.LogInfo($"发送群消息[{plainText}]失败，正在尝试发送临时会话图文消息给QQ[{qq}]", "测试");
                    await MessageManager.SendTempMessageAsync(qq, groupId, finalMessageChain);
                }
                else
                {
                    MessageChain finalMessageChain = builder.Build();
                    LogUtil.LogInfo($"发送群消息[{plainText}]失败，正在尝试发送临时会话消息给QQ[{qq}]", "测试");
                    await MessageManager.SendTempMessageAsync(qq, groupId, finalMessageChain);
                }
            }
            else
            {
                LogUtil.LogInfo($"发送群消息[{plainText}]失败，没有找到QQ[{qq}]，发送临时消息失败", "测试");
            }
        }
    }

    public async static void SendGroupOrTempMessage(MessageChain mc, string qq, string groupId)
    {
        if (string.IsNullOrEmpty(groupId)) return;
        string messageId = await MessageManager.SendGroupMessageAsync(groupId, mc);
        if(messageId == "-1")
        {
            string plainText = mc.GetPlainMessage();
            string imageBase64 = mc.GetImageBase64Message();
            MessageChainBuilder builder = new MessageChainBuilder().Plain(plainText);
            if (!string.IsNullOrEmpty(imageBase64))
            {
                string[] base64Array = imageBase64.Split('#', StringSplitOptions.RemoveEmptyEntries);
                foreach (var base64 in base64Array)
                {
                    builder.ImageFromBase64(base64);
                }
                MessageChain finalMessageChain = builder.Build();
                LogUtil.LogInfo($"发送群消息[{plainText}]失败，正在尝试发送临时会话图文消息给QQ[{qq}]", "测试");
                await MessageManager.SendTempMessageAsync(qq, groupId, finalMessageChain);
            }
            else
            {
                MessageChain finalMessageChain = builder.Build();
                LogUtil.LogInfo($"发送群消息[{plainText}]失败，正在尝试发送临时会话消息给QQ[{qq}]", "测试");
                await MessageManager.SendTempMessageAsync(qq, groupId, finalMessageChain);
            }
        }
    }

    public async static void SendFriendMessage(string friendId, MessageChain mc)
    {
        if (string.IsNullOrEmpty(friendId)) return;
        await MessageManager.SendFriendMessageAsync(friendId, mc);
    }

    public async static void SendTempMessage(string friendId, string groupId, MessageChain mc)
    {
        if (string.IsNullOrEmpty(friendId) || string.IsNullOrEmpty(groupId)) return;
        await MessageManager.SendTempMessageAsync(friendId, groupId, mc);
    }

    /// <summary>
    /// 判断元素是否在数组中
    /// </summary>
    /// <param name="str">要查找的字符串元素</param>
    /// <param name="strArray">字符串数组</param>
    /// <returns></returns>
    public static bool inArray(string str, string[] strArray)
    {
        for (int i = 0; i < strArray.Length; i++)
        {
            if (str.Equals(strArray[i]))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断发送方是否为机器人或者不在本群
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns></returns>
    private bool IsBotOrNotTargetGroup(GroupMessageReceiver receiver)
    {
        return !inArray(receiver.Sender.Group.Id, Common.GroupIdList) || receiver.Sender.Id == Settings.QQ.ToString();
    }


    /// <summary>
    /// 建立链接并接受消息
    /// </summary>
    private static async void Receive()
    {
        await 建立连接(Socket, SocketUri);
        while (true)
        {
            await Task.Delay(1);
            try
            {
                MsgInfo? mesg = await Socket.Receive(CTokrn); //收到的消息
                if (mesg is not null)
                {
                    NoPMesgList.Add(mesg);
                    //Console.WriteLine(mesg);
                }
            }
            catch (Exception e)
            {
                NapCatScript.Core.Services.Log.InstanceLog.Erro("消息接收发生错误: ", e.Message, e.StackTrace);
            }

        }
    }

    /// <summary>
    /// 每收到消息 就发送
    /// </summary>
    private static async void Send()
    {
        Send sned = new Send(HttpUri);
        while (true)
        {
            await Task.Delay(1);
            if (NoPMesgList.Count <= 0)
                continue;
            MsgInfo mesg = NoPMesgList.First();
            //interfaceTest(sned); // Test
            NoPMesgList.RemoveAt(0);
            NapCatScript.Core.Services.Log.InstanceLog.Info(mesg);
            //MService.SetAsync(mesg);
            foreach (var pType in Plugins)
            {
                try
                {
                    _ = pType.Run(mesg, HttpUri);
                }
                catch (Exception e)
                {
                    NapCatScript.Core.Services.Log.InstanceLog.Erro($"插件:{pType.GetType().FullName}", e.Message, e.StackTrace);
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
        foreach (var ie in ies)
        {
            sbuilder.Append(string.Join(", ", ie) + " | ");
            //foreach (var str in ie) {
            //    sbuilder.Append(str + ", ");
            //}
        }
    }

    private static async Task 建立连接(ClientWebSocket socket, string uri)
    {
        try
        {
            await socket.ConnectAsync(new Uri(uri), CTokrn);
            Console.WriteLine("连接成功");
            IsConnection = true;
        }
        catch (Exception e)
        {
            NapCatScript.Core.Services.Log.InstanceLog.Erro("建立连接: 请检查URI是否有效，服务是否正常可访问", e.Message, e.StackTrace);
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
        try
        {
            Socket.Abort();
            //await Socket.CloseAsync(WebSocketCloseStatus.Empty, "", CTokrn);
            Socket = new ClientWebSocket();
            await Socket.ConnectAsync(new Uri(uri), CTokrn);
            return true;
        }
        catch (Exception e)
        {
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
        while (true)
        {
            //1000是一秒
            await Task.Delay(1000); //500秒
            seconds++;
            if (!IsConnection)
                continue;
            //DateTime.Now.Ticks
            if (seconds % 9 == 0 && (Socket.State == WebSocketState.Closed || Socket.State == WebSocketState.CloseSent || Socket.State == WebSocketState.CloseReceived || Socket.State == WebSocketState.Aborted))
            {
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
