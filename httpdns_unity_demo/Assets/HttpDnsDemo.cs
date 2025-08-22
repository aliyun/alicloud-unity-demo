using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Aliyun.HttpDns;

public class HttpDnsDemo : MonoBehaviour
{
    [Header("UI组件")]
    public InputField urlInputField;
    public Button dnsResolveButton;
    public Button unityWebRequestButton;
    public Button httpWebRequestButton;
    public Button httpClientButton;
    public Button clearButton;
    public Text resultText;
    public ScrollRect resultScrollRect;

    [Header("配置")]
    public string accountId = "YOUR_ACCOUNT_ID";
    public string secret = "YOUR_SECRET_KEY";

    private static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
    private string lastResolvedHost = "";
    private string lastResolvedIP = "";

    void Start()
    {
        Debug.Log("=== HttpDNS Demo Started ===");
        
        // Display platform information
        string platform = "Unknown";
#if UNITY_ANDROID && !UNITY_EDITOR
        platform = "Android";
#elif UNITY_IOS && !UNITY_EDITOR
        platform = "iOS";
#else
        platform = "Editor/Desktop (using C SDK)";
#endif
        
        AppendResult($"平台: {platform}");
        
        HttpDnsHelper.debug(true);
        
        // Staged initialization pattern
        HttpDnsHelper.init(accountId, secret); // Stage 1: save parameters
        HttpDnsHelper.setHttpsRequestEnabled(true);
        HttpDnsHelper.setTimeout(3000);
        
        // New enhanced features
        HttpDnsHelper.setPersistentCacheIPEnabled(true, 3600); // Enable persistent cache for 1 hour
        HttpDnsHelper.setReuseExpiredIPEnabled(true); // Enable expired IP reuse
        HttpDnsHelper.setPreResolveAfterNetworkChanged(true); // Auto pre-resolve after network change
        
        // Stage 2: build the service
        bool buildSuccess = HttpDnsHelper.build();
        AppendResult($"HttpDNS 分阶段构建: {(buildSuccess ? "成功" : "失败")}");
        
        // Get session ID for debugging
        string sessionId = HttpDnsHelper.getSessionId();
        if (!string.IsNullOrEmpty(sessionId))
        {
            AppendResult($"会话ID: {sessionId}");
        }
        
        AppendResult("HttpDNS 初始化完成");
        
        // 设置默认URL
        if (urlInputField != null)
        {
            urlInputField.text = "https://cube.elemecdn.com/favicon.ico";
        }
        
        // 绑定按钮事件
        if (dnsResolveButton != null)
            dnsResolveButton.onClick.AddListener(OnDnsResolveClicked);
        if (unityWebRequestButton != null)
            unityWebRequestButton.onClick.AddListener(Button1_UnityWebRequest);
        if (httpWebRequestButton != null)
            httpWebRequestButton.onClick.AddListener(Button2_HttpWebRequest);
        if (httpClientButton != null)
            httpClientButton.onClick.AddListener(Button3_HttpClient);
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearResults);
    }

    private void AppendResult(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string logMessage = $"[{timestamp}] {message}";
        
        if (resultText != null)
        {
            resultText.text += logMessage + "\n";
            
            // 自动滚动到底部
            if (resultScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                resultScrollRect.verticalNormalizedPosition = 0f;
            }
        }
        
        Debug.Log(logMessage);
    }

    private void ClearResult()
    {
        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    public void ClearResults()
    {
        if (resultText != null)
        {
            resultText.text = "结果已清空...";
            Debug.Log("结果已清空");
        }
        else
        {
            Debug.LogWarning("无法清空结果：resultText为null");
        }
    }

    public void OnDnsResolveClicked()
    {
        if (urlInputField == null || string.IsNullOrEmpty(urlInputField.text))
        {
            AppendResult("错误: 请输入有效的URL");
            return;
        }

        try
        {
            var uri = new Uri(urlInputField.text);
            string host = uri.Host;
            
            AppendResult($"开始解析域名: {host}");
            
            // Standard resolution (backward compatibility)
            var ips = HttpDnsHelper.getIpsByHostAsync(host);
            if (ips != null && ips.Count > 0)
            {
                lastResolvedHost = host;
                lastResolvedIP = ips[0];
                AppendResult($"HttpDNS解析成功: {host} -> {ips[0]} (共找到{ips.Count}个IP)");
                
                for (int i = 1; i < ips.Count && i < 3; i++)
                {
                    AppendResult($"  备用IP: {ips[i]}");
                }
                
                // Demonstrate enhanced resolution
                var enhancedResult = HttpDnsHelper.resolveHostSyncNonBlocking(host, "auto");
                if (enhancedResult != null)
                {
                    AppendResult($"增强解析结果 - IPv4: {enhancedResult.IPv4.Count}个, IPv6: {enhancedResult.IPv6.Count}个");
                    if (enhancedResult.TTL > 0)
                    {
                        AppendResult($"TTL: {enhancedResult.TTL}秒");
                    }
                }
            }
            else
            {
                AppendResult($"警告: HttpDNS未返回IP，将使用系统DNS");
                string systemIP = ResolveWithSystemDns(host);
                lastResolvedHost = host;
                lastResolvedIP = systemIP;
            }
            
            // Demonstrate batch pre-resolution
            var hostsToPreResolve = new List<string> { "www.aliyun.com", "ecs.console.aliyun.com" };
            HttpDnsHelper.setPreResolveHosts(hostsToPreResolve, "auto");
            AppendResult($"批量预解析 {hostsToPreResolve.Count} 个域名");
        }
        catch (Exception e)
        {
            AppendResult($"错误: {e.Message}");
        }
    }

    public void Button1_UnityWebRequest()
    {
        AppendResult("=== 开始 UnityWebRequest 请求 ===");
        StartCoroutine(DownloadWithUnityWebRequest());
    }

    public void Button2_HttpWebRequest()
    {
        AppendResult("=== 开始 HttpWebRequest 请求 ===");
        StartCoroutine(DownloadWithHttpWebRequest());
    }

    public void Button3_HttpClient()
    {
        AppendResult("=== 开始 HttpClient 请求 ===");
        DownloadWithHttpClientAsync();
    }

    private IEnumerator DownloadWithUnityWebRequest()
    {
        if (string.IsNullOrEmpty(lastResolvedHost) || string.IsNullOrEmpty(lastResolvedIP))
        {
            AppendResult("错误: 请先进行DNS解析");
            yield break;
        }

        string url = urlInputField.text;
        var uri = new Uri(url);
        string newUrl = url.Replace(uri.Host, lastResolvedIP);
        
        AppendResult($"UnityWebRequest URL: {newUrl}");
        AppendResult($"设置Host头: {uri.Host}");

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(newUrl))
        {
            request.SetRequestHeader("Host", uri.Host);
            
            float startTime = Time.time;
            yield return request.SendWebRequest();
            float duration = Time.time - startTime;

            if (request.result == UnityWebRequest.Result.Success)
            {
                AppendResult($"✅ UnityWebRequest 成功! 耗时: {duration:F2}秒");
                AppendResult($"响应大小: {request.downloadedBytes} bytes");
                
                // 显示响应头信息
                var responseHeaders = request.GetResponseHeaders();
                if (responseHeaders != null)
                {
                    foreach (var header in responseHeaders)
                    {
                        AppendResult($"响应头: {header.Key}: {header.Value}");
                    }
                }
            }
            else
            {
                AppendResult($"❌ UnityWebRequest 失败: {request.error}");
                AppendResult($"响应码: {request.responseCode}");
            }
        }
    }

    private IEnumerator DownloadWithHttpWebRequest()
    {
        if (string.IsNullOrEmpty(lastResolvedHost) || string.IsNullOrEmpty(lastResolvedIP))
        {
            AppendResult("错误: 请先进行DNS解析");
            yield break;
        }

        string url = urlInputField.text;
        var uri = new Uri(url);
        string newUrl = url.Replace(uri.Host, lastResolvedIP);
        
        AppendResult($"HttpWebRequest URL: {newUrl}");
        AppendResult($"设置Host头: {uri.Host}");

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newUrl);
        request.Method = "GET";
        request.Host = uri.Host;

        bool requestCompleted = false;
        HttpWebResponse response = null;
        Exception requestException = null;
        float startTime = Time.time;
        
        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                requestCompleted = true;
            }
            catch (Exception e)
            {
                requestException = e;
                requestCompleted = true;
            }
        });
        
        yield return new WaitUntil(() => requestCompleted);
        float duration = Time.time - startTime;
        
        if (response != null)
        {
            try
            {
                using (response)
                using (Stream stream = response.GetResponseStream())
                {
                    byte[] data = ReadStreamToBytes(stream);
                    
                    AppendResult($"✅ HttpWebRequest 成功! 耗时: {duration:F2}秒");
                    AppendResult($"响应大小: {data?.Length ?? 0} bytes");
                    AppendResult($"响应码: {(int)response.StatusCode} {response.StatusDescription}");
                    AppendResult($"Content-Type: {response.ContentType}");
                }
            }
            catch (Exception e)
            {
                AppendResult($"❌ HttpWebRequest 处理失败: {e.Message}");
            }
        }
        else if (requestException != null)
        {
            AppendResult($"❌ HttpWebRequest 失败: {requestException.Message}");
        }
    }

    private async void DownloadWithHttpClientAsync()
    {
        if (string.IsNullOrEmpty(lastResolvedHost) || string.IsNullOrEmpty(lastResolvedIP))
        {
            AppendResult("错误: 请先进行DNS解析");
            return;
        }

        string url = urlInputField.text;
        var uri = new Uri(url);
        string newUrl = url.Replace(uri.Host, lastResolvedIP);
        
        AppendResult($"HttpClient URL: {newUrl}");
        AppendResult($"设置Host头: {uri.Host}");

        try
        {
            using (var requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, newUrl))
            {
                requestMessage.Headers.Host = uri.Host;
                
                float startTime = Time.time;
                var response = await httpClient.SendAsync(requestMessage);
                float duration = Time.time - startTime;
                
                response.EnsureSuccessStatusCode();
                byte[] data = await response.Content.ReadAsByteArrayAsync();
                
                AppendResult($"✅ HttpClient 成功! 耗时: {duration:F2}秒");
                AppendResult($"响应大小: {data?.Length ?? 0} bytes");
                AppendResult($"响应码: {(int)response.StatusCode} {response.ReasonPhrase}");
                AppendResult($"Content-Type: {response.Content.Headers.ContentType}");
                
                // 显示一些关键的响应头
                foreach (var header in response.Headers)
                {
                    AppendResult($"响应头: {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ HttpClient 失败: {e.Message}");
        }
    }

    private string ResolveWithHttpDns(string host)
    {
        try
        {
            AppendResult($"🔍 HttpDNS解析: {host}");
            var ips = HttpDnsHelper.getIpsByHostAsync(host);
            if (ips != null && ips.Count > 0)
            {
                AppendResult($"✅ HttpDNS解析 {host} -> {ips[0]} (共找到{ips.Count}个IP)");
                return ips[0];
            }
            else
            {
                AppendResult($"⚠️ HttpDNS未返回IP，使用系统DNS");
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ HttpDNS失败: {e.Message}");
        }

        return ResolveWithSystemDns(host);
    }

    private string ResolveWithSystemDns(string host)
    {
        try
        {
            AppendResult($"系统DNS解析: {host}");
            var addresses = System.Net.Dns.GetHostAddresses(host);
            if (addresses.Length > 0)
            {
                AppendResult($"系统DNS解析 {host} -> {addresses[0]}");
                return addresses[0].ToString();
            }
        }
        catch (Exception e)
        {
            AppendResult($"系统DNS失败: {e.Message}");
        }

        return host;
    }

    private byte[] ReadStreamToBytes(Stream stream)
    {
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}