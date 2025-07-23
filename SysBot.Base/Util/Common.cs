using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleJSON;
//using OpenCvSharp;
//using Sdcb.PaddleInference;
//using Sdcb.PaddleOCR.Models;
//using Sdcb.PaddleOCR;
//using Sdcb.PaddleOCR.Models.Local;
using System.Diagnostics;

namespace SysBot.Base
{
    public class GameTradeOTInfo
    {
        public int Version { get; set; }

        public string OtName { get; set; } = "";

        public int OtGender { get; set; }

        public int OtLanguage { get; set; }

        public int Sid { get; set; }

        public int Tid { get; set; }
    }

    public static class Common
    {
        public static Dictionary<string, GameTradeOTInfo> GameOTInfoList = new Dictionary<string, GameTradeOTInfo>();
        public static string ConfigPath = "";
        public static int SoftwareId = 4;//软件ID
        public static int ExchangeMode = 0;//交换模式
        private static int gameVersion = 45;//游戏版本，44剑，45盾
        public static int GameVersion
        {
            get { return gameVersion; }
            set { gameVersion = value; }
        }

        private static int maxAttempts = 2000000000;//最大尝试次数
        public static int MaxAttempts
        {
            get { return maxAttempts; }
            set { maxAttempts = value; }
        }

        private static int isUpdateWildAreaEvent = 0;//是否更新旷野地带新闻，1是0否
        public static int IsUpdateWildAreaEvent
        {
            get { return isUpdateWildAreaEvent; }
            set { isUpdateWildAreaEvent = value; }
        }

        private static int oT_Language = 0;//初训家语言，默认没有设置
        public static int OT_Language
        {
            get { return oT_Language; }
            set { oT_Language = value; }
        }

        private static string oT_Name = "";//初训家昵称,贵重球16，究极球26
        public static string OT_Name
        {
            get { return oT_Name; }
            set { oT_Name = value; }
        }

        private static string hT_Name = "";//最近持有人昵称
        public static string HT_Name
        {
            get { return hT_Name; }
            set { hT_Name = value; }
        }

        private static int hT_Gender = 0;//最近持有人性别,0男1女
        public static int HT_Gender
        {
            get { return hT_Gender; }
            set { hT_Gender = value; }
        }

        private static int hT_Language = 9;//最近持有人语言，简体中文CHS为9
        public static int HT_Language
        {
            get { return hT_Language; }
            set { hT_Language = value; }
        }

        private static int trainerSID7 = 0;//初训家里ID
        public static int TrainerSID7
        {
            get { return trainerSID7; }
            set { trainerSID7 = value; }
        }

        private static int trainerTID7 = 0;//初训家表ID
        public static int TrainerTID7
        {
            get { return trainerTID7; }
            set { trainerTID7 = value; }
        }

        private static int oT_Gender = 0;//初训家性别,0男1女
        public static int OT_Gender
        {
            get { return oT_Gender; }
            set { oT_Gender = value; }
        }

        private static int defaultBall = 26;//默认球种
        public static int DefaultBall
        {
            get { return defaultBall; }
            set { defaultBall = value; }
        }

        private static int defaultFullIv = 1;//默认6v,1是0否
        public static int DefaultFullIv
        {
            get { return defaultFullIv; }
            set { defaultFullIv = value; }
        }

        private static int defaultHeldItem = 1606;//默认持有物品
        public static int DefaultHeldItem
        {
            get { return defaultHeldItem; }
            set { defaultHeldItem = value; }
        }

        private static bool hasBDSPCache = false;//是否有珍钻缓存
        public static bool HasBDSPCache
        {
            get { return hasBDSPCache; }
            set { hasBDSPCache = value; }
        }

        private static bool hasSWSHCache = false;//是否有剑盾缓存
        public static bool HasSWSHCache
        {
            get { return hasSWSHCache; }
            set { hasSWSHCache = value; }
        }

        private static bool hasArceusCache = false;//是否有阿尔宙斯缓存
        public static bool HasArceusCache
        {
            get { return hasArceusCache; }
            set { hasArceusCache = value; }
        }

        //772属性空,773银伴战兽,891熊徒弟,892武道熊师,151梦幻,292脱壳忍者,722-730七代御三家,810-818八代御三家,880-883剑盾化石宝可梦
        private static string[] specialBallPM = new string[] { };//特殊球种精灵
        public static string[] SpecialBallPM
        {
            get { return specialBallPM; }
            set { specialBallPM = value; }
        }

        private static string[] greatAdventurePM = new string[] { };//极巨大冒险神兽
        public static string[] GreatAdventurePM
        {
            get { return greatAdventurePM; }
            set { greatAdventurePM = value; }
        }

        private static string[] shinyEggList = new string[] { };//闪蛋列表
        public static string[] ShinyEggList
        {
            get { return shinyEggList; }
            set { shinyEggList = value; }
        }

