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

/// <summary>
/// HttpDNS高级功能演示脚本
/// 测试分阶段初始化、增强解析、批量预解析等高级功能
/// </summary>
public class HttpDnsAdvancedDemo : MonoBehaviour
{
    [Header("基础配置")]
    public string accountId = "YOUR_ACCOUNT_ID";
    public string secret = "YOUR_SECRET_KEY";
    public string aesSecret = "";
    
    [Header("测试域名配置")]
    public List<string> testHosts = new List<string>
    {
        "www.aliyun.com",
        "ecs.console.aliyun.com", 
        "cube.elemecdn.com",
        "www.taobao.com"
    };
    
    [Header("UI组件 - 基础功能")]
    public Button initButton;
    public Button buildButton;
    public Button debugToggleButton;
    public Button httpsToggleButton;
    public Button timeoutButton;
    public Button clearCacheButton;
    
    [Header("UI组件 - 高级功能")]
    public Button persistentCacheToggleButton;
    public Button reuseExpiredToggleButton;
    public Button networkChangedToggleButton;
    public Button sessionIdButton;
    public Button batchPreResolveButton;
    public Button cleanAllCacheButton;
    
    [Header("UI组件 - 解析测试")]
    public InputField hostInputField;
    public Button singleResolveButton;
    public Button asyncResolveButton;
    public Button preResolveButton;
    public Button clearButton;
    
    [Header("UI组件 - 网络请求方法")]
    public Button httpClientButton;
    public Button httpWebRequestButton;
    public Button unityWebRequestButton;
    
    [Header("结果显示")]
    public Text resultText;
    public ScrollRect resultScrollRect;
    
    [Header("状态显示")]
    public Text statusText;

    // 状态变量
    private bool debugEnabled = false;
    private bool httpsEnabled = true;
    private bool persistentCacheEnabled = true;
    private bool reuseExpiredEnabled = true;
    private bool networkChangedEnabled = true;
    private bool isInitialized = false;
    private bool isBuilt = false;

    void Start()
    {
        Debug.Log("=== HttpDNS 高级功能演示开始 ===");
        
        // 绑定按钮事件
        BindButtonEvents();
        
        // 初始化UI状态
        InitializeUI();
        
        AppendResult("HttpDNS 高级功能演示脚本已启动");
        AppendResult("点击 '阶段1:初始化' 开始测试流程");
        UpdateStatus("就绪");
    }

    void BindButtonEvents()
    {
        // 基础功能
        if (initButton != null)
            initButton.onClick.AddListener(OnInitialize);
        if (buildButton != null)
            buildButton.onClick.AddListener(OnBuild);
        if (debugToggleButton != null)
            debugToggleButton.onClick.AddListener(OnToggleDebug);
        if (httpsToggleButton != null)
            httpsToggleButton.onClick.AddListener(OnToggleHttps);
        if (timeoutButton != null)
            timeoutButton.onClick.AddListener(OnSetTimeout);
        if (clearCacheButton != null)
            clearCacheButton.onClick.AddListener(OnClearCache);
        
        // 高级功能
        if (persistentCacheToggleButton != null)
            persistentCacheToggleButton.onClick.AddListener(OnTogglePersistentCache);
        if (reuseExpiredToggleButton != null)
            reuseExpiredToggleButton.onClick.AddListener(OnToggleReuseExpired);
        if (networkChangedToggleButton != null)
            networkChangedToggleButton.onClick.AddListener(OnToggleNetworkChanged);
        if (sessionIdButton != null)
            sessionIdButton.onClick.AddListener(OnGetSessionId);
        if (batchPreResolveButton != null)
            batchPreResolveButton.onClick.AddListener(OnBatchPreResolve);
        if (cleanAllCacheButton != null)
            cleanAllCacheButton.onClick.AddListener(OnCleanAllCache);
        
        // 解析测试
        if (singleResolveButton != null)
            singleResolveButton.onClick.AddListener(OnSingleResolve);
        if (asyncResolveButton != null)
            asyncResolveButton.onClick.AddListener(OnAsyncResolve);
        if (preResolveButton != null)
            preResolveButton.onClick.AddListener(OnPreResolve);
        if (clearButton != null)
            clearButton.onClick.AddListener(OnClearResults);
        
        // 网络请求方法
        if (httpClientButton != null)
            httpClientButton.onClick.AddListener(OnTestHttpClient);
        if (httpWebRequestButton != null)
            httpWebRequestButton.onClick.AddListener(OnTestHttpWebRequest);
        if (unityWebRequestButton != null)
            unityWebRequestButton.onClick.AddListener(OnTestUnityWebRequest);
    }

