using PKHeX.Core;
using SysBot.Base;
using SysBot.Pokemon.Helpers;

namespace SysBot.Pokemon.QQ;

public class MiraiQQTrade<T> : AbstractTrade<T> where T : PKM, new()
{
    private readonly string GroupId = default!;
    public MiraiQQTrade(string qq, string nickName, string groupId) 
    {
        SetPokeTradeTrainerInfo(new PokeTradeTrainerInfo(nickName, ulong.Parse(qq)));
        SetTradeQueueInfo(MiraiQQBot<T>.Info);
        GroupId = groupId;
    }

    public override IPokeTradeNotifier<T> GetPokeTradeNotifier(T pkm, int code)
    {
        return new MiraiQQTradeNotifier<T>(pkm, userInfo, code, userInfo.TrainerName, GroupId);
    }

    public override void SendMessage(string message)
    {
        //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At(userInfo.ID.ToString()).Plain(message).Build(), GroupId);
    }
}
