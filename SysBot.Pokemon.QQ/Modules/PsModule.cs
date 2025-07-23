using Manganese.Array;
using Manganese.Text;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using PKHeX.Core;
using SysBot.Base;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

public class PsModule<T> : IModule where T : PKM, new()
{
    public bool? IsEnable { get; set; } = true;

    public void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();
        QQSettings settings = MiraiQQBot<T>.Settings;

        if (receiver.MessageChain.OfType<AtMessage>().All(x => x.Target != Convert.ToString((long)settings.QQ))) return;

        var text = receiver.MessageChain.OfType<PlainMessage>()?.FirstOrDefault()?.Text ?? "";
        if (string.IsNullOrWhiteSpace(text)) return;
        var qq = receiver.Sender.Id;
        var nickName = receiver.Sender.Name;
        var groupId = receiver.GroupId;
        LogUtil.LogInfo($"接受到消息：[{text}]", "测试");
        // 中英文判断
        if (IsChinesePS(text))
        {
            ProcessChinesePS(text, qq, nickName, groupId);
        }
        //else if (IsPS(text))
        //    ProcessPS(text, qq, nickName, groupId);
    }

    //private void ProcessPS(string text, string qq, string nickName, string groupId)
    //{
    //    LogUtil.LogInfo($"收到ps代码:\n{text}", nameof(PsModule<T>));
    //    var pss = text.Split("\n\n");
    //    if (pss.Length > 1)
    //        new MiraiQQTrade<T>(qq, nickName, groupId).StartTradeMultiPs(text);
    //    else
    //        new MiraiQQTrade<T>(qq, nickName, groupId).StartTradePs(text);
    //}

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
    private async void ProcessChinesePS(string text, string qq, string nickName, string groupId)
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
        LogUtil.LogInfo(jsonText, "测试");
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var response = JsonSerializer.Deserialize<APIResponse>(jsonText, options);
        if (response != null)
        {
            int repsonse_code = (int)response.Code;
            string? repsonse_msg = response.Msg;
            if (repsonse_code == 200)
            {
                
                LogUtil.LogInfo($"{qq}-{repsonse_msg}", "测试");
                if(response.Data != null)
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
                
                new MiraiQQTrade<T>(qq, nickName, groupId).StartTradeChinesePs(text);
            }
            else
            {
                await MessageManager.SendGroupOrTempMessageAsync(qq, groupId, repsonse_msg);
                LogUtil.LogInfo($"{qq}-{repsonse_msg}", "测试");
            }
            
        }
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

    //private static bool IsPS(string str)
    //{
    //    var gameStrings = ShowdownTranslator<T>.GameStringsEn;
    //    for (int i = 1; i < gameStrings.Species.Count; i++)
    //    {
    //        if (str.Contains(gameStrings.Species[i]))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}
