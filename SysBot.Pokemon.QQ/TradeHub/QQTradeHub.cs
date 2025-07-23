using PKHeX.Core;
using SysBot.Base;
using System.Collections.Concurrent;

namespace SysBot.Pokemon.QQ;

/// <summary>
/// Centralizes logic for trade bot coordination.
/// </summary>
/// <typeparam name="T">Type of <see cref="PKM"/> to distribute.</typeparam>
public class QQTradeHub<T> where T : PKM, new()
{
    public QQTradeHub(QQTradeHubConfig config)
    {
        Config = config;
        var pool = new PokemonPool<T>(config);
        BotSync = new BotSynchronizer(config.Distribution);
        BotSync.BarrierReleasingActions.Add(() => LogUtil.LogInfo($"{BotSync.Barrier.ParticipantCount} bots released.", "Barrier"));
    }

    public readonly QQTradeHubConfig Config;
    public readonly BotSynchronizer BotSync;

    /// <summary> Trade Bots only, used to delegate multi-player tasks </summary>
    public readonly ConcurrentPool<QQRoutineExecutorBase> Bots = new();
    public bool TradeBotsReady => !Bots.All(z => z.Config.CurrentRoutineType == QQRoutineType.Idle);

   
}