        private static string[] femalePM = new string[] { };//性别只有雌性的精灵
        public static string[] FemalePM
        {
            get { return femalePM; }
            set { femalePM = value; }
        }

        private static string[] notModifyOTPM = new string[] { };//不能修改初训家的精灵
        public static string[] NotModifyOTPM
        {
            get { return notModifyOTPM; }
            set { notModifyOTPM = value; }
        }

        private static string[] notShinyPM = new string[] { };//不能闪光的精灵
        public static string[] NotShinyPM
        {
            get { return notShinyPM; }
            set { notShinyPM = value; }
        }

        private static string[] reversalFormPM = new string[] { };//形态需要反转的精灵
        public static string[] ReversalFormPM
        {
            get { return reversalFormPM; }
            set { reversalFormPM = value; }
        }

        private static NameValueCollection pokemonAttribute = new NameValueCollection{ };//宝可梦属性列表
        public static NameValueCollection PokemonAttribute
        {
            get { return pokemonAttribute; }
            set { pokemonAttribute = value; }
        }

        private static NameValueCollection ballItemList = new NameValueCollection { };//球种的道具编号列表
        public static NameValueCollection BallItemList
        {
            get { return ballItemList; }
            set { ballItemList = value; }
        }

        private static NameValueCollection dittoList = new NameValueCollection { };//百变怪列表
        public static NameValueCollection DittoList
        {
            get { return dittoList; }
            set { dittoList = value; }
        }

        private static NameValueCollection wildAreaEvent = new NameValueCollection { };//旷野地带新闻
        public static NameValueCollection WildAreaEvent
        {
            get { return wildAreaEvent; }
            set { wildAreaEvent = value; }
        }        

