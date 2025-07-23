using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace SysBot.Base
{
    /// <summary>
    /// 动态类，每个实例使用单独session
    /// </summary>
    public static class HttpUtils
    {

        /// <summary>
        /// 发送POST请求并返回HTML响应
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="postDataStr">请求的数据字符串</param>
        /// <param name="referer">请求头的Referer</param>
        /// <returns>HTML响应字符串</returns>
        public static async Task<string> PostAsync(string url, string postDataStr, string referer = "")
        {
            using (HttpClient client = new HttpClient())
            {
                // 设置HttpClientHandler以忽略SSL证书验证（注意：在生产环境中，不建议这样做）
                // 如果需要，可以使用ServerCertificateCustomValidationCallback
                // 但通常最好配置服务器以使用有效的SSL证书
                // client.Handler = new HttpClientHandler
                // {
                //     ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                // };

                // 设置请求头
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                if (!string.IsNullOrEmpty(referer))
                {
                    client.DefaultRequestHeaders.Referrer = new Uri(referer);
                }

                // 发送POST请求
                HttpContent content = new StringContent(postDataStr, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.PostAsync(url, content);

                // 确保请求成功
                response.EnsureSuccessStatusCode();

                // 读取并返回响应内容
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        // 同步包装器（不推荐，因为它会阻塞调用线程，但为了满足某些同步调用需求）
        public static string Post(string url, string postDataStr, string referer = "")
        {
            Task<string> task = PostAsync(url, postDataStr, referer);
            task.Wait(); // 这将阻塞当前线程，直到任务完成
            return task.Result;
        }
    }
}
