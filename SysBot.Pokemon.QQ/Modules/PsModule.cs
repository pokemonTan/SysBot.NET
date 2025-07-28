using NapCatScript.Core;
using NapCatScript.Core.JsonFormat.Msgs;
using NapCatScript.Core.Model;
using NapCatScript.Core.MsgHandle;
using PKHeX.Core;
using SysBot.Base;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using NapCatScript.Core.JsonFormat;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SysBot.Pokemon.QQ;
public class APIResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    [JsonPropertyName("data")]
    public PokemonData? Data { get; set; }
}

public class PokemonData
{
    [JsonPropertyName("sv")]
    public GameOTInfo? Sv { get; set; }

    [JsonPropertyName("bdsp")]
    public GameOTInfo? Bdsp { get; set; }

    [JsonPropertyName("pla")]
    public GameOTInfo? Pla { get; set; }

    [JsonPropertyName("swsh")]
    public GameOTInfo? Swsh { get; set; }
}

public class GameOTInfo
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("ot_name")]
    public string OtName { get; set; } = "";

    [JsonPropertyName("ot_gender")]
    public int OtGender { get; set; }

    [JsonPropertyName("ot_language")]
    public int OtLanguage { get; set; }

    [JsonPropertyName("sid")]
    public int Sid { get; set; }

    [JsonPropertyName("tid")]
    public int Tid { get; set; }
}

public class PsModule<T>  where T : PKM, new()
{
    public bool? IsEnable { get; set; } = true;

    public  void Execute(MsgInfo mesg)
    {
        if (!mesg.IsAtRobot || (mesg.MessageType != "group")) return;
        string firstPlain = mesg.FirstPlain;
        if (string.IsNullOrWhiteSpace(firstPlain)) return;
        
        var qq = mesg.SenderId;
        var nickName = mesg.SenderMemberName;
        var groupId = mesg.GroupId;
        long botQQ = mesg.BotQQ;
        long sourceMessageId = mesg.MessageId;
        LogUtil.LogInfo($"接受到消息：[{firstPlain}]", "测试");
        //中英文判断
        if (IsChinesePS(firstPlain))
        {
            ProcessChinesePS(botQQ, firstPlain, qq, nickName, groupId, sourceMessageId);
        }
        else if (IsPS(firstPlain))
        {
            ProcessPS(botQQ, firstPlain, qq, nickName, groupId, sourceMessageId);
        }
    }

    private void ProcessPS(long botQQ, string text, string qq, string nickName, string groupId, long sourceMessageId)
    { 
        LogUtil.LogInfo($"收到ps代码:\n{text}", nameof(PsModule<T>));
        var pss = text.Split("\n\n");
        if (pss.Length > 1)
        {
            new MiraiQQTrade<T>(botQQ, qq, nickName, groupId, sourceMessageId).StartTradeMultiPs(text);
        }
        else
        {
            new MiraiQQTrade<T>(botQQ, qq, nickName, groupId, sourceMessageId).StartTradePs(text);
        }
    }

    private void UpdateOrAddGameTradeOTInfo<TGameOTInfo>(string qq, TGameOTInfo gameOTInfo) where TGameOTInfo : GameOTInfo 
    {
        GameTradeOTInfo tradeInfo = new GameTradeOTInfo
        {
            Sid = gameOTInfo.Sid,
            Tid = gameOTInfo.Tid,
            OtGender = gameOTInfo.OtGender,
            OtName = gameOTInfo.OtName,
            Version = gameOTInfo.Version,
            OtLanguage = gameOTInfo.OtLanguage,
        };
        if (Common.GameOTInfoList.ContainsKey(qq))
        {
            Common.GameOTInfoList[qq] = tradeInfo;
        }
        else
        {
            Common.GameOTInfoList.Add(qq, tradeInfo);
        }
    }

    private async void ProcessChinesePS(long botQQ, string text, string qq, string nickName, string groupId, long sourceMessageId = 0)
    {
        await Task.Run(() =>
        {
            LogUtil.LogInfo($"收到中文ps代码:\n{text}", nameof(PsModule<T>));
            //var pss = text.Split("+");
            //if (pss.Length > 1)
            //{
            //    new MiraiQQTrade<T>(qq, nickName, groupId).StartTradeMultiChinesePs(text);
            //}
            //else
            //{
            string jsonText = CheckPokemonGroupGold(qq, groupId);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var response = JsonSerializer.Deserialize<APIResponse>(jsonText, options);
            if (response != null)
            {
                int repsonse_code = (int)response.Code;
                string? repsonse_msg = response.Msg ?? "";
                if (repsonse_code == 200)
                {

                    LogUtil.LogInfo($"{qq}-{repsonse_msg}", "测试");
                    if (response.Data != null)
                    {
                        PokemonData response_data = response.Data;
                        if (typeof(T) == typeof(PK8) && response_data.Swsh != null)
                        {
                            UpdateOrAddGameTradeOTInfo(qq, response_data.Swsh);
                        }
                        if (typeof(T) == typeof(PB8) && response_data.Bdsp != null)
                        {
                            UpdateOrAddGameTradeOTInfo(qq, response_data.Bdsp);
                        }
                        if (typeof(T) == typeof(PA8) && response_data.Pla != null)
                        {
                            UpdateOrAddGameTradeOTInfo(qq, response_data.Pla);
                        }
                        if (typeof(T) == typeof(PK9) && response_data.Sv != null)
                        {
                            UpdateOrAddGameTradeOTInfo(qq, response_data.Sv);
                        }
                    }

                    new MiraiQQTrade<T>(botQQ, qq, nickName, groupId, sourceMessageId).StartTradeChinesePs(text);
                }
                else
                {
                    MiraiQQBot<T>.SendGroupTextMessage(botQQ, groupId, repsonse_msg, sourceMessageId);
                    LogUtil.LogInfo($"{qq}-{repsonse_msg}", "测试");
                }

            }
        });
    }

    /// <summary>
    /// 检查熊熊币是否充足
    /// </summary>
    /// <param name="qq"></param>
    /// <param name="groupId"></param>
    /// <returns></returns>
    private static string CheckPokemonGroupGold(string qq, string groupId)
    {
        return HttpUtils.Post("https://api.17yohui.com/api/Report/checkPokemonGroupGold", $"qq={qq}&qq_group={groupId}&{Common.GetServerSignStr("checkPokemonGroupGold")}", "https://api.17yohui.com");
    }

    private static bool IsChinesePS(string str)
    {
        var gameStrings = ShowdownTranslator<T>.GameStringsZh;
        for (int i = 1; i < gameStrings.Species.Count; i++)
        {
            if (str.Contains(gameStrings.Species[i]))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsPS(string str)
    {
        var gameStrings = ShowdownTranslator<T>.GameStringsEn;
        for (int i = 1; i < gameStrings.Species.Count; i++)
        {
            if (str.Contains(gameStrings.Species[i]))
            {
                return true;
            }
        }
        return false;
    }
}
