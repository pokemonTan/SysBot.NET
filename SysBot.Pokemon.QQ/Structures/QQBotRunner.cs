using PKHeX.Core;
using SysBot.Base;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Pokemon.QQ;

public interface IQQBotRunner
{
    QQTradeHubConfig Config { get; }
    bool RunOnce { get; }
    bool IsRunning { get; }

    void StartAll();
    void StopAll();
    void InitializeStart();

    void Add(QQRoutineExecutorBase newbot);
    void Remove(IConsoleBotConfig state, bool callStop);

    BotSource<QQBotState>? GetBot(QQBotState state);
    QQRoutineExecutorBase CreateBotFromConfig(QQBotState cfg);
    bool SupportsRoutine(QQRoutineType qqRoutineType);
}

public abstract class QQBotRunner<T> : BotRunner<QQBotState>, IQQBotRunner where T : PKM, new()
{
    public readonly QQTradeHub<T> Hub;
    private readonly QQBotFactory<T> Factory;

    public QQTradeHubConfig Config => Hub.Config;

    protected QQBotRunner(QQTradeHub<T> hub, QQBotFactory<T> factory)
    {
        Hub = hub;
        Factory = factory;
    }

    protected QQBotRunner(QQTradeHubConfig config, QQBotFactory<T> factory)
    {
        Factory = factory;
        Hub = new QQTradeHub<T>(config);
    }

    protected virtual void AddIntegrations() { }

    public override void Add(RoutineExecutor<QQBotState> bot)
    {
        base.Add(bot);
        if (bot is QQRoutineExecutorBase b)
            Hub.Bots.Add(b);
    }

    public override bool Remove(IConsoleBotConfig cfg, bool callStop)
    {
        var bot = GetBot(cfg)?.Bot;
        if (bot is QQRoutineExecutorBase b)
            Hub.Bots.Remove(b);
        return base.Remove(cfg, callStop);
    }

    public override void StartAll()
    {

        Console.WriteLine("测试");
        InitializeStart();

        if (!Hub.Config.SkipConsoleBotCreation)
            base.StartAll();
    }

    public override void InitializeStart()
    {
        if (RunOnce)
            return;

        LogUtil.LogInfo("已经跑到[QQBotRunner.cs]->[InitializeStart]...", "Form");
        AutoLegalityWrapper.EnsureInitialized(Hub.Config.Legality);

        AddIntegrations();

        base.InitializeStart();
    }

    public override void StopAll()
    {
        base.StopAll();

        // bots currently don't de-register
        Thread.Sleep(100);
        int count = Hub.BotSync.Barrier.ParticipantCount;
        if (count != 0)
            Hub.BotSync.Barrier.RemoveParticipants(count);
    }

    public override void PauseAll()
    {
        if (!Hub.Config.SkipConsoleBotCreation)
            base.PauseAll();
    }

    public override void ResumeAll()
    {
        if (!Hub.Config.SkipConsoleBotCreation)
            base.ResumeAll();
    }

    public QQRoutineExecutorBase CreateBotFromConfig(QQBotState cfg) => Factory.CreateBot(Hub, cfg);
    public BotSource<QQBotState>? GetBot(QQBotState state) => base.GetBot(state);
    void IQQBotRunner.Remove(IConsoleBotConfig state, bool callStop) => Remove(state, callStop);
    public void Add(QQRoutineExecutorBase newbot) => Add((RoutineExecutor<QQBotState>)newbot);
    public bool SupportsRoutine(QQRoutineType t) => Factory.SupportsRoutine(t);
}
