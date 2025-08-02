using NapCatScript.Core.Model;
using PKHeX.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SysBot.Pokemon.QQ;

public class CommandModule<T> where T : PKM, new()
{
    public void Execute(MsgInfo mesg)
    {
        if (!mesg.IsAtRobot || (mesg.MessageType != "group")) return;
        string firstPlain = mesg.FirstPlain;
        if (string.IsNullOrWhiteSpace(firstPlain)) return;

        var qq = mesg.SenderId;
        var memberName = mesg.SenderMemberName;
        var nickName = mesg.SenderNickName;
        var groupId = mesg.GroupId;
        long botQQ = mesg.BotQQ;
        long sourceMessageId = mesg.MessageId;
        if (firstPlain.Trim().StartsWith("取消"))
        {
            var result = MiraiQQBot<T>.Info.ClearTrade(ulong.Parse(qq));
            MiraiQQBot<T>.SendGroupTextMessage(botQQ, groupId, GetClearTradeMessage(result), sourceMessageId);
        }
        else if (firstPlain.Trim().StartsWith("位置"))
        {
            var result = MiraiQQBot<T>.Info.CheckPosition(ulong.Parse(qq));
            MiraiQQBot<T>.SendGroupTextMessage(botQQ, groupId, GetQueueCheckResultMessage(result), sourceMessageId);
        }
    }
  
    public static string GetQueueCheckResultMessage(QueueCheckResult<T> result)
    {
        if (!result.InQueue || result.Detail is null)
            return "你不在队列里";
        var msg = $" 你在第{result.Position}位";
        var pk = result.Detail.Trade.TradeData;
        if (pk.Species != 0)
            msg += $"，交换宝可梦：{ShowdownTranslator<T>.GameStringsZh.Species[result.Detail.Trade.TradeData.Species]}";
        return msg;
    }

    private static string GetClearTradeMessage(QueueResultRemove result)
    {
        return result switch
        {
            QueueResultRemove.CurrentlyProcessing => "你正在交换中",
            QueueResultRemove.CurrentlyProcessingRemoved => "正在将你的排队取消",
            QueueResultRemove.Removed => "已取消你的排队",
            _ => "你不在队列里",
        };
    }
}

