using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static SysBot.Base.SwitchButton;

namespace SysBot.Pokemon.QQ;

public abstract class QQRoutineExecutor<T>(IConsoleBotManaged<IConsoleConnection, IConsoleConnectionAsync> Config)
    : QQRoutineExecutorBase(Config)
    where T : PKM, new()
{
    
    protected async Task<(bool, ulong)> ValidatePointerAll(IEnumerable<long> jumps, CancellationToken token)
    {
        Log("执行ValidatePointerAll,校验所有指针");
        // 打印 jumps 的值
        foreach (var jump in jumps)
        {
            Log($"jumps 中的一个值: {jump}");
        }
        var solved = await SwitchConnection.PointerAll(jumps, token).ConfigureAwait(false);
        return (solved != 0, solved);
    }


    public async Task<bool> TryReconnect(int attempts, int extraDelay, SwitchProtocol protocol, CancellationToken token)
    {
        // USB can have several reasons for connection loss, some of which is not recoverable (power loss, sleep).
        // Only deal with Wi-Fi for now.
        if (protocol is SwitchProtocol.WiFi)
        {
            // If ReconnectAttempts is set to -1, this should allow it to reconnect (essentially) indefinitely.
            for (int i = 0; i < (uint)attempts; i++)
            {
                LogUtil.LogInfo($"正在尝试重连WiFi... ({i + 1})", Connection.Label);
                Connection.Reset();
                if (Connection.Connected)
                    break;

                await Task.Delay(30_000 + extraDelay, token).ConfigureAwait(false);
            }
        }
        return Connection.Connected;
    }

    public async Task VerifyBotbaseVersion(CancellationToken token)
    {
        var data = await SwitchConnection.GetBotbaseVersion(token).ConfigureAwait(false);
        var version = decimal.TryParse(data, CultureInfo.InvariantCulture, out var v) ? v : 0;
        if (version < BotbaseVersion)
        {
            var protocol = Config.Connection.Protocol;
            var msg = protocol is SwitchProtocol.WiFi ? "sys-botbase" : "usb-botbase";
            msg += $" version is not supported. Expected version {BotbaseVersion} or greater, and current version is {version}. Please download the latest version from: ";
            if (protocol is SwitchProtocol.WiFi)
                msg += "https://github.com/olliz0r/sys-botbase/releases/latest";
            else
                msg += "https://github.com/Koi-3088/usb-botbase/releases/latest";
            throw new Exception(msg);
        }
    }

    // Check if either Tesla or dmnt are active if the sanity check for Trainer Data fails, as these are common culprits.
    private const ulong ovlloaderID = 0x420000000007e51a; // Tesla Menu
    private const ulong dmntID = 0x010000000000000d;      // dmnt used for cheats

    public async Task CheckForRAMShiftingApps(CancellationToken token)
    {
        Log("Trainer data is not valid.");

        bool found = false;
        var msg = "";
        if (await SwitchConnection.IsProgramRunning(ovlloaderID, token).ConfigureAwait(false))
        {
            msg += "Found Tesla Menu";
            found = true;
        }

        if (await SwitchConnection.IsProgramRunning(dmntID, token).ConfigureAwait(false))
        {
            if (found)
                msg += " and ";
            msg += "dmnt (cheat codes?)";
            found = true;
        }
        if (found)
        {
            msg += ".";
            Log(msg);
            Log("Please remove interfering applications and reboot the Switch.");
        }
    }

    private static RemoteControlAccess GetReference(string name, ulong id, string comment) => new()
    {
        ID = id,
        Name = name,
        Comment = $"Added automatically on {DateTime.Now:yyyy.MM.dd-hh:mm:ss} ({comment})",
    };

    // Blocks a user from the box during in-game trades (SWSH).
    private async Task BlockUser(CancellationToken token)
    {
        Log("Blocking user in-game...");
        await PressAndHold(RSTICK, 0_750, 0, token).ConfigureAwait(false);
        await Click(DUP, 0_300, token).ConfigureAwait(false);
        await Click(A, 1_300, token).ConfigureAwait(false);
        await Click(A, 1_300, token).ConfigureAwait(false);
        await Click(DUP, 0_300, token).ConfigureAwait(false);
        await Click(A, 1_100, token).ConfigureAwait(false);
        await Click(A, 1_100, token).ConfigureAwait(false);
    }

    
}
