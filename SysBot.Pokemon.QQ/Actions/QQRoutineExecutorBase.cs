using PKHeX.Core;
using SysBot.Base;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Pokemon.QQ;

public abstract class QQRoutineExecutorBase(IConsoleBotManaged<IConsoleConnection, IConsoleConnectionAsync> Config)
    : SwitchRoutineExecutor<QQBotState>(Config)
{
    public const decimal BotbaseVersion = 2.4m;

    public override string GetSummary()
    {
        var current = Config.CurrentRoutineType;
        var initial = Config.InitialRoutine;
        if (current == initial)
            return $"{Connection.Label} - {initial}";
        return $"{Connection.Label} - {initial} ({current})";
    }

    public override void SoftStop() => Config.Pause();

    public Task Click(SwitchButton b, int delayMin, int delayMax, CancellationToken token) =>
        Click(b, Util.Rand.Next(delayMin, delayMax), token);

    public Task SetStick(SwitchStick stick, short x, short y, int delayMin, int delayMax, CancellationToken token) =>
        SetStick(stick, x, y, Util.Rand.Next(delayMin, delayMax), token);
}
