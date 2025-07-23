using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PKHeX.Core;
using SysBot.Base;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Pokemon.QQ;

public class RemoteControlModule<T> : IModule where T : PKM, new()
{
    private string receiverQQ = "964954800";
    private string receiverGroup = "612566288";
    public bool? IsEnable { get; set; } = true;

    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();
        QQSettings settings = MiraiQQBot<T>.Settings;

        var text = receiver.MessageChain.OfType<PlainMessage>()?.FirstOrDefault()?.Text ?? "";
        if (string.IsNullOrWhiteSpace(text)) return;
        var qq = receiver.Sender.Id;
        var nickName = receiver.Sender.Name;
        receiverGroup = receiver.GroupId;
        if(qq != receiverQQ) { return; }
        if(text == "截图")
        {
            await CaptureScreenAsync().ConfigureAwait(false);
            return;
        }
        else if (text == "关闭屏幕")
        {
            await SetScreenOffAsync("192.168.1.105").ConfigureAwait(false);
        }
        else if (text == "打开屏幕" || text == "点亮屏幕" || text == "开启屏幕")
        {
            await SetScreenOnAsync("192.168.1.105").ConfigureAwait(false);
        }
        else if (text == "按键A")
        {
            await ClickAsync(SwitchButton.A).ConfigureAwait(false);
        }
        else if (text == "按键B")
        {
            await ClickAsync(SwitchButton.B).ConfigureAwait(false);
        }
        else if (text == "按键X")
        {
            await ClickAsync(SwitchButton.X).ConfigureAwait(false);
        }
        else if (text == "按键Y")
        {
            await ClickAsync(SwitchButton.Y).ConfigureAwait(false);
        }
        else if (text == "按键+")
        {
            await ClickAsync(SwitchButton.PLUS).ConfigureAwait(false);
        }
        else if (text == "按键-")
        {
            await ClickAsync(SwitchButton.MINUS).ConfigureAwait(false);
        }
        else if (text == "按键上" || text == "按键↑")
        {
            await ClickAsync(SwitchButton.DUP).ConfigureAwait(false);
        }
        else if (text == "按键下" || text == "按键↓")
        {
            await ClickAsync(SwitchButton.DDOWN).ConfigureAwait(false);
        }
        else if (text == "按键左" || text == "按键←")
        {
            await ClickAsync(SwitchButton.DLEFT).ConfigureAwait(false);
        }
        else if (text == "按键右" || text == "按键→")
        {
            await ClickAsync(SwitchButton.DRIGHT).ConfigureAwait(false);
        }
        else if (text == "按键L")
        {
            await ClickAsync(SwitchButton.L).ConfigureAwait(false);
        }
        else if (text == "按键R")
        {
            await ClickAsync(SwitchButton.R).ConfigureAwait(false);
        }
        else if (text == "按键HOME")
        {
            await ClickAsync(SwitchButton.HOME).ConfigureAwait(false);
        }
        else if (text == "按键ZL")
        {
            await ClickAsync(SwitchButton.ZL).ConfigureAwait(false);
        }
        else if (text == "按键ZR")
        {
            await ClickAsync(SwitchButton.ZR).ConfigureAwait(false);
        }
    }

    public async Task CaptureScreenAsync()
    {
        var bot = MiraiQQBot<T>.Runner.Bots.Find(z => IsRemoteControlBot(z.Bot));
        if (bot == null)
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"没有机器人可用于执行命令:[截图]").Build());
            return;
        }

        await CaptureScreenAsyncImpl(bot).ConfigureAwait(false);
    }

    public async Task ClickAsync(SwitchButton b)
    {
        var bot = MiraiQQBot<T>.Runner.Bots.Find(z => IsRemoteControlBot(z.Bot));
        if (bot == null)
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"没有机器人可用于执行命令:[{b}]").Build());
            return;
        }

        await ClickAsyncImpl(b, bot).ConfigureAwait(false);
    }

    public async Task ClickAsync(string ip, SwitchButton b)
    {
        var bot = MiraiQQBot<T>.Runner.GetBot(ip);
        if (bot == null)
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"没有机器人可用于执行命令:[{b}]").Build());
            return;
        }

        await ClickAsyncImpl(b, bot).ConfigureAwait(false);
    }

    public async Task SetStickAsync(SwitchStick s, short x, short y, ushort ms = 1_000)
    {
        var bot = MiraiQQBot<T>.Runner.Bots.Find(z => IsRemoteControlBot(z.Bot));
        if (bot == null)
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"没有机器人可用于执行命令:[{s}]").Build());
            return;
        }

        await SetStickAsyncImpl(s, x, y, ms, bot).ConfigureAwait(false);
    }

    public async Task SetStickAsync(string ip, SwitchStick s, short x, short y, ushort ms = 1_000)
    {
        var bot = MiraiQQBot<T>.Runner.GetBot(ip);
        if (bot == null)
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"没有机器人属于IP地址:[{ip}]").Build());
            return;
        }

        await SetStickAsyncImpl(s, x, y, ms, bot).ConfigureAwait(false);
    }

    public Task SetScreenOnAsync(string ip)
    {
        return SetScreen(true, ip);
    }

    public Task SetScreenOffAsync(string ip)
    {
        return SetScreen(false, ip);
    }

    private async Task SetScreen(bool on, string ip)
    {
        var bot = GetBot(ip);
        if (bot == null)
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"没有机器人属于IP地址:[{ip}]").Build());
            return;
        }

        var b = bot.Bot;
        var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
        await b.Connection.SendAsync(SwitchCommand.SetScreen(on ? ScreenState.On : ScreenState.Off, crlf), CancellationToken.None).ConfigureAwait(false);
        await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"屏幕已经:[{(on ? "打开" : "关闭")}]").Build());
        return;
    }

    private static BotSource<PokeBotState>? GetBot(string ip)
    {
        var r = MiraiQQBot<T>.Runner;
        return r.GetBot(ip) ?? r.Bots.Find(x => x.IsRunning); // safe fallback for users who mistype IP address for single bot instances
    }

    private async Task ClickAsyncImpl(SwitchButton button, BotSource<PokeBotState> bot)
    {
        if (!Enum.IsDefined(typeof(SwitchButton), button))
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"未知的按键:[{button}]").Build());
            return;
        }

        var b = bot.Bot;
        var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
        await b.Connection.SendAsync(SwitchCommand.Click(button, crlf), CancellationToken.None).ConfigureAwait(false);
        await Task.Delay(800).ConfigureAwait(false);
        await CaptureScreenAsync().ConfigureAwait(false);
        return;
    }

    private async Task CaptureScreenAsyncImpl(BotSource<PokeBotState> bot)
    {
        var b = bot.Bot;
        var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };

        if (b.Connection is not ISwitchConnectionAsync connect)
            throw new System.Exception("Not a valid switch connection");
        ISwitchConnectionAsync SwitchConnection = connect;
        try
        {
            byte[] screenData = await SwitchConnection.CaptureCurrentScreen(CancellationToken.None).ConfigureAwait(false);
            // 将字节数组转换为Base64字符串
            string base64String = Convert.ToBase64String(screenData);
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().ImageFromBase64(base64String).Build());
        }
        catch (Exception ex)
        {
            LogUtil.LogInfo("保存屏幕截图时出错：" + ex.Message, "截图");
        }
        return;
    }

    private async Task SetStickAsyncImpl(SwitchStick s, short x, short y, ushort ms, BotSource<PokeBotState> bot)
    {
        if (!Enum.IsDefined(typeof(SwitchStick), s))
        {
            await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"未知的摇杆:[{s}]").Build());
            return;
        }

        var b = bot.Bot;
        var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
        await b.Connection.SendAsync(SwitchCommand.SetStick(s, x, y, crlf), CancellationToken.None).ConfigureAwait(false);
        await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"[{b.Connection.Name}]已经执行:[{s}]").Build());
        await Task.Delay(ms).ConfigureAwait(false);
        await b.Connection.SendAsync(SwitchCommand.ResetStick(s, crlf), CancellationToken.None).ConfigureAwait(false);
        await MessageManager.SendGroupOrTempMessageAsync(receiverQQ, receiverGroup, new MessageChainBuilder().Plain($"[{b.Connection.Name}]已经重置回摇杆的位置").Build());
    }

    private static bool IsRemoteControlBot(RoutineExecutor<PokeBotState> botstate)
        //=> botstate is RemoteControlBotSWSH or RemoteControlBotBS or RemoteControlBotLA or RemoteControlBotSV;
        => true;
}


