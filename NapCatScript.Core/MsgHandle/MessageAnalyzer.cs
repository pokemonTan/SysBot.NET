using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapCatScript.Core.MsgHandle
{
    // 定义消息链解析结果的结构
    public class MessageChainResult
    {
        public string FirstPlain { get; set; } = "";
        public List<JsonElement> List { get; set; } = new List<JsonElement>();
        public Boolean IsAtRobot { get; set; } = false;

        // 消息解析方法
        public MessageChainResult AnalysisMessageChain(long BotQQ, JsonElement messageChain)
        {
            var result = new MessageChainResult();
            var tempList = new List<JsonElement>();

            // 遍历消息链中的每个元素
            for (int i = 0; i < messageChain.GetArrayLength(); i++)
            {
                var element = messageChain[i];

                // 检查是否为@类型且@的是机器人
                if (element.TryGetProperty("type", out JsonElement typeElem)
                    && typeElem.GetString() == "at"
                    && element.TryGetProperty("data", out JsonElement dataElem)
                    && dataElem.TryGetProperty("qq", out JsonElement qqElem))
                {
                    string qq = qqElem.GetString();
                    
                    if (qq == BotQQ.ToString()) // 假设RealRobotList是存储机器人QQ的列表
                    {
                        IsAtRobot = true;
                        continue; // 跳过添加到临时列表，相当于unset
                    }
                }

                // 检查是否为空文本
                if (element.TryGetProperty("type", out typeElem)
                    && typeElem.GetString() == "text"
                    && element.TryGetProperty("data", out dataElem)
                    && dataElem.TryGetProperty("text", out JsonElement textElem))
                {
                    string text = textElem.GetString() ?? "";
                    string mergedText = MergeSpaces(text); // 合并空格的方法
                    if (mergedText == " ")
                    {
                        continue; // 跳过空文本
                    }

                    // 记录第一个非空文本
                    if (string.IsNullOrEmpty(result.FirstPlain))
                    {
                        result.FirstPlain = mergedText;
                    }
                }

                // 添加到临时列表
                tempList.Add(element);
            }

            // 赋值处理后的列表
            result.List = tempList;
            return result;
        }

        // 合并空格的辅助方法，类似PHP中的merge_spaces
        private string MergeSpaces(string text)
        {
            // 替换多个空格为单个空格，并去除首尾空格
            return System.Text.RegularExpressions.Regex.Replace(text.Trim(), @"\s+", " ");
        }
    }
}
