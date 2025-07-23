using PKHeX.Core;

namespace SysBot.Pokemon.QQ;

public abstract class QQBotFactory<T> where T : PKM, new()
{
    public abstract QQRoutineExecutorBase CreateBot(QQTradeHub<T> hub, QQBotState cfg);
    public abstract bool SupportsRoutine(QQRoutineType type);
}
