using PKHeX.Core;
using SysBot.Base;
using SysBot.Pokemon.Helpers;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SysBot.Pokemon.QQ;

public class MiraiQQTrade<T> : AbstractTrade<T> where T : PKM, new()
{
    private readonly string GroupId = default!;
    private readonly long BotQQ = default!;
    private readonly long MessageId = default!;
    public MiraiQQTrade(long botQQ, string qq, string nickName, string groupId, long message_id) 
    {
        SetPokeTradeTrainerInfo(new PokeTradeTrainerInfo(nickName, ulong.Parse(qq)));
        SetTradeQueueInfo(MiraiQQBot<T>.Info);
        GroupId = groupId;
        MessageId = message_id;
    }

    public override IPokeTradeNotifier<T> GetPokeTradeNotifier(T pkm, int code)
    {
        return new MiraiQQTradeNotifier<T>(pkm, userInfo, code, userInfo.TrainerName, GroupId, BotQQ, MessageId);
    }

    public override void SendMessage(string message)
    {
        MiraiQQBot<T>.SendGroupTextMessage(BotQQ, GroupId, message, MessageId);
    }

    public override void SendMessageWithImage(string message, string filePath)
    {
        MiraiQQBot<T>.SendGroupTextImageMessage(BotQQ, GroupId, message, filePath, MessageId);
    }

    public override void SendMessageWithImageBase64(string message, string base64)
    {
        MiraiQQBot<T>.SendGroupTextImageBase64Message(BotQQ, GroupId, message, base64, MessageId);
    }

    /// <summary>
    /// 上传用户的宝可梦
    /// </summary>
    /// <param name="qq"></param>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public static string GetUserPokemon(string qq, string groupId, T data)
    {
        var str = GameInfo.GetStrings("zh-Hans");
        var forms = FormConverter.GetFormList(data.Species, str.types, str.forms, GameInfo.GenderSymbolUnicode, data.Context);
        string formName = forms[data.Form];//当前形态名称
        return HttpUtils.Post("https://miraibot-admin.17yohui.com/PokemonHttpApi/showPKHeXUserPokemon", $"level={data.CurrentLevel}&pokemon_number={data.Species}&is_shiny={data.IsShiny}&formName={formName}&gender={data.Gender}&ability_id={data.Ability}&ball={data.Ball}&nature_id={(int)data.Nature}&iv_hp={data.IV_HP}&iv_atk={data.IV_ATK}&iv_def={data.IV_DEF}&iv_spa={data.IV_SPA}&iv_spd={data.IV_SPD}&iv_spe={data.IV_SPE}&charcter={data.Characteristic}&hold_item={data.HeldItem}&ot_name={data.HandlingTrainerName}&qq={qq}&qq_group={groupId}");
    }

    public override string GetPokemonInfo(T pkm)
    {
        string uploadJson = GetUserPokemon(userInfo.ID.ToString(), GroupId, pkm);
        string filePath = ""; // 初始化图片路径变量
        try
        {
            // 解析JSON到顶层响应模型
            var response = JsonSerializer.Deserialize<GetUserPokemonResponse>(uploadJson);

            if (response != null)
            {
                // 检查接口是否调用成功（假设200为成功状态码）
                if (response.Code == 200)
                {
                    filePath = response.Base64 ?? "";
                }
                else
                {
                    // 接口返回错误状态时，输出错误信息
                    Debug.WriteLine($"接口调用失败，错误码: {response.Code}，消息: {response.Message}");
                }
            }
            else
            {
                Debug.WriteLine("JSON解析失败，响应内容为空");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"处理响应时发生错误: {ex.Message}");
        }

        return filePath; // 返回提取到的图片路径（可能为空）
    }
}

// 定义顶层响应模型：包含code、data等字段
public class GetUserPokemonResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("base64")] // 对应data中的"base64"字段（图片路径）
    public string? Base64 { get; set; }

    [JsonPropertyName("msg")]
    public string? Message { get; set; } // 可选的消息字段
}
