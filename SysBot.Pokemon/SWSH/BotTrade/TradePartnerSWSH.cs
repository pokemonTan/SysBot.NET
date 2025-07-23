using PKHeX.Core;
using System;
using System.Buffers.Binary;

namespace SysBot.Pokemon;

public sealed class TradePartnerSWSH(string TID7, string SID7, string TrainerName, int Game, int Gender, int Language)
{
    public string TID7 { get; } = TID7;
    public string SID7 { get; } = SID7;
    public string TrainerName { get; } = TrainerName;
    public int Game { get; } = Game;
    public int Gender { get; } = Gender;
    public int Language { get; } = Language;
}
