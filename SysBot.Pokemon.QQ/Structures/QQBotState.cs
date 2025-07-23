using SysBot.Base;
using System;

namespace SysBot.Pokemon.QQ;

/// <summary>
/// Tracks the state of the bot and what it should execute next.
/// </summary>
[Serializable]
public sealed class QQBotState : BotState<QQRoutineType, SwitchConnectionConfig>
{
    /// <inheritdoc/>
    public override void IterateNextRoutine() => CurrentRoutineType = NextRoutineType;
    /// <inheritdoc/>
    public override void Initialize() => Resume();
    /// <inheritdoc/>
    public override void Pause() => NextRoutineType = QQRoutineType.Idle;
    /// <inheritdoc/>
    public override void Resume() => NextRoutineType = InitialRoutine;
}