    void InitializeUI()
    {
        if (hostInputField != null)
        {
            hostInputField.text = "www.aliyun.com";
        }
        
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        // 根据当前状态更新按钮可用性
        if (buildButton != null)
            buildButton.interactable = isInitialized && !isBuilt;
        
        // 高级功能按钮需要先完成build
        var advancedButtons = new Button[] 
        {
            persistentCacheToggleButton, reuseExpiredToggleButton, 
            networkChangedToggleButton, sessionIdButton,
            cleanAllCacheButton
        };
        
        foreach (var btn in advancedButtons)
        {
            if (btn != null)
                btn.interactable = isBuilt;
        }
    }

    // ==================== 基础功能测试 ====================
    
    public void OnInitialize()
    {
        try
        {
            AppendResult("=== 阶段1: 初始化HttpDNS服务 ===");
            
            // 分阶段初始化
            HttpDnsHelper.init(accountId, secret, aesSecret);
            isInitialized = true;
            
            AppendResult($"✅ 初始化完成 - AccountId: {accountId}");
            AppendResult("参数已保存，等待调用build()");
            UpdateStatus("已初始化");
            UpdateButtonStates();
        }
        catch (Exception e)
        {
            AppendResult($"❌ 初始化失败: {e.Message}");
            UpdateStatus("初始化失败");
        }
    }

    public void OnBuild()
    {
        try
        {
            AppendResult("=== 阶段2: 构建HttpDNS服务实例 ===");
            
            // 先设置配置项（在build之前）
            HttpDnsHelper.debug(debugEnabled);
            HttpDnsHelper.setHttpsRequestEnabled(httpsEnabled);
            HttpDnsHelper.setTimeout(3000);
            
            // 分阶段构建
            bool success = HttpDnsHelper.build();
            
            if (success)
            {
                isBuilt = true;
                AppendResult("✅ 服务构建成功！");
                AppendResult("所有高级功能现已可用");
                UpdateStatus("已构建");
                
                // 设置高级功能
                ConfigureAdvancedFeatures();
            }
            else
            {
                AppendResult("❌ 服务构建失败");
                UpdateStatus("构建失败");
            }
            
            UpdateButtonStates();
        }
        catch (Exception e)
        {
            AppendResult($"❌ 构建失败: {e.Message}");
            UpdateStatus("构建异常");
        }
    }

    void ConfigureAdvancedFeatures()
    {
        try
        {
            AppendResult("=== 配置高级功能 ===");
            
            // 持久化缓存
            HttpDnsHelper.setPersistentCacheIPEnabled(persistentCacheEnabled, 3600);
            AppendResult($"✅ 持久化缓存: {persistentCacheEnabled} (1小时)");
            
            // 过期IP复用
            HttpDnsHelper.setReuseExpiredIPEnabled(reuseExpiredEnabled);
            AppendResult($"✅ 过期IP复用: {reuseExpiredEnabled}");
            
            // 网络切换预解析
            HttpDnsHelper.setPreResolveAfterNetworkChanged(networkChangedEnabled);
            AppendResult($"✅ 网络切换预解析: {networkChangedEnabled}");
            
            AppendResult("高级功能配置完成！");
        }
        catch (Exception e)
        {
            AppendResult($"❌ 高级功能配置失败: {e.Message}");
        }
    }

    public void OnToggleDebug()
    {
        debugEnabled = !debugEnabled;
        HttpDnsHelper.debug(debugEnabled);
        AppendResult($"🔧 调试日志: {(debugEnabled ? "开启" : "关闭")}");
        UpdateDebugButtonText();
    }

    public void OnToggleHttps()
    {
        httpsEnabled = !httpsEnabled;
        if (isBuilt)
        {
            HttpDnsHelper.setHttpsRequestEnabled(httpsEnabled);
        }
        AppendResult($"🔒 HTTPS请求: {(httpsEnabled ? "启用" : "禁用")}");
        UpdateHttpsButtonText();
    }

    public void OnSetTimeout()
    {
        int timeout = 5000; // 5秒
        HttpDnsHelper.setTimeout(timeout);
        AppendResult($"⏱️ 请求超时设置为: {timeout}ms");
    }

    public void OnClearCache()
    {
        HttpDnsHelper.clearCache();
        AppendResult("🗑️ DNS缓存已清除");
    }

    // ==================== 高级功能测试 ====================

    public void OnTogglePersistentCache()
    {
        if (!isBuilt) return;
        
        persistentCacheEnabled = !persistentCacheEnabled;
        HttpDnsHelper.setPersistentCacheIPEnabled(persistentCacheEnabled, 3600);
        AppendResult($"💾 持久化缓存: {(persistentCacheEnabled ? "启用" : "禁用")} (1小时)");
        UpdatePersistentCacheButtonText();
    }

