using PKHeX.Core;
using SysBot.Base;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Pokemon.QQ;

public class MiraiQQTradeNotifier<T> : IPokeTradeNotifier<T> where T : PKM, new()
{
    private T Data { get; }
    private PokeTradeTrainerInfo Info { get; }
    private int Code { get; }
    private string Username { get; }

    private string GroupId { get; }

    public MiraiQQTradeNotifier(T data, PokeTradeTrainerInfo info, int code, string username, string groupId)
    {
        Data = data;
        Info = info;
        Code = code;
        Username = username;
        GroupId = groupId;
        LogUtil.LogText($"Created trade details for {Username} - {Code}");
    }

    public Action<PokeRoutineExecutor<T>>? OnFinish { private get; set; }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
    {
        LogUtil.LogText(message);
        if (message.Contains("Found Trading Partner:"))
        {
            Regex regex = new Regex("TID: (\\d+)");
            string tid = regex.Match(message).Groups[1].ToString();
            regex = new Regex("SID: (\\d+)");
            string sid = regex.Match(message).Groups[1].ToString();
            //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().Plain($"找到你了，你的SID7:{sid},TID7:{tid}").Build(), GroupId);
        }
        else if (message.StartsWith("批量"))
        {
            //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().Plain(message).Build(), GroupId);
        }
        else
        {
            //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(message).Build(), GroupId);
        }
    }
    public void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
    {
        OnFinish?.Invoke(routine);
        var line = $"@{info.Trainer.TrainerName}: Trade canceled, {msg}";
        LogUtil.LogText(line);
        //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(" 取消").Build(), GroupId);
    }

