using NapCatScript.Core.MsgHandle;
using NapCatScript.Core.Services;
using NapCatScript.Start;
using static NapCatScript.Core.Services.Config;
namespace NapCatScript.Core;
public static class CoreConfigValueAndObject
{
    /// <summary>
    /// 用户配置的Uri，这个决定WebSocket链接
    /// </summary>
    public static string SocketUri { get; private set; }
    /// <summary>
    /// 基础请求uri http://127.0.0.1:6666
    /// </summary>
    public static string HttpUri { get; set; }
    public static string RootId { get; set; }
    public static string BotId { get; set; }
    public static ClientWebSocket Socket { get; private set; } = new ClientWebSocket();
    public static CancellationToken CTokrn { get; } = new CancellationToken();
    public static Send SendObject { get; private set; }
    public static List<PluginType> Plugins { get; } = [];
    static CoreConfigValueAndObject()
    {
        InstanceLog.Info("加载核心配置!");
        //我的服务器
        //string? useUri = "ws://39.108.178.212:3001/?access_token=G5.Zk.IqX.";
        //string? httpUri = "http://39.108.178.212:3000/";
        //文件服务器
        //string? useUri = "ws://14.152.49.93:3001/?access_token=G5.Zk.IqX.";
        //string? httpUri = "http://14.152.49.93:3000/";
        //线上数据库服务器
        string? useUri = "ws://120.48.195.144:3001/?access_token=G5.Zk.IqX.";
        string? httpUri = "http://120.48.195.144:3000/";
        string? httpAccessToken = "p2?4UpMx9r";
        BotId = "3861914692";
        if (string.IsNullOrEmpty(useUri) || string.IsNullOrEmpty(httpUri)) {
            InstanceLog.Waring("配置文件已生成，请检查Uri配置");
            Console.ReadLine();
            Environment.Exit(0);
        }
        SocketUri = useUri;
        SocketUri ??= "1";
        HttpUri = httpUri;
        RootId = "1";
        SendObject = new Send(HttpUri, httpAccessToken);

        PluginLoad.LoadPlugin(Plugins);
    }
}
