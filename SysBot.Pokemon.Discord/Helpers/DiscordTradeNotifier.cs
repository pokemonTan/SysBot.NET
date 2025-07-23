using Discord;
using Discord.WebSocket;
using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord;

public class DiscordTradeNotifier<T>(T Data, PokeTradeTrainerInfo Info, int Code, SocketUser Trader)
    : IPokeTradeNotifier<T>
    where T : PKM, new()
{
    private T Data { get; } = Data;
    private PokeTradeTrainerInfo Info { get; } = Info;
    private int Code { get; } = Code;
    private SocketUser Trader { get; } = Trader;
    public Action<PokeRoutineExecutor<T>>? OnFinish { private get; set; }
    public readonly PokeTradeHub<T> Hub = SysCord<T>.Runner.Hub;

    public void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
        Trader.SendMessageAsync($"Initializing trade{receive}. Please be ready. Your code is **{Code:0000 0000}**.").ConfigureAwait(false);
    }

    public void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", {name}";
        Trader.SendMessageAsync($"I'm waiting for you{trainer}! Your code is **{Code:0000 0000}**. My IGN is **{routine.InGameName}**.").ConfigureAwait(false);
    }

    public void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
    {
        OnFinish?.Invoke(routine);
        Trader.SendMessageAsync($"Trade canceled: {msg}").ConfigureAwait(false);
    }

    public void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = tradedToUser != 0 ? $"Trade finished. Enjoy your {(Species)tradedToUser}!" : "Trade finished!";
        Trader.SendMessageAsync(message).ConfigureAwait(false);
        if (result.Species != 0 && Hub.Config.Discord.ReturnPKMs)
            Trader.SendPKMAsync(result, "Here's what you traded me!").ConfigureAwait(false);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
    {
        Trader.SendMessageAsync(message).ConfigureAwait(false);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
    {
        if (message.ExtraInfo is SeedSearchResult r)
        {
            SendNotificationZ3(r);
            return;
        }

        var msg = message.Summary;
        if (message.Details.Count > 0)
            msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
        Trader.SendMessageAsync(msg).ConfigureAwait(false);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
    {
        if (result.Species != 0 && (Hub.Config.Discord.ReturnPKMs || info.Type == PokeTradeType.Dump))
            Trader.SendPKMAsync(result, message).ConfigureAwait(false);
    }

    private void SendNotificationZ3(SeedSearchResult r)
    {
        var lines = r.ToString();

        var embed = new EmbedBuilder { Color = Color.LighterGrey };
        embed.AddField(x =>
        {
            x.Name = $"Seed: {r.Seed:X16}";
            x.Value = lines;
            x.IsInline = false;
        });
        var msg = $"Here are the details for `{r.Seed:X16}`:";
        Trader.SendMessageAsync(msg, embed: embed.Build()).ConfigureAwait(false);
    }

    public void SendNotificationWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message, string base64Image)
    {
        LogUtil.LogText(message);
    }

    /// <summary>
    /// 交换完成，发送图片
    /// </summary>
    /// <param name="routine"></param>
    /// <param name="info"></param>
    /// <param name="result"></param>
    /// <param name="base64Image"></param>
    public void TradeFinishedWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, SharePartnerInfo sharePartnerInfo)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0
            ? $"Trade finished. Enjoy your {(Species)tradedToUser}!"
            : "Trade finished!");
        LogUtil.LogText(message);
    }

    /// <summary>
    /// 交换完成，发送图片和耗时
    /// </summary>
    /// <param name="routine"></param>
    /// <param name="info"></param>
    /// <param name="result"></param>
    /// <param name="base64Image"></param>
    public int TradeFinishedWithImageAndElapsedTime(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, int elapsedTime, SharePartnerInfo sharePartnerInfo)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0
            ? $"Trade finished. Enjoy your {(Species)tradedToUser}!"
            : "Trade finished!");
        LogUtil.LogText(message);
        return 0;
    }

    public void TradePreviewPokemon(PokeRoutineExecutor<T> routine, string base64Image1, string base64Image2, string base64Image3, PokeTradeDetail<T> info)
    {
        var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
        var text = $"\n派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}";
        if (Data.IsEgg)
        {
            text += $"\n蛋属性分析:宝可梦[{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}],球种:{ShowdownTranslator<T>.GameStringsZh.balllist[Data.Ball]},个体:{Data.IV_HP} HP / {Data.IV_ATK} 攻击 / {Data.IV_DEF} 防御 / {Data.IV_SPA} 特攻 / {Data.IV_SPD} 特防 / {Data.IV_SPE} 速度,需要的孵化圈数[{Data.OriginalTrainerFriendship}],是否闪光:{(Data.IsShiny ? "是" : "不闪")} \n状态:预览";
        }
        else
        {
            text += $"\n个体值:{Data.IV_HP} HP / {Data.IV_ATK} 攻击 / {Data.IV_DEF} 防御 / {Data.IV_SPA} 特攻 / {Data.IV_SPD} 特防 / {Data.IV_SPE} 速度\n努力值:{Data.EV_HP} HP / {Data.EV_ATK} 攻击 / {Data.EV_DEF} 防御 / {Data.EV_SPA} 特攻 / {Data.EV_SPD} 特防 / {Data.EV_SPE} 速度 \n状态:预览";
        }
        List<T> batchPKMs = (List<T>)info.Context.GetValueOrDefault("batch", new List<T>());
        if (batchPKMs.Count > 1)
        {
            text = $"\n批量派送{batchPKMs.Count}只宝可梦\n密码:{info.Code:0000 0000}\n状态:初始化";
        }
        LogUtil.LogInfo(text, "消息");
    }

    public void TradeSearchingWithSecond(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, int second, bool needScreenShot)
    {
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", @{name}";
        var message = $"I'm waiting for you{trainer}! My IGN is {routine.InGameName}.";
        message += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(message);
        var text = $"\n派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}\n状态:搜索中\n我会等你[{second}]秒,我的游戏名是[{routine.InGameName}]";
        List<T> batchPKMs = (List<T>)info.Context.GetValueOrDefault("batch", new List<T>());
        if (batchPKMs.Count > 1)
        {
            text = $"批量派送{batchPKMs.Count}只宝可梦\n密码:{info.Code:0000 0000}\n状态:搜索中";
        }

    }
}