    public void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0
            ? $"Trade finished. Enjoy your {(Species) tradedToUser}!"
            : "Trade finished!");
        LogUtil.LogText(message);
        string jsonResult = DecrPokemonGroupGold(info.Trainer.ID.ToString(), GroupId, new SharePartnerInfo("","","",0,0,0));
        //int code = Convert.ToInt32(jsonResult.Fetch("code"));
        //string? msg = jsonResult.Fetch("msg");
        //if (code == 200)
        //{
        //    MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain($" {msg}").Build(), GroupId);
        //    LogUtil.LogInfo($"{info.Trainer.ID.ToString()}-{msg}", "测试");
        //}
        //else
        //{
        //    MiraiQQBot<T>.SendGroupOrTempMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain($" {msg}").Build(), info.Trainer.ID.ToString(), GroupId);
        //    LogUtil.LogInfo($"{info.Trainer.ID.ToString()}-{msg}", "测试");
        //}
    }

    public void SendNotificationWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message, string base64Image)
    {
        LogUtil.LogText(message);
        //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(message).ImageFromBase64(base64Image).Build(), GroupId);
    }

    /// <summary>
    /// 交换完成，发送图片
    /// </summary>
    /// <param name="routine"></param>
    /// <param name="info"></param>
    /// <param name="result"></param>
    /// <param name="base64Image"></param>
    public void TradeFinishedWithImage(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, SharePartnerInfo sharePartnerInfo)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0
            ? $"Trade finished. Enjoy your {(Species)tradedToUser}!"
            : "Trade finished!");
        LogUtil.LogText(message);
        string jsonResult = DecrPokemonGroupGold(info.Trainer.ID.ToString(), GroupId, sharePartnerInfo);
        //int code = Convert.ToInt32(jsonResult.Fetch("code"));
        //string? responseMsg = jsonResult.Fetch("msg");
        //MessageChainBuilder builder = new MessageChainBuilder().At($"{info.Trainer.ID}").Plain($" {responseMsg}");
        //string[] base64Array = base64Image.Split('#', StringSplitOptions.RemoveEmptyEntries);
        //foreach (var base64 in base64Array)
        //{
        //    builder.ImageFromBase64(base64);
        //}
        //MessageChain finalMessageChain = builder.Build();
        //MiraiQQBot<T>.SendGroupOrTempMessage(finalMessageChain, info.Trainer.ID.ToString(), GroupId);
    }

    /// <summary>
    /// 交换完成，发送图片和耗时
    /// </summary>
    /// <param name="routine"></param>
    /// <param name="info"></param>
    /// <param name="result"></param>
    /// <param name="base64Image"></param>
    public int TradeFinishedWithImageAndElapsedTime(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string base64Image, int elapsedTime, SharePartnerInfo sharePartnerInfo)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;
        var message = $"@{info.Trainer.TrainerName}: " + (tradedToUser != 0
            ? $"Trade finished. Enjoy your {(Species)tradedToUser}!"
            : "Trade finished!");
        LogUtil.LogText(message);
        string jsonResult = DecrPokemonGroupGold(info.Trainer.ID.ToString(),  GroupId, sharePartnerInfo);
        //int code = Convert.ToInt32(jsonResult.Fetch("code"));
        //string? responseMsg = jsonResult.Fetch("msg");
        //MessageChainBuilder builder = new MessageChainBuilder().At($"{info.Trainer.ID}").Plain($" {responseMsg}");
        string[] base64Array = base64Image.Split('#', StringSplitOptions.RemoveEmptyEntries);
        //foreach (var base64 in base64Array)
        //{
        //    builder.ImageFromBase64(base64);
        //}
        //MessageChain finalMessageChain = builder.Build();
        //MiraiQQBot<T>.SendGroupOrTempMessage(finalMessageChain, info.Trainer.ID.ToString(), GroupId);
        //int total_integral = Convert.ToInt32(jsonResult.Fetch("data.total_integral"));
        //LogUtil.LogInfo($"QQ[{info.Trainer.ID.ToString()}]在QQ群[{GroupId}]还剩余[{total_integral}]熊熊币", "接口查询");
        //string uploadJsonResult = UploadUserPokemon(info.Trainer.ID.ToString(), GroupId, Data, sharePartnerInfo);
        return 0;
    }

    /// <summary>
    /// 扣除熊熊币
    /// </summary>
    /// <param name="qq"></param>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public static string DecrPokemonGroupGold(string qq, string groupId, SharePartnerInfo sharePartnerInfo, int elapsedTime = 0)
    {
        return HttpUtils.Post("https://api.17yohui.com/api/Report/decrPokemonGroupGold", $"version={sharePartnerInfo.Game}&ot_name={sharePartnerInfo.TrainerName}&sid={sharePartnerInfo.SID7}&tid={sharePartnerInfo.TID7}&ot_gender={sharePartnerInfo.Gender}&ot_language={sharePartnerInfo.Language}&qq={qq}&qq_group={groupId}&elapsed_time={elapsedTime}&{Common.GetServerSignStr("decrPokemonGroupGold")}", "https://api.17yohui.com");
    }

    /// <summary>
    /// 上传用户的宝可梦
    /// </summary>
    /// <param name="qq"></param>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public static string UploadUserPokemon(string qq, string groupId, T data, SharePartnerInfo sharePartnerInfo)
    {
        return HttpUtils.Post("https://api.17yohui.com/api/Report/uploadUserPokemon", $"level={data.CurrentLevel}&pokemon_number={data.Species}&is_shiny={data.IsShiny}&form={data.Form}&gender={data.Gender}&ability_id={data.Ability}&ball={data.Ball}&nature_id={(int)data.Nature}&iv_hp={data.IV_HP}&iv_atk={data.IV_ATK}&iv_def={data.IV_DEF}&iv_spa={data.IV_SPA}&iv_spd={data.IV_SPD}&iv_spe={data.IV_SPE}&charcter={data.Characteristic}&hold_item={data.HeldItem}&ot_name={sharePartnerInfo.TrainerName}&qq={qq}&qq_group={groupId}&{Common.GetServerSignStr("uploadUserPokemon")}", "https://api.17yohui.com");
    }

    public void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
        var msg =
            $"@{info.Trainer.TrainerName} (ID: {info.ID}): Initializing trade{receive} with you. Please be ready.";
        msg += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(msg);
        var text = $"\n派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}\n状态:初始化";
        List<T> batchPKMs = (List<T>)info.Context.GetValueOrDefault("batch", new List<T>());
        if (batchPKMs.Count > 1)
        {
            text = $"\n批量派送{batchPKMs.Count}只宝可梦\n密码:{info.Code:0000 0000}\n状态:初始化";
        }
        //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).Build(), GroupId);
    }

    public void TradePreviewPokemon(PokeRoutineExecutor<T> routine, string base64Image1, string base64Image2, string base64Image3, PokeTradeDetail<T> info)
    {
        var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
        var text = $"\n派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}";
        if (Data.IsEgg)
        {
            text += $"\n蛋属性分析:宝可梦[{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}],球种:{ShowdownTranslator<T>.GameStringsZh.balllist[Data.Ball]},个体:{Data.IV_HP} HP / {Data.IV_ATK} 攻击 / {Data.IV_DEF} 防御 / {Data.IV_SPA} 特攻 / {Data.IV_SPD} 特防 / {Data.IV_SPE} 速度,需要的孵化圈数[{Data.OriginalTrainerFriendship}],是否闪光:{(Data.IsShiny ? "是" : "不闪")} \n状态:预览";
        }
        else
        {
            text += $"\n个体值:{Data.IV_HP} HP / {Data.IV_ATK} 攻击 / {Data.IV_DEF} 防御 / {Data.IV_SPA} 特攻 / {Data.IV_SPD} 特防 / {Data.IV_SPE} 速度\n努力值:{Data.EV_HP} HP / {Data.EV_ATK} 攻击 / {Data.EV_DEF} 防御 / {Data.EV_SPA} 特攻 / {Data.EV_SPD} 特防 / {Data.EV_SPE} 速度 \n状态:预览";
        }
        List<T> batchPKMs = (List<T>)info.Context.GetValueOrDefault("batch", new List<T>());
        if (batchPKMs.Count > 1)
        {
            text = $"\n批量派送{batchPKMs.Count}只宝可梦\n密码:{info.Code:0000 0000}\n状态:初始化";
        }
        LogUtil.LogInfo(text, "消息");
        //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).ImageFromBase64(base64Image1).ImageFromBase64(base64Image2).ImageFromBase64(base64Image3).Build(), GroupId);
        //string uploadJsonResult = UploadUserPokemon(info.Trainer.ID.ToString(), GroupId, Data, new SharePartnerInfo("", "", "", 0, 0, 0));
    }

    
    public void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", @{name}";
        var message = $"I'm waiting for you{trainer}! My IGN is {routine.InGameName}.";
        message += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(message);
        var text = $"\n派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}\n状态:搜索中\n我在等你，我的游戏名是[{routine.InGameName}]";
        List<T> batchPKMs = (List<T>)info.Context.GetValueOrDefault("batch", new List<T>());
        if (batchPKMs.Count > 1)
        {
            text = $"批量派送{batchPKMs.Count}只宝可梦\n密码:{info.Code:0000 0000}\n状态:搜索中";
        }
        Task.Run(async () =>
        {
            string screenImageBase64Str = await routine.CaptureCurrentScreenBase64(CancellationToken.None);
            if (screenImageBase64Str == "")
            {
                //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).Build(), GroupId);
            }
            else
            {
                //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).ImageFromBase64(screenImageBase64Str).Build(), GroupId);
            }
        });
    }

    public void TradeSearchingWithSecond(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, int second, bool needScreenShot)
    {
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", @{name}";
        var message = $"I'm waiting for you{trainer}! My IGN is {routine.InGameName}.";
        message += $" Your trade code is: {info.Code:0000 0000}";
        LogUtil.LogText(message);
        var text = $"\n派送:{ShowdownTranslator<T>.GameStringsZh.Species[Data.Species]}\n密码:{info.Code:0000 0000}\n状态:搜索中\n我会等你[{second}]秒,我的游戏名是[{routine.InGameName}]";
        List<T> batchPKMs = (List<T>)info.Context.GetValueOrDefault("batch", new List<T>());
        if (batchPKMs.Count > 1)
        {
            text = $"批量派送{batchPKMs.Count}只宝可梦\n密码:{info.Code:0000 0000}\n状态:搜索中";
        }
        Task.Run(async () =>
        {
            if (needScreenShot)
            {
                string screenImageBase64Str = await routine.CaptureCurrentScreenBase64(CancellationToken.None);
                if (screenImageBase64Str == "")
                {
                    //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).Build(), GroupId);
                }
                else
                {
                    //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).ImageFromBase64(screenImageBase64Str).Build(), GroupId);
                }
            }
            else
            {
                //MiraiQQBot<T>.SendGroupMessage(new MessageChainBuilder().At($"{info.Trainer.ID}").Plain(text).Build(), GroupId);
            }
        });
        
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
    {
        var msg = message.Summary;
        if (message.Details.Count > 0)
            msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
        LogUtil.LogText(msg);
        //MiraiQQBot<T>.SendGroupMessage(msg, GroupId);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
    {
        var msg = $"Details for {result.FileName}: " + message;
        LogUtil.LogText(msg);
        if (result.Species != 0 && info.Type == PokeTradeType.Dump)
        {
            var text =
                $"species:{result.Species}\npid:{result.PID}\nec:{result.EncryptionConstant}\nIVs:{string.Join(",", result.IVs)}\nisShiny:{result.IsShiny}";
            //MiraiQQBot<T>.SendGroupMessage(text, GroupId);
        }
    }
}
