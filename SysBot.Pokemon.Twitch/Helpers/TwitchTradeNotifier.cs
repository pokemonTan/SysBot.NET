using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client;

namespace SysBot.Pokemon.Twitch;

public class TwitchTradeNotifier<T> : IPokeTradeNotifier<T> where T : PKM, new()
{
    private T Data { get; }
    private PokeTradeTrainerInfo Info { get; }
    private int Code { get; }
    private string Username { get; }
    private TwitchClient Client { get; }
    private string Channel { get; }
    private TwitchSettings Settings { get; }

    public TwitchTradeNotifier(T data, PokeTradeTrainerInfo info, int code, string username, TwitchClient client, string channel, TwitchSettings settings)
    {
        Data = data;
        Info = info;
        Code = code;
        Username = username;
        Client = client;
        Channel = channel;
        Settings = settings;

        LogUtil.LogText($"Created trade details for {Username} - {Code}");
    }

    public Action<PokeRoutineExecutor<T>>? OnFinish { private get; set; }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
    {
        LogUtil.LogText(message);
        SendMessage($"@{info.Trainer.TrainerName}: {message}", Settings.NotifyDestination);
    }

    public void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
    {
        OnFinish?.Invoke(routine);
        var line = $"@{info.Trainer.TrainerName}: Trade canceled, {msg}";
        LogUtil.LogText(line);
        SendMessage(line, Settings.TradeCanceledDestination);
    }

    public void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0 ? $"Trade finished. Enjoy your {(Species)tradedToUser}!" : "Trade finished!");
        LogUtil.LogText(message);
        SendMessage(message, Settings.TradeFinishDestination);
    }

    public void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
        var msg = $"@{info.Trainer.TrainerName} (ID: {info.ID}): Initializing trade{receive} with you. Please be ready. Use the code you whispered me to search!";
        var dest = Settings.TradeStartDestination;
        if (dest == TwitchMessageDestination.Whisper)
            msg += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(msg);
        SendMessage(msg, dest);
    }

    public void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", @{name}";
        var message = $"I'm waiting for you{trainer}! My IGN is {routine.InGameName}.";
        var dest = Settings.TradeSearchDestination;
        if (dest == TwitchMessageDestination.Channel)
            message += " Use the code you whispered me to search!";
        else if (dest == TwitchMessageDestination.Whisper)
            message += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(message);
        SendMessage($"@{info.Trainer.TrainerName} {message}", dest);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
    {
        var msg = message.Summary;
        if (message.Details.Count > 0)
            msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
        LogUtil.LogText(msg);
        SendMessage(msg, Settings.NotifyDestination);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
    {
        var msg = $"Details for {result.FileName}: " + message;
        LogUtil.LogText(msg);
        SendMessage(msg, Settings.NotifyDestination);
    }

    private void SendMessage(string message, TwitchMessageDestination dest)
    {
        switch (dest)
        {
            case TwitchMessageDestination.Channel:
                Client.SendMessage(Channel, message);
                break;
            case TwitchMessageDestination.Whisper:
                Client.SendWhisper(Username, message);
                break;
        }
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
