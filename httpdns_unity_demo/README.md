# HTTPDNS Unity DEMO

本文介绍如何在Unity应用开发中集成使用HTTPDNS。

Unity是跨平台游戏引擎，通过一套代码库，就能构建精美的、原生平台编译的多平台应用和游戏。

本示例提供了一个完整的Unity HTTPDNS解决方案，展示如何在Unity应用中集成使用HTTPDNS，支持Android、iOS、MacOS和Windows等平台。

以下是插件的使用说明和最佳实践。

## 一、快速入门

### 1.1 开通服务

请参考[快速入门文档](https://help.aliyun.com/document_detail/2867674.html)开通HTTPDNS。

### 1.2 获取配置

请参考开发配置文档在EMAS控制台开发配置中获取AccountId/SecretKey等信息，用于初始化SDK。

### 1.3 集成插件到项目

本项目以源码形式提供，需要将插件复制到您的Unity项目中进行集成。

#### 1.3.1 导入Unity Package

1. 下载本项目的 `httpdns_unity_demo` 目录
2. 将整个 `Assets/Plugins` 目录复制到您的Unity项目中
3. 将 `Assets/Editor` 目录（可选，用于创建测试UI）复制到您的项目中

项目结构应如下所示：
```
YourUnityProject/
├── Assets/
│   ├── Plugins/
│   │   ├── HttpDnsHelper.cs           # 主接口文件
│   │   ├── AndroidHttpDnsHelper.cs     # Android平台实现
│   │   ├── iOSHttpDnsHelper.cs        # iOS平台实现  
│   │   ├── CHttpDnsHelper.cs          # C SDK平台实现
│   │   ├── Android/                   # Android JAR库
│   │   ├── iOS/                       # iOS Bridge文件
│   │   └── C/                         # C SDK
│   └── Editor/                        # 测试UI创建器
├── ProjectSettings/
└── ...
```

#### 1.3.2 验证原生SDK版本

插件已集成了对应平台的HTTPDNS原生SDK，当前版本：

- **Android**: `com.aliyun.ams:alicloud-android-httpdns:2.6.5`
- **iOS**: `AlicloudHTTPDNS:3.2.1`
- **C**: `alibabacloud-httpdns-c-sdk:2.2.5`

## 二、安装配置

### 2.1 添加Unity依赖

在您的Unity项目中进行以下配置：

```yaml
# Unity项目配置
Player Settings:
  - Android: 
      Minimum API Level: 19+
      Internet Permission: Enabled
  - iOS:
      Deployment Target: 12.0+
      Internet Capability: Enabled
  - Standalone:
      Platforms: Windows/macOS
      Architectures: x64/arm64
      Scripting Backend: Mono 或 IL2CPP
```

插件使用本地源码依赖方式引用。添加到项目之后需要根据目标平台进行相应配置。

### 2.2 原生SDK版本更新

如需更新HTTPDNS原生SDK版本，请按以下步骤操作：

#### 2.2.1 更新Android SDK版本

Android 端通过 Maven 依赖管理。请在 `Assets/Plugins/Android/mainTemplate.gradle` 的 `dependencies` 中修改版本号：

```gradle
dependencies {
    // 其他依赖...
    implementation 'com.aliyun.ams:alicloud-android-httpdns:2.6.5' // 修改为所需版本
}
```

提示：Android、iOS、C 三端接口能力存在差异，请以各平台官方接口文档为准。

启用了自定义 Gradle 模板（Custom Main/Base Gradle Template、Custom Proguard）后，Unity 构建会自动从仓库拉取对应版本。

可用版本请参考：[阿里云HTTPDNS Android SDK](https://help.aliyun.com/document_detail/435251.html)。

#### 2.2.2 更新iOS SDK版本

编辑构建后处理器中的CocoaPods依赖版本：

```csharp
// 在 HttpDnsIOSPostProcessor.cs 中更新版本
podfileContent += "  pod 'AlicloudHTTPDNS', '~> 3.2.1'\n";

// 更新为您需要的版本
podfileContent += "  pod 'AlicloudHTTPDNS', '~> 新版本号'\n";
```

可用版本请参考：[阿里云HTTPDNS iOS SDK](https://help.aliyun.com/document_detail/2868036.html)

#### 2.2.3 更新C SDK版本

请参考以下步骤操作：

- 获取更新后的 C SDK 源码。
- 在本机编译生成原生库文件。
- 将生成的库文件放入 Unity 工程的插件目录，例如 `Assets/Plugins/C/x86_64/` 下的 `.dll` 或 macOS 的 `.dylib`。
- 回到 Unity，重新构建项目以生效。

更多信息与版本说明参考：[阿里云HTTPDNS C SDK 文档](https://help.aliyun.com/document_detail/2867645.html)

#### 2.2.4 重新构建项目

更新版本后，需要重新构建项目：

**Android:**
```bash
# Unity Editor中
File -> Build Settings -> Android -> Build
```

**iOS:**
```bash
# Unity Editor中构建后
cd [iOS构建目录]
pod install --repo-update
open Unity-iPhone.xcworkspace
```

**Standalone（Windows/macOS）:**
```bash
# Unity Editor中
File -> Build Settings -> PC, Mac & Linux Standalone -> Build
```

## 三、配置和使用

### 3.1 初始化配置

应用启动后，需要先初始化插件，才能调用HTTPDNS能力。
初始化主要是配置AccountId/SecretKey等信息及功能开关。
示例代码如下：

```csharp
using UnityEngine;

public class HttpDnsManager : MonoBehaviour 
{
    void Start()
    {
        // 初始化HTTPDNS - 分阶段初始化模式
        HttpDnsHelper.init("YOUR_ACCOUNT_ID", "YOUR_SECRET_KEY");
        
        // 设置功能选项
        HttpDnsHelper.setHttpsRequestEnabled(true);
        HttpDnsHelper.debug(true);
        HttpDnsHelper.setTimeout(3000);
        
        // 高级功能配置
        HttpDnsHelper.setPersistentCacheIPEnabled(true, 3600); // 启用持久化缓存1小时
        HttpDnsHelper.setReuseExpiredIPEnabled(true);          // 允许复用过期IP
        HttpDnsHelper.setPreResolveAfterNetworkChanged(true);  // 网络切换时自动预解析
        
        // 构建服务实例
        bool success = HttpDnsHelper.build();
        if (success) 
        {
            Debug.Log("HTTPDNS初始化成功");
            
            // 预解析常用域名
            var hosts = new List<string> { "www.aliyun.com", "ecs.console.aliyun.com" };
            HttpDnsHelper.setPreResolveHosts(hosts, "auto");
        }
        else 
        {
            Debug.LogError("HTTPDNS初始化失败");
        }
    }
}
```

注意：`getIpsByHostAsync` 为非阻塞获取，可能返回空或无结果。建议调用方在无解析结果时回退到系统 DNS 或直接使用原始域名发起请求。

#### 3.1.1 日志配置

应用开发过程中，如果要输出HTTPDNS的日志，可以调用日志输出控制方法，开启日志，示例代码如下：

```csharp
HttpDnsHelper.debug(true);
Debug.Log("日志输出已启用");
```

#### 3.1.2 sessionId记录

应用在运行过程中，可以调用获取SessionId方法获取sessionId，记录到应用的数据采集系统中。
sessionId用于表示标识一次应用运行，线上排查时，可以用于查询应用一次运行过程中的解析日志，示例代码如下：

```csharp
string sessionId = HttpDnsHelper.getSessionId();
Debug.Log($"SessionId = {sessionId}");
```

### 3.2 域名解析

#### 3.2.1 预解析

当需要提前解析域名时，可以调用预解析域名方法，示例代码如下：

```csharp
var hosts = new List<string> { "www.aliyun.com", "www.example.com" };
HttpDnsHelper.setPreResolveHosts(hosts, "both");
Debug.Log("预解析设置成功");
```

调用之后，插件会发起域名解析，并把结果缓存到内存，用于后续请求时直接使用。

#### 3.2.2 域名解析

当需要解析域名时，可以通过调用域名解析方法解析域名获取IP，示例代码如下：

```csharp
public void ResolveDomain(string hostname)
{
    // 异步解析
    var ips = HttpDnsHelper.getIpsByHostAsync(hostname);
    if (ips != null && ips.Count > 0)
    {
        Debug.Log($"解析成功: {hostname} -> {ips[0]}");
        foreach (var ip in ips)
        {
            Debug.Log($"可用IP: {ip}");
        }
    }
    
    // 同步非阻塞解析
    var result = HttpDnsHelper.resolveHostSyncNonBlocking(hostname, "auto");
    if (result != null)
    {
        Debug.Log($"IPv4地址: {result.IPv4.Count}个");
        Debug.Log($"IPv6地址: {result.IPv6.Count}个");
        Debug.Log($"TTL: {result.TTL}秒");
    }
}
```

## 四、Unity最佳实践

### 4.1 原理说明

本示例展示了Unity中集成HTTPDNS的完整解决方案：

1. **跨平台统一接口**: 使用 `HttpDnsHelper` 提供统一的API接口
2. **平台特定实现**: 针对Android、iOS、C SDK提供不同的底层实现
3. **自动DNS替换**: 在网络请求前自动将域名替换为解析的IP地址
4. **Host头设置**: 确保HTTPS/SSL连接的SNI正确性

### 4.2 网络请求集成

#### 4.2.1 HttpClient集成示例

```csharp
using System;
using System.Net.Http;
using UnityEngine;

public class HttpDnsHttpClient : MonoBehaviour
{
    private static readonly HttpClient httpClient = new HttpClient();
    
    public async void MakeRequest(string url)
    {
        try
        {
            var uri = new Uri(url);
            string hostname = uri.Host;
            
            // 使用HTTPDNS解析域名
            var ips = HttpDnsHelper.getIpsByHostAsync(hostname);
            if (ips != null && ips.Count > 0)
            {
                string resolvedIP = ips[0];
                string newUrl = url.Replace(hostname, resolvedIP);
                
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, newUrl))
                {
                    // 关键：设置Host头保证SSL/SNI正确性
                    requestMessage.Headers.Host = hostname;
                    
                    var response = await httpClient.SendAsync(requestMessage);
                    string content = await response.Content.ReadAsStringAsync();
                    
                    Debug.Log($"请求成功: {response.StatusCode}");
                    Debug.Log($"内容长度: {content.Length}");
                }
            }
            else
            {
                Debug.LogWarning("HTTPDNS解析失败，使用原始URL");
                var response = await httpClient.GetAsync(url);
                // 处理响应...
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"请求失败: {e.Message}");
        }
    }
}
```

#### 4.2.2 UnityWebRequest集成示例

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpDnsUnityWebRequest : MonoBehaviour
{
    public IEnumerator MakeRequest(string url)
    {
        var uri = new Uri(url);
        string hostname = uri.Host;
        
        // 使用HTTPDNS解析域名
        var ips = HttpDnsHelper.getIpsByHostAsync(hostname);
        if (ips != null && ips.Count > 0)
        {
            string resolvedIP = ips[0];
            string newUrl = url.Replace(hostname, resolvedIP);
            
            using (UnityWebRequest request = UnityWebRequest.Get(newUrl))
            {
                // 设置Host头
                request.SetRequestHeader("Host", hostname);
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"请求成功: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.LogError($"请求失败: {request.error}");
                }
            }
        }
    }
}
```

### 4.3 测试UI使用

项目提供了两个测试UI创建器：

1. **基础测试UI**: 通过菜单 `HttpDNS/Create DNS Test UI` 创建
2. **高级测试UI**: 通过菜单 `HttpDNS/Create Advanced Test UI` 创建

测试UI包含完整的功能演示，帮助您快速验证集成效果。

## 五、API参考

### 5.1 初始化方法

#### init
初始化HTTPDNS服务。

```csharp
// 基础初始化
HttpDnsHelper.init(string accountId, string secretKey);

// 完整初始化
HttpDnsHelper.init(string accountId, string secretKey, string aesSecretKey);
```

参数说明：
- `accountId`: HTTPDNS账户ID（必选）
- `secretKey`: 访问密钥（可选，用于鉴权）
- `aesSecretKey`: AES加密密钥（可选，用于加密）

#### build
构建HTTPDNS服务实例，在设置完所有配置后调用。

```csharp
bool success = HttpDnsHelper.build();
```

返回值：
- `true`: 构建成功
- `false`: 构建失败

### 5.2 配置方法

#### debug
控制是否打印调试日志。

```csharp
HttpDnsHelper.debug(bool enable);
```

#### setHttpsRequestEnabled
设置是否使用HTTPS进行DNS查询。

```csharp
HttpDnsHelper.setHttpsRequestEnabled(bool enable);
```

#### setTimeout
设置DNS查询超时时间。

```csharp
HttpDnsHelper.setTimeout(int timeoutMs);
```

#### setPersistentCacheIPEnabled
设置是否启用持久化缓存。

```csharp
HttpDnsHelper.setPersistentCacheIPEnabled(bool enable, int cacheTTL);
```

参数：
- `enable`: 是否启用
- `cacheTTL`: 缓存TTL时间（秒）

#### setReuseExpiredIPEnabled
设置是否允许复用过期IP。

```csharp
HttpDnsHelper.setReuseExpiredIPEnabled(bool enable);
```

#### setPreResolveAfterNetworkChanged
设置网络切换时是否自动刷新预解析。

```csharp
HttpDnsHelper.setPreResolveAfterNetworkChanged(bool enable);
```

### 5.3 解析方法

#### getIpsByHostAsync
异步解析域名。

```csharp
List<string> ips = HttpDnsHelper.getIpsByHostAsync(string hostname);
```

返回：IP地址列表，如果解析失败返回null或空列表。

#### getIpsByHost
同步解析域名。

```csharp
List<string> ips = HttpDnsHelper.getIpsByHost(string hostname);
```

#### resolveHostSyncNonBlocking
同步非阻塞解析，返回详细信息。

```csharp
HttpDnsResult result = HttpDnsHelper.resolveHostSyncNonBlocking(string hostname, string ipType);
```

参数：
- `hostname`: 要解析的域名
- `ipType`: IP类型 ("auto", "ipv4", "ipv6", "both")

返回对象包含：
- `IPv4`: IPv4地址列表
- `IPv6`: IPv6地址列表  
- `TTL`: 缓存存活时间
- `Extra`: 额外信息

### 5.4 预解析方法

#### setPreResolveHosts
设置预解析域名列表。

```csharp
// 单个域名
HttpDnsHelper.setPreResolveHosts(string hostname);

// 多个域名
HttpDnsHelper.setPreResolveHosts(List<string> hostnames, string ipType);
```

### 5.5 缓存管理

#### clearCache
清除指定域名的缓存。

```csharp
HttpDnsHelper.clearCache();
```

#### cleanAllHostCache
清除所有域名缓存。

```csharp
HttpDnsHelper.cleanAllHostCache();
```

### 5.6 辅助方法

#### getSessionId
获取当前会话ID。

```csharp
string sessionId = HttpDnsHelper.getSessionId();
```

## 六、常见问题

### 6.1 Android构建问题

**问题**: Gradle 无法解析 `com.aliyun.ams:alicloud-android-httpdns` 依赖或构建失败。
**解决**: 
1. 启用自定义 Gradle 模板（Player Settings -> Publishing Settings -> Custom Main/Base Gradle Template）。
2. 检查 `Assets/Plugins/Android/mainTemplate.gradle`：
   - `repositories` 是否配置了阿里云远程仓库镜像。
   - `dependencies` 中存在 `implementation 'com.aliyun.ams:alicloud-android-httpdns:版本号'`。

### 6.2 iOS构建问题

**问题**: iOS构建后找不到CocoaPods依赖。
**解决**: 确保按照构建说明执行 `pod install` 命令，并使用 `.xcworkspace` 文件打开项目。

### 6.3 C SDK平台问题

**问题**: C SDK平台缺少原生库依赖。
**解决**: 
1. **Windows**: 确保安装了VCPKG并设置了环境变量
2. **macOS**: 确保安装了Homebrew依赖库
3. 检查构建后处理器是否正确复制了库文件
4. 手动复制依赖库到构建目录

### 6.4 网络请求问题

**问题**: HTTPS请求失败，提示证书错误。
**解决**: 确保设置了正确的Host头，使用原始域名而不是IP地址作为SNI。