        /// <summary>
        /// 判断元素是否在数组中
        /// </summary>
        /// <param name="str">要查找的字符串元素</param>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static bool inArray(string str, string[] strArray)
        {
            for (int i = 0; i < strArray.Length; i++)
            {
                if (str.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取当前dll文件名
        /// </summary>
        /// <returns></returns>
        public static string GetDllFileName()
        {
            return  System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// 获取当前dll版本号
        /// </summary>
        /// <returns></returns>
        private static string GetDllVersion()
        {
            string filename = GetDllFileName();
            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
            return $"{fileVersionInfo.FileMajorPart}.{fileVersionInfo.FileMinorPart}.{fileVersionInfo.FileBuildPart}.{fileVersionInfo.FilePrivatePart}";
        }

        //获取服务器签名字符串
        public static string GetServerSignStr(string route)
        {
            long now_time = GetTimeStamp();
            string version_code = "1.0.0.10";
            string sign = MD5String(String.Format("{0}{1}17yohuiPostServer", route, now_time)).ToLower();
            return String.Format("route={0}&time={1}&sign={2}&software_id={3}&version_code={4}", route, now_time, sign, SoftwareId, version_code);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp(int length = 10)
        {
            if (length == 10)
            {
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000; //10位
            }
            else
            {
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000; //13位
            }
        }

        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <returns>密文</returns>
        public static string MD5String(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text); // 使用UTF8编码，这通常是更好的选择
            try
            {
                using (MD5 md5 = MD5.Create()) // 使用MD5.Create()来创建MD5实例
                {
                    byte[] hashBytes = md5.ComputeHash(buffer);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("X2")); // 使用X2来确保每个字节都被格式化为两位十六进制数
                    }
                    return sb.ToString().ToLower();
                }
            }
            catch (Exception ex)
            {
                // 在这里处理异常，例如记录日志或重新抛出异常
                throw new ApplicationException("An error occurred while computing the MD5 hash.", ex);
            }
        }

        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary>
        /// 检测本机是否联网
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedInternet()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
            {
                return true;//已联网
            }
            else
            {
                return false;//未联网
            }
        }

        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLog(string sMsg)
        {
            if (sMsg != "")
            {
                string filename = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                try
                {
                    string path = GetSoftwareInstallPath() + "\\Logs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileInfo fi = new FileInfo(path + "\\" + filename);
                    if (!fi.Exists)
                    {
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now + "\n " + sMsg + "\n");
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now + "\n " + sMsg + "\n");
                            sw.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="NewFileName"></param>
        public static bool RenameFile(string FileName, string NewFileName)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    return false;
                }
                else
                {
                    if (File.Exists(NewFileName))
                    {
                        File.Delete(NewFileName);
                        Thread.Sleep(1000);
                    }
                    File.Move(FileName, NewFileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLog("文件重命名失败,错误码：" + ex.HResult + ",错误信息：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取文件MD5加密信息
        /// </summary>
        /// <param name="strFileFullPath"></param>
        /// <returns></returns>
        public static string GetFileMd5Hash(string strFileFullPath)
        {
            try
            {
                using (FileStream fst = new FileStream(strFileFullPath, FileMode.Open, FileAccess.Read))
                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(fst);
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    return sBuilder.ToString().ToLower();
                }
            }
            catch (Exception ex)
            {
                WriteLog($"获取文件MD5失败, 错误码：{ex.HResult}, 错误信息：{ex.Message}");
                return "";
            }
        }

        //获取软件安装路径
        public static string GetSoftwareInstallPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="bytes">字节值</param>
        /// <returns></returns>
        public static string FormatBytes(double bytes)
        {
            String[] units = new String[] { "B", "KB", "M", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            int reserve = bytes < 1024 ? 0 : (bytes > 1024 && bytes < 10240) ? 2 : 1;
            while (bytes >= mod)
            {
                bytes /= mod;
                i++;
            }
            return Math.Round(bytes, reserve) + units[i];
        }

        private static CancellationToken publicToken;//取消Token
        public static CancellationToken PublicToken
        {
            get { return publicToken; }
            set { publicToken = value; }
        }

        private static string[] robotDealBlackList = new string[] { };//机器人处理黑名单
        public static string[] RobotDealBlackList
        {
            get { return robotDealBlackList; }
            set { robotDealBlackList = value; }
        }

        private static string[] groupIdList = new string[] { };//QQ群ID列表，用逗号分割
        public static string[] GroupIdList
        {
            get { return groupIdList; }
            set { groupIdList = value; }
        }
        /// <summary>
        /// 获取机器人信息
        /// </summary>
        public async static void GetRobotInfo()
        {
            await Task.Run(() =>
            {
                string server_sign_str = Common.GetServerSignStr("GetRobotInfo");
                string serverJsonText = HttpUtils.Post("https://api.17yohui.com/api/Report/GetRobotInfo", server_sign_str, "https://api.17yohui.com");
                if (!string.IsNullOrWhiteSpace(serverJsonText))
                {
                    JSONNode server_result = JSON.Parse(serverJsonText);
                    JSONNode data = server_result["data"];
                    JSONArray RobotDealBlackList = data["RobotDealBlackList"].AsArray;
                    Common.RobotDealBlackList = new string[RobotDealBlackList.Count];
                    for (int i = 0; i < RobotDealBlackList.Count; i++)
                    {
                        Common.RobotDealBlackList[i] = RobotDealBlackList[i];
                    }
                    JSONArray GroupIdList = data["GroupIdList"].AsArray;
                    Common.GroupIdList = new string[GroupIdList.Count];
                    for (int i = 0; i < GroupIdList.Count; i++)
                    {
                        Common.GroupIdList[i] = GroupIdList[i];
                    }
                    LogUtil.LogInfo($"{String.Join(",", Common.RobotDealBlackList)}", "录入黑名单");
                    LogUtil.LogInfo($"{String.Join(",", Common.GroupIdList)}", "监听群组列表");
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// OCR识别图片中的中文
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        //public static async Task<string> ImageOCRChinese(string filePath)
        //{
        //    FullOcrModel model = LocalFullModels.ChineseV3;
        //    Stopwatch stopwatch = new Stopwatch();
        //    try
        //    {
        //        stopwatch.Start(); // 开始计时
        //        string ocrResult = await Task.Run(() =>
        //        {
        //            PaddleOcrAll ?paddleOcr = default;
        //            try
        //            {
        //                paddleOcr = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
        //                {
        //                    AllowRotateDetection = true,
        //                    Enable180Classification = false,
        //                };

        //                using (Mat src = Cv2.ImRead(filePath, ImreadModes.Color))
        //                {
        //                    if (src.Empty())
        //                    {
        //                        throw new FileNotFoundException("无法加载图像文件", filePath);
        //                    }

        //                    PaddleOcrResult result = paddleOcr.Run(src);

        //                    //foreach (PaddleOcrResultRegion region in result.Regions)
        //                    //{
        //                    //    LogUtil.LogInfo($"Text: {region.Text}, Score: {region.Score}, BoundingBoxCenter: {region.Rect.Center}, BoundingBoxSize: {region.Rect.Size}, Angle: {region.Rect.Angle}", "OCR识别");
        //                    //}

        //                    return result.Text;
        //                }
        //            }
        //            finally
        //            {
        //                paddleOcr?.Dispose(); // 确保在完成后释放资源
        //            }
        //        });
        //        stopwatch.Stop(); // 停止计时
        //        return ocrResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        stopwatch.Stop(); // 停止计时
        //        LogUtil.LogError($"OCR 识别过程中发生错误: {ex.Message},耗时{stopwatch.ElapsedMilliseconds}毫秒", "OCR识别");
        //        // 根据需要处理异常，例如返回 null 或空字符串
        //        return ""; // 或者 throw; 如果您想要让调用者知道发生了错误
        //    }
        //}

        /// <summary>
        /// 包含是否同时包含多个字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="substrings"></param>
        /// <returns></returns>
        public static bool ContainsAllSubstrings(string input, string[] substrings)
        {
            return substrings.All(substring => input.Contains(substring));
        }

        public static void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);//删除文件
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo($"删除文件[{filePath}]发生错误: {ex.Message}", "删除文件");
            }
        }

    }
}
