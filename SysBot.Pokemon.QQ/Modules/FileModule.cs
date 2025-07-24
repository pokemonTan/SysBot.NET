using PKHeX.Core;
using SysBot.Base;
using SysBot.Pokemon.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SysBot.Pokemon.QQ;

public class FileModule<T> where T : PKM, new()
{
    public bool? IsEnable { get; set; } = true;

    public async void Execute()
    {
        //QQSettings settings = MiraiQQBot<T>.Settings;

        //var receiver = @base.Concretize<GroupMessageReceiver>();

        //var senderQQ = receiver.Sender.Id;
        //var nickname = receiver.Sender.Name;
        //var groupId = receiver.Sender.Group.Id;

        //var fileMessage = receiver.MessageChain.OfType<FileMessage>()?.FirstOrDefault();
        //if (fileMessage == null) return;
        //LogUtil.LogInfo("检测到文件上传", nameof(FileModule<T>));
        //var fileName = fileMessage.Name;
        //if (!FileTradeHelper<T>.ValidFileName(fileName) || !FileTradeHelper<T>.ValidFileSize(fileMessage.Size))
        //{
        //    await MessageManager.SendGroupMessageAsync(groupId, "非法文件");
        //    return;
        //}

        //List<T> pkms = default!;
        //try
        //{
        //    var f = await FileManager.GetFileAsync(groupId, fileMessage.FileId, true);
        //    using var client = new HttpClient();
        //    byte[] data = client.GetByteArrayAsync(f.DownloadInfo.Url).Result;
        //    pkms = FileTradeHelper<T>.Bin2List(data);
        //    await FileManager.DeleteFileAsync(groupId, fileMessage.FileId);
        //}
        //catch (Exception ex)
        //{
        //    LogUtil.LogError(ex.Message, nameof(FileModule<T>));
        //    return;
        //}
        //if (pkms.Count > 1 && pkms.Count <= FileTradeHelper<T>.MaxCountInBin)
        //    new MiraiQQTrade<T>(senderQQ, nickname, groupId).StartTradeMultiPKM(pkms);
        //else if (pkms.Count == 1)
        //    new MiraiQQTrade<T>(senderQQ, nickname, groupId).StartTradePKM(pkms[0]);
        //else
        //    await MessageManager.SendGroupMessageAsync(groupId, "文件内容不正确");
    }
}
