using Google.Apis.YouTube.v3.Data;
using PKHeX.Core;
using SysBot.Base;
using SysBot.Pokemon.Z3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SysBot.Pokemon.WinForms;

public sealed partial class Main : Form
{
    private readonly List<PokeBotState> Bots = [];
    private readonly IPokeBotRunner RunningEnvironment;
    private readonly ProgramConfig Config;

    public Main()
    {
        InitializeComponent();
        Common.ConfigPath = Program.ConfigPath;
        PokeTradeBotSWSH.SeedChecker = new Z3SeedSearchHandler<PK8>();
        if (File.Exists(Program.ConfigPath))
        {
            var lines = File.ReadAllText(Program.ConfigPath);
            Config = JsonSerializer.Deserialize(lines, ProgramConfigContext.Default.ProgramConfig) ?? new ProgramConfig();
            LogConfig.MaxArchiveFiles = Config.Hub.MaxArchiveFiles;
            LogConfig.LoggingEnabled = Config.Hub.LoggingEnabled;

            RunningEnvironment = GetRunner(Config);
            foreach (var bot in Config.Bots)
            {
                bot.Initialize();
                AddBot(bot);
            }
        }
        else
        {
            Config = new ProgramConfig();
            RunningEnvironment = GetRunner(Config);
            Config.Hub.Folder.CreateDefaults(Program.WorkingDirectory);
        }

        RTB_Logs.MaxLength = 32_767; // character length
        LoadControls();
        Text = $"{Text} ({Config.Mode})";
        Task.Run(BotMonitor);

        InitUtil.InitializeStubs(Config.Mode);
    }

    private static IPokeBotRunner GetRunner(ProgramConfig cfg) => cfg.Mode switch
    {
        ProgramMode.SWSH => new PokeBotRunnerImpl<PK8>(cfg.Hub, new BotFactory8SWSH()),
        ProgramMode.BDSP => new PokeBotRunnerImpl<PB8>(cfg.Hub, new BotFactory8BS()),
        ProgramMode.LA => new PokeBotRunnerImpl<PA8>(cfg.Hub, new BotFactory8LA()),
        ProgramMode.SV => new PokeBotRunnerImpl<PK9>(cfg.Hub, new BotFactory9SV()),
        _ => throw new IndexOutOfRangeException("Unsupported mode."),
    };

    private async Task BotMonitor()
    {
        while (!Disposing)
        {
            try
            {
                foreach (var c in FLP_Bots.Controls.OfType<BotController>())
                    c.ReadState();
            }
            catch
            {
                // Updating the collection by adding/removing bots will change the iterator
                // Can try a for-loop or ToArray, but those still don't prevent concurrent mutations of the array.
                // Just try, and if failed, ignore. Next loop will be fine. Locks on the collection are kinda overkill, since this task is not critical.
            }
            await Task.Delay(2_000).ConfigureAwait(false);
        }
    }

    private void LoadControls()
    {
        MinimumSize = Size;
        PG_Hub.SelectedObject = RunningEnvironment.Config;

        var routines = Enum.GetValues<PokeRoutineType>().Where(z => RunningEnvironment.SupportsRoutine(z));
        var list = routines.Select(z => new ComboItem(z.ToString(), (int)z)).ToArray();
        CB_Routine.DisplayMember = nameof(ComboItem.Text);
        CB_Routine.ValueMember = nameof(ComboItem.Value);
        CB_Routine.DataSource = list;
        CB_Routine.SelectedValue = (int)PokeRoutineType.FlexTrade; // default option

        var protocols = Enum.GetValues<SwitchProtocol>();
        var listP = protocols.Select(z => new ComboItem(z.ToString(), (int)z)).ToArray();
        CB_Protocol.DisplayMember = nameof(ComboItem.Text);
        CB_Protocol.ValueMember = nameof(ComboItem.Value);
        CB_Protocol.DataSource = listP;
        CB_Protocol.SelectedIndex = (int)SwitchProtocol.WiFi; // default option

        LogUtil.Forwarders.Add(new TextBoxForwarder(RTB_Logs));
    }

    private ProgramConfig GetCurrentConfiguration()
    {
        Config.Bots = [.. Bots];
        return Config;
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        SaveCurrentConfig();
        var bots = RunningEnvironment;
        if (!bots.IsRunning)
            return;

        async Task WaitUntilNotRunning()
        {
            while (bots.IsRunning)
                await Task.Delay(10).ConfigureAwait(false);
        }

        // Try to let all bots hard-stop before ending execution of the entire program.
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        bots.StopAll();
        Task.WhenAny(WaitUntilNotRunning(), Task.Delay(5_000)).ConfigureAwait(true).GetAwaiter().GetResult();
    }

    private void SaveCurrentConfig()
    {
        var cfg = GetCurrentConfiguration();
        var lines = JsonSerializer.Serialize(cfg, ProgramConfigContext.Default.ProgramConfig);
        File.WriteAllText(Program.ConfigPath, lines);
    }

