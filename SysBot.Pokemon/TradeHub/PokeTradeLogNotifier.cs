using PKHeX.Core;
using SysBot.Base;
using System;
using System.Linq;

namespace SysBot.Pokemon;

public class PokeTradeLogNotifier<T> : IPokeTradeNotifier<T> where T : PKM, new()
{
    public void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        LogUtil.LogInfo($"Starting trade loop for {info.Trainer.TrainerName}, sending {GameInfo.GetStrings("en").Species[info.TradeData.Species]}", routine.Connection.Label);
    }

    public void TradePreviewPokemon(PokeRoutineExecutor<T> routine, string base64Image1, string base64Image2, string base64Image3, PokeTradeDetail<T> info)
    {
        LogUtil.LogInfo($"开始宝可梦预览 {info.Trainer.TrainerName}, 正在发送 {GameInfo.GetStrings("en").Species[info.TradeData.Species]}", routine.Connection.Label);
    }

    public void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        LogUtil.LogInfo($"Searching for trade with {info.Trainer.TrainerName}, sending {GameInfo.GetStrings("en").Species[info.TradeData.Species]}", routine.Connection.Label);
    }

    public void TradeSearchingWithSecond(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, int second, bool needScreenShot)
    {
        LogUtil.LogInfo($"正在搜寻 {info.Trainer.TrainerName}, 派送 {GameInfo.GetStrings("en").Species[info.TradeData.Species]}, 将等待 [{second}] 秒", routine.Connection.Label);
    }

    public void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
    {
        LogUtil.LogInfo($"Canceling trade with {info.Trainer.TrainerName}, because {msg}.", routine.Connection.Label);
        OnFinish?.Invoke(routine);
    }

    public void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
    {
        // Print the nickname for Ledy trades so we can see what was requested.
        var ledyname = string.Empty;
        if (info.Trainer.TrainerName == "Random Distribution" && result.IsNicknamed)
            ledyname = $" (Nickname: \"{result.Nickname}\")";

        LogUtil.LogInfo($"Finished trading {info.Trainer.TrainerName} {GameInfo.GetStrings("en").Species[info.TradeData.Species]} for {GameInfo.GetStrings("en").Species[result.Species]}{ledyname}", routine.Connection.Label);
        OnFinish?.Invoke(routine);
    }

    public void TradeFinishedWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, SharePartnerInfo sharePartnerInfo)
    {
        LogUtil.LogInfo($"Finished trading {info.Trainer.TrainerName} {GameInfo.GetStrings("en").Species[info.TradeData.Species]} for {GameInfo.GetStrings("en").Species[result.Species]}", routine.Connection.Label);
        OnFinish?.Invoke(routine);
    }

    public int TradeFinishedWithImageAndElapsedTime(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, int elapsedTime, SharePartnerInfo sharePartnerInfo)
    {
        LogUtil.LogInfo($"Finished trading {info.Trainer.TrainerName} {GameInfo.GetStrings("en").Species[info.TradeData.Species]} for {GameInfo.GetStrings("en").Species[result.Species]}", routine.Connection.Label);
        OnFinish?.Invoke(routine);
        return 0;
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
    {
        LogUtil.LogInfo(message, routine.Connection.Label);
    }

    public void SendNotificationWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message, string base64Image)
    {
        LogUtil.LogInfo(message, routine.Connection.Label);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
    {
        var msg = message.Summary;
        if (message.Details.Count > 0)
            msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
        LogUtil.LogInfo(msg, routine.Connection.Label);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
    {
        LogUtil.LogInfo($"Notifying {info.Trainer.TrainerName} about their {GameInfo.GetStrings("en").Species[result.Species]}", routine.Connection.Label);
        LogUtil.LogInfo(message, routine.Connection.Label);
    }

    public Action<PokeRoutineExecutor<T>>? OnFinish { get; set; }
}