    public void OnToggleReuseExpired()
    {
        if (!isBuilt) return;
        
        reuseExpiredEnabled = !reuseExpiredEnabled;
        HttpDnsHelper.setReuseExpiredIPEnabled(reuseExpiredEnabled);
        AppendResult($"♻️ 过期IP复用: {(reuseExpiredEnabled ? "启用" : "禁用")}");
        UpdateReuseExpiredButtonText();
    }

    public void OnToggleNetworkChanged()
    {
        if (!isBuilt) return;
        
        networkChangedEnabled = !networkChangedEnabled;
        HttpDnsHelper.setPreResolveAfterNetworkChanged(networkChangedEnabled);
        AppendResult($"📶 网络切换预解析: {(networkChangedEnabled ? "启用" : "禁用")}");
        UpdateNetworkChangedButtonText();
    }

    public void OnGetSessionId()
    {
        if (!isBuilt) return;
        
        try
        {
            string sessionId = HttpDnsHelper.getSessionId();
            if (!string.IsNullOrEmpty(sessionId))
            {
                AppendResult($"🆔 会话ID: {sessionId}");
            }
            else
            {
                AppendResult("⚠️ 无法获取会话ID");
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ 获取会话ID失败: {e.Message}");
        }
    }

    public void OnBatchPreResolve()
    {
        if (!isBuilt) return;
        
        try
        {
            AppendResult("=== 批量预解析测试 ===");
            
            // 批量预解析
            HttpDnsHelper.setPreResolveHosts(testHosts, "auto");
            AppendResult($"🚀 批量预解析启动: {testHosts.Count}个域名");
            
            foreach (var host in testHosts)
            {
                AppendResult($"  - {host}");
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ 批量预解析失败: {e.Message}");
        }
    }

    public void OnCleanAllCache()
    {
        if (!isBuilt) return;
        
        HttpDnsHelper.cleanAllHostCache();
        AppendResult("🧹 所有主机缓存已清理");
    }

    // ==================== 解析测试 ====================

    public void OnSingleResolve()
    {
        if (!isBuilt) return;
        
        var host = hostInputField?.text ?? "www.aliyun.com";
        
        try
        {
            AppendResult($"=== 单域名解析: {host} ===");
            
            var ips = HttpDnsHelper.getIpsByHostAsync(host);
            if (ips != null && ips.Count > 0)
            {
                AppendResult($"✅ 解析成功: {string.Join(", ", ips)}");
            }
            else
            {
                AppendResult("⚠️ 解析返回空结果");
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ 解析失败: {e.Message}");
        }
    }

    public void OnAsyncResolve()
    {
        if (!isBuilt) return;
        
        var host = hostInputField?.text ?? "www.aliyun.com";
        
        try
        {
            AppendResult($"=== 异步解析: {host} ===");
            
            var ips = HttpDnsHelper.getIpsByHost(host);
            if (ips != null && ips.Count > 0)
            {
                AppendResult($"✅ 异步解析成功: {string.Join(", ", ips)}");
            }
            else
            {
                AppendResult("⚠️ 异步解析返回空结果");
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ 异步解析失败: {e.Message}");
        }
    }

    public void OnPreResolve()
    {
        if (!isBuilt) return;
        
        var host = hostInputField?.text ?? "www.aliyun.com";
        
        try
        {
            HttpDnsHelper.setPreResolveHosts(host);
            AppendResult($"🚀 预解析已启动: {host}");
        }
        catch (Exception e)
        {
            AppendResult($"❌ 预解析失败: {e.Message}");
        }
    }

    // ==================== UI辅助方法 ====================

    public void OnClearResults()
    {
        if (resultText != null)
        {
            resultText.text = "结果已清空...\n\n等待新的测试结果";
        }
        Debug.Log("高级UI测试结果已清空");
    }

    void UpdateDebugButtonText()
    {
        if (debugToggleButton != null)
        {
            var text = debugToggleButton.GetComponentInChildren<Text>();
            if (text != null)
                text.text = debugEnabled ? "调试:开" : "调试:关";
        }
    }

    void UpdateHttpsButtonText()
    {
        if (httpsToggleButton != null)
        {
            var text = httpsToggleButton.GetComponentInChildren<Text>();
            if (text != null)
                text.text = httpsEnabled ? "HTTPS:开" : "HTTPS:关";
        }
    }

    void UpdatePersistentCacheButtonText()
    {
        if (persistentCacheToggleButton != null)
        {
            var text = persistentCacheToggleButton.GetComponentInChildren<Text>();
            if (text != null)
                text.text = persistentCacheEnabled ? "持久缓存:开" : "持久缓存:关";
        }
    }

    void UpdateReuseExpiredButtonText()
    {
        if (reuseExpiredToggleButton != null)
        {
            var text = reuseExpiredToggleButton.GetComponentInChildren<Text>();
            if (text != null)
                text.text = reuseExpiredEnabled ? "过期复用:开" : "过期复用:关";
        }
    }

    void UpdateNetworkChangedButtonText()
    {
        if (networkChangedToggleButton != null)
        {
            var text = networkChangedToggleButton.GetComponentInChildren<Text>();
            if (text != null)
                text.text = networkChangedEnabled ? "网络预解析:开" : "网络预解析:关";
        }
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

    private void UpdateStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = $"状态: {status}";
        }
    }
    
    // ==================== 网络请求方法测试 ====================
    
    private string ResolveWithHttpDns(string hostname)
    {
        try
        {
            var ips = HttpDnsHelper.getIpsByHostAsync(hostname);
            if (ips != null && ips.Count > 0)
            {
                return ips[0]; // 返回第一个IP
            }
        }
        catch (Exception e)
        {
            AppendResult($"⚠️ DNS解析失败: {e.Message}");
        }
        return hostname; // 如果解析失败，返回原域名
    }
    
    public async void OnTestHttpClient()
    {
        if (!isBuilt) return;
        
        var url = $"https://{hostInputField?.text ?? "www.aliyun.com"}";
        
        try
        {
            AppendResult($"=== HttpClient请求测试 ===");
            AppendResult($"🔗 目标URL: {url}");
            
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var uri = new System.Uri(url);
                string ip = ResolveWithHttpDns(uri.Host);
                string newUrl = url.Replace(uri.Host, ip);
                
                AppendResult($"🔄 替换后URL: {newUrl}");
                
                using (var requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, newUrl))
                {
                    // 关键步骤：设置Host头
                    requestMessage.Headers.Host = uri.Host;
                    AppendResult($"🏷️ 设置Host头: {uri.Host}");
                    
                    var response = await httpClient.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    
                    string result = await response.Content.ReadAsStringAsync();
                    AppendResult($"✅ HttpClient请求成功！");
                    AppendResult($"📊 响应状态: {response.StatusCode}");
                    AppendResult($"📄 响应大小: {result.Length} 字符");
                }
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ HttpClient请求失败: {e.Message}");
        }
    }
    
    public void OnTestHttpWebRequest()
    {
        if (!isBuilt) return;
        
        var url = $"https://{hostInputField?.text ?? "www.aliyun.com"}";
        
        try
        {
            AppendResult($"=== HttpWebRequest请求测试 ===");
            AppendResult($"🔗 目标URL: {url}");
            
            var uri = new System.Uri(url);
            string ip = ResolveWithHttpDns(uri.Host);
            string newUrl = url.Replace(uri.Host, ip);
            
            AppendResult($"🔄 替换后URL: {newUrl}");
            
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(newUrl);
            request.Method = "GET";
            // 关键步骤：设置Host头
            request.Host = uri.Host;
            AppendResult($"🏷️ 设置Host头: {uri.Host}");
            
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            using (System.IO.Stream stream = response.GetResponseStream())
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                AppendResult($"✅ HttpWebRequest请求成功！");
                AppendResult($"📊 响应状态: {response.StatusCode}");
                AppendResult($"📄 响应大小: {result.Length} 字符");
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ HttpWebRequest请求失败: {e.Message}");
        }
    }
    
    public async void OnTestUnityWebRequest()
    {
        if (!isBuilt) return;
        
        var url = $"https://{hostInputField?.text ?? "www.aliyun.com"}";
        
        try
        {
            AppendResult($"=== UnityWebRequest请求测试 ===");
            AppendResult($"⚠️ 注意：不推荐使用，可能无法正确配置SNI");
            AppendResult($"🔗 目标URL: {url}");
            
            var uri = new System.Uri(url);
            string ip = ResolveWithHttpDns(uri.Host);
            string newUrl = url.Replace(uri.Host, ip);
            
            AppendResult($"🔄 替换后URL: {newUrl}");
            
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(newUrl))
            {
                // 关键步骤：设置Host头
                request.SetRequestHeader("Host", uri.Host);
                AppendResult($"🏷️ 设置Host头: {uri.Host}");
                
                var operation = request.SendWebRequest();
                
                // 等待请求完成
                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
                
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    string response = request.downloadHandler.text;
                    AppendResult($"✅ UnityWebRequest请求成功！");
                    AppendResult($"📊 响应状态: {request.responseCode}");
                    AppendResult($"📄 响应大小: {response.Length} 字符");
                }
                else
                {
                    AppendResult($"❌ UnityWebRequest请求失败: {request.error}");
                }
            }
        }
        catch (Exception e)
        {
            AppendResult($"❌ UnityWebRequest请求异常: {e.Message}");
        }
    }
}