    [JsonSerializable(typeof(ProgramConfig))]
    [JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public sealed partial class ProgramConfigContext : JsonSerializerContext;

    private void B_Start_Click(object sender, EventArgs e)
    {
        SaveCurrentConfig();

        LogUtil.LogInfo("Starting all bots...", "Form");
        RunningEnvironment.InitializeStart();
        SendAll(BotControlCommand.Start);
        Tab_Logs.Select();

        if (Bots.Count == 0)
            WinFormsUtil.Alert("No bots configured, but all supporting services have been started.");
    }

    private void SendAll(BotControlCommand cmd)
    {
        foreach (var c in FLP_Bots.Controls.OfType<BotController>())
            c.SendCommand(cmd, false);

        EchoUtil.Echo($"All bots have been issued a command to {cmd}.");
    }

    private void B_Stop_Click(object sender, EventArgs e)
    {
        var env = RunningEnvironment;
        if (!env.IsRunning && (ModifierKeys & Keys.Alt) == 0)
        {
            WinFormsUtil.Alert("当前没有正在运行中的机器人.");
            return;
        }

        var cmd = BotControlCommand.Stop;

        if ((ModifierKeys & Keys.Control) != 0 || (ModifierKeys & Keys.Shift) != 0) // either, because remembering which can be hard
        {
            if (env.IsRunning)
            {
                WinFormsUtil.Alert("Commanding all bots to Idle.", "Press Stop (without a modifier key) to hard-stop and unlock control, or press Stop with the modifier key again to resume.");
                cmd = BotControlCommand.Idle;
            }
            else
            {
                WinFormsUtil.Alert("Commanding all bots to resume their original task.", "Press Stop (without a modifier key) to hard-stop and unlock control.");
                cmd = BotControlCommand.Resume;
            }
        }
        SendAll(cmd);
    }

    private void B_New_Click(object sender, EventArgs e)
    {
        var cfg = CreateNewBotConfig();
        if (!AddBot(cfg))
        {
            WinFormsUtil.Alert("Unable to add bot; ensure details are valid and not duplicate with an already existing bot.");
            return;
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private bool AddBot(PokeBotState cfg)
    {
        if (!cfg.IsValid())
            return false;

        if (Bots.Any(z => z.Connection.Equals(cfg.Connection)))
            return false;

        PokeRoutineExecutorBase newBot;
        try
        {
            Console.WriteLine($"Current Mode ({Config.Mode}) does not support this type of bot ({cfg.CurrentRoutineType}).");
            newBot = RunningEnvironment.CreateBotFromConfig(cfg);
        }
        catch
        {
            return false;
        }

        try
        {
            RunningEnvironment.Add(newBot);
        }
        catch (ArgumentException ex)
        {
            WinFormsUtil.Error(ex.Message);
            return false;
        }

        AddBotControl(cfg);
        Bots.Add(cfg);
        return true;
    }

    private void AddBotControl(PokeBotState cfg)
    {
        var row = new BotController { Width = FLP_Bots.Width };
        row.Initialize(RunningEnvironment, cfg);
        FLP_Bots.Controls.Add(row);
        FLP_Bots.SetFlowBreak(row, true);
        row.Click += (s, e) =>
        {
            var details = cfg.Connection;
            TB_IP.Text = details.IP;
            NUD_Port.Text = details.Port.ToString();
            CB_Protocol.SelectedIndex = (int)details.Protocol;
            CB_Routine.SelectedValue = (int)cfg.InitialRoutine;
        };

        row.Remove += (s, e) =>
        {
            Bots.Remove(row.State);
            RunningEnvironment.Remove(row.State, !RunningEnvironment.Config.SkipConsoleBotCreation);
            FLP_Bots.Controls.Remove(row);
        };
    }

    private PokeBotState CreateNewBotConfig()
    {
        var ip = TB_IP.Text;
        var port = int.TryParse(NUD_Port.Text, out var p) ? p : 6000;
        var cfg = BotConfigUtil.GetConfig<SwitchConnectionConfig>(ip, port);
        cfg.Protocol = (SwitchProtocol)WinFormsUtil.GetIndex(CB_Protocol);

        var pk = new PokeBotState { Connection = cfg };
        var type = (PokeRoutineType)WinFormsUtil.GetIndex(CB_Routine);
        pk.Initialize(type);
        return pk;
    }

    private void FLP_Bots_Resize(object sender, EventArgs e)
    {
        foreach (var c in FLP_Bots.Controls.OfType<BotController>())
            c.Width = FLP_Bots.Width;
    }

    private void CB_Protocol_SelectedIndexChanged(object sender, EventArgs e)
    {
        var isWifi = CB_Protocol.SelectedIndex == 0;
        TB_IP.Visible = isWifi;
        NUD_Port.ReadOnly = isWifi;

        if (isWifi)
            NUD_Port.Text = "6000";
    }

    /// <summary>
    /// 更新机器人信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btn_update_robot_info_Click(object sender, EventArgs e)
    {
        Common.GetRobotInfo();
    }

    private void btn_minus_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.MINUS);
    }

    private void btn_plus_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.PLUS);
    }

    private void btn_arrow_left_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.DLEFT);
    }

    private void btn_arrow_up_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.DUP);
    }

    private void btn_arrow_right_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.DRIGHT);
    }

    private void btn_arrow_down_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.DDOWN);
    }

    private void btn_x_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.X);
    }

    private void btn_y_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.Y);
    }

    private void btn_a_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.A);
    }

    private void btn_b_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.B);
    }

    private void btn_screen_capture_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.CAPTURE);
    }

    private void btn_home_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.HOME);
    }

    private void btn_l_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.L);
    }

    private void btn_r_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.R);
    }

    private void btn_zr_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.ZR);
    }

    private void btn_zl_Click(object sender, EventArgs e)
    {
        ClickButton(SwitchButton.ZL);
    }

    private async void ClickButton(SwitchButton button)
    {
        var bots = RunningEnvironment;
        if (!bots.IsRunning)
        {
            LogUtil.LogText("没有机器人在运行中...");
            return;
        }

        foreach (var bot in Bots)
        {
            var b = bots.GetBot(bot)?.Bot;
            var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
            if (b != null)
            {
                await b.Connection.SendAsync(SwitchCommand.Click(button, crlf), CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                LogUtil.LogText("手柄没有连接成功...");
            }
        }
    }

    private async void CaptureScreenButton_Click(object sender, EventArgs e)
    {
        var bots = RunningEnvironment;
        if (!bots.IsRunning)
        {
            LogUtil.LogInfo("没有机器人在运行中...", "截图");
            return;
        }

        foreach (var bot in Bots)
        {
            var b = bots.GetBot(bot)?.Bot;
            var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
            if (b != null)
            {
                if (b.Connection is not ISwitchConnectionAsync connect)
                    throw new System.Exception("Not a valid switch connection");
                ISwitchConnectionAsync SwitchConnection = connect;
                try
                {
                    byte[] screenData = await SwitchConnection.CaptureCurrentScreen(CancellationToken.None).ConfigureAwait(false);
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
                    string filePath = $@"{screenDir}\{formattedDateTime}.jpg";
                    File.WriteAllBytes(filePath, screenData);
                    LogUtil.LogInfo($"屏幕截图已保存为JPG文件：[{filePath}]", "截图");
                }
                catch (Exception ex)
                {
                    LogUtil.LogInfo("保存屏幕截图时出错：" + ex.Message, "截图");
                }
            }
            else
            {
                LogUtil.LogInfo("手柄没有连接成功...", "手柄连接");
            }
        }
    }

    private async void ResetGame_Click(object sender, EventArgs e)
    {
        //String Text = await Common.ImageOCRChinese($@"D:\SwitchScreen\20250104125533261.jpg");
        //LogUtil.LogInfo($"{Text}", "截图");
        //if (Common.ContainsAllSubstrings(Text, new string[] { "信", "息", "结", "束" }))
        //{
        //    LogUtil.LogInfo($"{Text}", "截图");
        //}
        ////先重启游戏再重启自身
        //Config.Mode = ProgramMode.SV;
        //SaveCurrentConfig();
        //// 获取当前进程的可执行文件路径
        //string exePath = Process.GetCurrentProcess().MainModule.FileName;

        //// 启动新的进程来运行当前应用程序
        //Process.Start(exePath);

        //// 启动机器人

        //// 终止当前进程
        //Environment.Exit(0);
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        var bots = RunningEnvironment;
        if (!bots.IsRunning)
        {
            LogUtil.LogInfo("没有机器人在运行中...", "截图");
            return;
        }

        foreach (var bot in Bots)
        {
            var b = bots.GetBot(bot)?.Bot;
            var crlf = b is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
            if (b != null)
            {
                if (b.Connection is not ISwitchConnectionAsync connect)
                    throw new System.Exception("Not a valid switch connection");
                ISwitchConnectionAsync SwitchConnection = connect;
                try
                {
                    SwitchButton sb = SwitchButton.DLEFT;
                    await SwitchConnection.SendAsync(SwitchCommand.Hold(sb), CancellationToken.None).ConfigureAwait(false);
                    await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
                    await SwitchConnection.SendAsync(SwitchCommand.Release(sb), CancellationToken.None).ConfigureAwait(false);
                    await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogUtil.LogInfo("保存屏幕截图时出错：" + ex.Message, "截图");
                }
            }
            else
            {
                LogUtil.LogInfo("手柄没有连接成功...", "手柄连接");
            }
        }
    }
}
