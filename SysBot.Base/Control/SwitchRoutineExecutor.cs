using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Base;

public abstract class SwitchRoutineExecutor<T> : RoutineExecutor<T> where T : class, IConsoleBotConfig
{
    public readonly bool UseCRLF;
    protected readonly ISwitchConnectionAsync SwitchConnection;

    protected SwitchRoutineExecutor(IConsoleBotManaged<IConsoleConnection, IConsoleConnectionAsync> Config) : base(Config)
    {
        UseCRLF = Config.GetInnerConfig() is ISwitchConnectionConfig { UseCRLF: true };
        if (Connection is not ISwitchConnectionAsync connect)
            throw new System.Exception("Not a valid switch connection");
        SwitchConnection = connect;
    }

    public override Task InitialStartup(CancellationToken token) => EchoCommands(false, token);

    public async Task Click(SwitchButton b, int delay, CancellationToken token)
    {
        await Connection.SendAsync(SwitchCommand.Click(b, UseCRLF), token).ConfigureAwait(false);
        await Task.Delay(delay, token).ConfigureAwait(false);
    }

    public async Task PressAndHold(SwitchButton b, int hold, int delay, CancellationToken token)
    {
        await Connection.SendAsync(SwitchCommand.Hold(b, UseCRLF), token).ConfigureAwait(false);
        await Task.Delay(hold, token).ConfigureAwait(false);
        await Connection.SendAsync(SwitchCommand.Release(b, UseCRLF), token).ConfigureAwait(false);
        await Task.Delay(delay, token).ConfigureAwait(false);
    }

    public async Task DaisyChainCommands(int delay, IEnumerable<SwitchButton> buttons, CancellationToken token)
    {
        SwitchCommand.Configure(SwitchConfigureParameter.mainLoopSleepTime, delay, UseCRLF);
        var commands = buttons.Select(z => SwitchCommand.Click(z, UseCRLF)).ToArray();
        var chain = commands.SelectMany(x => x).ToArray();
        await Connection.SendAsync(chain, token).ConfigureAwait(false);
        SwitchCommand.Configure(SwitchConfigureParameter.mainLoopSleepTime, 0, UseCRLF);
    }

    public async Task SetStick(SwitchStick stick, short x, short y, int delay, CancellationToken token)
    {
        var cmd = SwitchCommand.SetStick(stick, x, y, UseCRLF);
        await Connection.SendAsync(cmd, token).ConfigureAwait(false);
        await Task.Delay(delay, token).ConfigureAwait(false);
    }

    public async Task DetachController(CancellationToken token)
    {
        await Connection.SendAsync(SwitchCommand.DetachController(UseCRLF), token).ConfigureAwait(false);
    }

    public async Task SetScreen(ScreenState state, CancellationToken token)
    {
        await Connection.SendAsync(SwitchCommand.SetScreen(state, UseCRLF), token).ConfigureAwait(false);
    }

    public async Task EchoCommands(bool value, CancellationToken token)
    {
        var cmd = SwitchCommand.Configure(SwitchConfigureParameter.echoCommands, value ? 1 : 0, UseCRLF);
        await Connection.SendAsync(cmd, token).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ReadUntilChanged(ulong,byte[],int,int,bool,bool,CancellationToken)"/>
    public Task<bool> ReadUntilChanged(uint offset, byte[] comparison, int waitms, int waitInterval, bool match, CancellationToken token) =>
        ReadUntilChanged(offset, comparison, waitms, waitInterval, match, false, token);

    /// <summary>
    /// Reads an offset until it changes to either match or differ from the comparison value.
    /// </summary>
    /// <returns>If <see cref="match"/> is set to true, then the function returns true when the offset matches the given value.<br>Otherwise, it returns true when the offset no longer matches the given value.</br></returns>
    public async Task<bool> ReadUntilChanged(ulong offset, byte[] comparison, int waitms, int waitInterval, bool match, bool absolute, CancellationToken token)
    {
        var sw = new Stopwatch();
        sw.Start();
        int readInteval = 0;
        do
        {
            var task = absolute
                ? SwitchConnection.ReadBytesAbsoluteAsync(offset, comparison.Length, token)
                : SwitchConnection.ReadBytesAsync((uint)offset, comparison.Length, token);
            var result = await task.ConfigureAwait(false);
            if (match == result.SequenceEqual(comparison))
                return true;
            readInteval++;
            //if (readInteval % 10 == 0)
            //{
            //    Log($"正在读取[{offset:X16}]的变化,已等待[{sw.ElapsedMilliseconds}]毫秒,仍需要等待[{waitms - sw.ElapsedMilliseconds}]毫秒");
            //}
            await Task.Delay(waitInterval, token).ConfigureAwait(false);
        } while (sw.ElapsedMilliseconds < waitms);
        Log($"读取[{offset:X16}]的变化失败，等待超时");
        return false;
    }

    /// <summary>
    /// 返回当前截图文件路径
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<string> CaptureCurrentScreenFilePath(CancellationToken token)
    {
        byte[] screenData = await SwitchConnection.CaptureCurrentScreen(token).ConfigureAwait(false);
        string filePath = "";
        try
        {
            // 获取当前日期和时间
            DateTime now = DateTime.Now;
            // 格式化日期和时间
            string formattedDateTime = now.ToString("yyyyMMddHHmmssfff");
            string screenDir = "D:\\SwitchScreen";
            // 检查目录是否存在
            if (!Directory.Exists(screenDir))
            {
                // 如果目录不存在，则创建它
                Directory.CreateDirectory(screenDir);
            }
            filePath = $@"{screenDir}\{formattedDateTime}.jpg";
            System.IO.File.WriteAllBytes(filePath, screenData);
            LogUtil.LogInfo($"屏幕截图已保存为JPG文件：[{filePath}]", "截图");
            return filePath;
        }
        catch (Exception ex)
        {
            LogUtil.LogInfo("保存屏幕截图时出错：" + ex.Message, "截图");
            return filePath;
        }
    }

    /// <summary>
    /// 返回当前截图文件Base64字符串
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<string> CaptureCurrentScreenBase64(CancellationToken token)
    {
        byte[] screenData = await SwitchConnection.CaptureCurrentScreen(token).ConfigureAwait(false);
        string base64String = "";
        try
        {
            // 将字节数组转换为Base64字符串
            base64String = Convert.ToBase64String(screenData);
            LogUtil.LogInfo($"屏幕截图已转换为Base64字符串，长度：{base64String.Length} 字符", "截图");
            return base64String;
        }
        catch (Exception ex)
        {
            LogUtil.LogInfo("保存屏幕截图时出错：" + ex.Message, "截图");
            return "";
        }
    }
}
