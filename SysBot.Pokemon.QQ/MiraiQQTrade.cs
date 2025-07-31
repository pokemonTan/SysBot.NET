using PKHeX.Core;
using SysBot.Base;
using SysBot.Pokemon.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
        BotQQ = botQQ;
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
    public string GetUserPokemon(T data)
    {
        var str = GameInfo.GetStrings("zh-Hans");
        var forms = FormConverter.GetFormList(data.Species, str.types, str.forms, GameInfo.GenderSymbolUnicode, data.Context);
        string formName = forms[data.Form];//当前形态名称
        int hp_st = 0;
        int attack_st = 0;
        int defense_st = 0;
        int special_attack_st = 0;
        int special_defense_st = 0;
        int speed_st = 0;
        ushort CheckSum = data is ISanityChecksum s ? s.Checksum : Checksums.CRC16_CCITT(data.Data.AsSpan(data.SIZE_STORED));

        var locationName = str.GetLocationName(data.WasEgg, data.MetLocation, data.Format, data.Generation, data.Version);

        if (data is IHyperTrain h)
        {
            for (int i = 0; i < 6; i++)
            {
                if (h.IsHyperTrained(i))
                {
                    if(i == 0)
                    {
                        hp_st = 1;
                    }else if (i == 1)
                    {
                        attack_st = 1;
                    }
                    else if (i == 2)
                    {
                        defense_st = 1;
                    }
                    else if (i == 3)
                    {
                        special_attack_st = 1;
                    }
                    else if (i == 4)
                    {
                        special_defense_st = 1;
                    }
                    else if (i == 5)
                    {
                        speed_st = 1;
                    }
                }
                    
            }
        }
        return HttpUtils.Post("https://miraibot-admin.17yohui.com/PokemonHttpApi/getPKHeXUserPokemon", $"level={data.CurrentLevel}" +
            $"&pokemon_number={data.Species}" +
            $"&isShiny={data.IsShiny}" +
            $"&formName={formName}" +
            $"&gender={data.Gender}" +
            $"&ability_id={data.Ability}" +
            $"&ball={data.Ball}" +
            $"&nature_id={(int)data.Nature}" +
            $"&mint_nature_id={(int)data.StatNature}" +
            $"&iv_hp={data.IV_HP}" +
            $"&iv_atk={data.IV_ATK}" +
            $"&iv_def={data.IV_DEF}" +
            $"&iv_spa={data.IV_SPA}" +
            $"&iv_spd={data.IV_SPD}" +
            $"&iv_spe={data.IV_SPE}" +
            $"&hp_st={hp_st}" +
            $"&attack_st={attack_st}" +
            $"&defense_st={defense_st}" +
            $"&special_attack_st={special_attack_st}" +
            $"&special_defense_st={special_defense_st}" +
            $"&speed_st={speed_st}" +
            $"&charcter={data.Characteristic}" +
            $"&hold_item={data.HeldItem}" +
            $"&HandlingTrainerName={data.HandlingTrainerName}" +
            $"&senderId={userInfo.ID.ToString()}" +
            $"&groupId={GroupId}" +
            $"&botQQ={BotQQ}" +
            $"&sourceMessageId={MessageId}" +
            $"&groupNickname={userInfo.TrainerName}" +
            $"&pid={data.PID.ToString("X8")}" +
            $"&EC={data.EncryptionConstant.ToString("X8")}" +
            $"&CheckSum={CheckSum.ToString("X8")}" +
            $"&pokemonLanguage={data.Language}" +
            $"&now_experience={data.EXP}" +
            $"&ev_hp={data.EV_HP}" +
            $"&ev_atk={data.EV_ATK}" +
            $"&ev_def={data.EV_DEF}" +
            $"&ev_spa={data.EV_SPA}" +
            $"&ev_spd={data.EV_SPD}" +
            $"&ev_spe={data.EV_SPE}" +
            $"&move1={data.Move1}" +
            $"&move2={data.Move2}" +
            $"&move3={data.Move3}" +
            $"&move4={data.Move4}" +
            $"&move1_PP={data.Move1_PP}" +
            $"&move2_PP={data.Move2_PP}" +
            $"&move3_PP={data.Move3_PP}" +
            $"&move4_PP={data.Move4_PP}" +
            $"&height={data.PersonalInfo.Height}" +
            $"&weight={data.PersonalInfo.Weight}" +
            $"&meetDate={data.MetDate}" +
            $"&meetLocation={locationName}" +
            $"&meetLevel={data.MetLevel}" +
            $"&OriginalTrainerName={data.OriginalTrainerName}" +
            $"&SID16={data.SID16}" +
            $"&TID16={data.TID16}" +
            $"&OriginalTrainerFriendship={data.OriginalTrainerFriendship}");
    }

    public override string GetPokemonInfo(T pkm)
    {
        string uploadJson = GetUserPokemon(pkm);
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
        catch (AggregateException ex)
        {
            Debug.WriteLine($"处理响应时发生错误: {ex.Message}");
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
