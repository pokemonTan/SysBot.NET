﻿namespace NapCatScript.Core.Services;

/// <summary>
/// 配置系统
/// </summary>
public static class Config
{
    public const string URI = "ws://39.108.178.212:3001/?access_token=G5.Zk.IqX.";
    public const string HttpURI = "http://39.108.178.212:3000/";
    public const string DeepSeekKey = "DeepSeekAPIKey";
    public const string RootId = "RootID";
    public const string BootId = "BootID";
    /// <summary>
    /// 配置文件名
    /// </summary>
    public static string ConfigName { get; } = "Cof.conf";
    private static Dictionary<string, string> AConfig { get; } = [];

    /// <summary>
    /// 创建配置文件
    /// </summary>
    /// <param name="confName"></param>
    /// <returns></returns>
    private static string CreateConfFile(string confName)
    {
        string confPath = Path.Combine(Environment.CurrentDirectory, confName);
        if (!File.Exists(confPath)) {
            File.Create(confPath).Dispose();
        }
        return confPath;
    }

    /// <summary>
    /// 创建空配置项
    /// </summary>
    private static void CreateConf(string confName)
    {
        LoadConf();
        if (AConfig.TryGetValue(confName, out _))//存在
            return;
        AConfig.Add(confName, "");
        WriteFile();
    }

    /// <summary>
    /// 加载配置项
    /// </summary>
    private static void LoadConf()
    {
        CreateConfFile(ConfigName);
        AConfig.Clear();
        string allText = File.ReadAllText(CreateConfFile(ConfigName));
        string[] allconf = allText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries/*"\r\n"*/);
        for (int i = 0; i < allconf.Length; i++) {
            string c = allconf[i];
            if (string.IsNullOrEmpty(c))
                continue;

            string[] oneConf = c.Split("=");
            AConfig.Add(oneConf[0].Trim(), oneConf[1].Trim());
        }
    }

    /// <summary>
    /// 获取配置值, 不存在则创建
    /// </summary>
    /// <param name="confName"> 配置名 </param>
    public static string? GetConf(string confName)
    {
        LoadConf();
        if (AConfig.TryGetValue(confName, out string? confsizer)) {
            Console.WriteLine(confName + " => " + confsizer);
            return confsizer ??= string.Empty;
        }
        CreateConf(confName);
        return string.Empty;
    }

    /// <summary>
    /// 设置配置值
    /// </summary>
    /// <param name="confName"> 项目名 </param>
    /// <param name="confValue"> 项目值 </param>
    public static void SetConf(string confName, string confValue)
    {
        LoadConf();
        string? conf = GetConf(confName);
        if (conf is null || conf == string.Empty) {
            AConfig.Add(confName, confValue);
            WriteFile();
        } else {
            AConfig.Remove(confName);
            AConfig.Add(confName, confValue);
            WriteFile();
        }
    }

    /// <summary>
    /// 将配置项写入配置文件
    /// </summary>
    /// <exception cref="Exception"></exception>
    private static void WriteFile()
    {
        //LoadConf();
        string filePath = CreateConfFile(ConfigName);
        try {
            File.Delete(filePath);
        } catch {
            throw new Exception("配置文件写失败");
        }

        StreamWriter file = File.AppendText(filePath);
        foreach (var kv in AConfig) {
            string 配置项 = kv.Key;
            string 配置值 = kv.Value;
            file.WriteLine(配置项+"="+配置值);
        }
        file.Dispose();
    }

    /// <summary>
    /// 字符串转字节数组
    /// </summary>
    public static byte[] StringToBytes(string value)
    {
        List<byte> bytes = [];
        foreach (var cr in value) {
            bytes.Add(Convert.ToByte(cr));
        }
        return [.. bytes];
    }
}
