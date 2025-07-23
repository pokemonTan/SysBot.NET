using PKHeX.Core;
using System;

namespace SysBot.Pokemon;

public interface IPokeTradeNotifier<T> where T : PKM, new()
{
    /// <summary> Notifies when a trade bot is initializing at the start. </summary>
    void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info);
    void TradePreviewPokemon(PokeRoutineExecutor<T> routine, string base64Image1, string base64Image2, string base64Image3, PokeTradeDetail<T> info);
    /// <summary> Notifies when a trade bot is searching for the partner. </summary>
    void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info);
    void TradeSearchingWithSecond(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, int second, bool needScreenShot);
    /// <summary> Notifies when a trade bot notices the trade was canceled. </summary>
    void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg);
    /// <summary> Notifies when a trade bot finishes the trade. </summary>
    void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result);
    /// <summary> Notifies when a trade bot finishes the trade. </summary>
    void TradeFinishedWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, SharePartnerInfo sharePartnerInfo);
    int TradeFinishedWithImageAndElapsedTime(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, int elapsedTime, SharePartnerInfo sharePartnerInfo);
    /// <summary> Sends a notification when called with parameters. </summary>
    void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message);
    /// <summary> Sends a notification when called with parameters. </summary>
    void SendNotificationWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message, string base64Image);
    /// <summary> Sends a notification when called with parameters. </summary>
    void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message);
    /// <summary> Sends a notification when called with parameters. </summary>
    void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message);

    /// <summary> Notifies when a trade bot is initializing at the start. </summary>
    Action<PokeRoutineExecutor<T>>? OnFinish { set; }
}

/// <summary>
/// 共享的初训家信息
/// </summary>
public class SharePartnerInfo(string TID7, string SID7, string TrainerName, int Game, int Gender, int Language)
{
    public string TID7 = TID7;
    public string SID7 = SID7;
    public string TrainerName = TrainerName;
    public int Game = Game;
    public int Gender = Gender;
    public int Language = Language;
}
