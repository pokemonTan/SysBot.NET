namespace SysBot.Pokemon;

public enum PokeTradeResult
{
    Success,

    // Trade Partner Failures
    NoTrainerFound,
    TrainerTooSlow,
    TrainerLeft,
    TrainerOfferCanceledQuick,
    TrainerRequestBad,
    IllegalTrade,
    SuspiciousActivity,

    // Recovery -- General Bot Failures
    // Anything below here should be retried once if possible.
    RoutineCancel,
    ExceptionConnection,
    ExceptionInternal,
    RecoverStart,
    RecoverPostLinkCode,
    RecoverOpenBox,
    RecoverReturnOverworld,
    RecoverEnterUnionRoom,
    RecoverPreviewPokemon,
}

public static class PokeTradeResultExtensions
{
    //大于等于8的常数都应该重试
    public static bool ShouldAttemptRetry(this PokeTradeResult t) => t >= PokeTradeResult.RoutineCancel;
}
