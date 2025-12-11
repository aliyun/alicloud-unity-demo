# 阿里云移动推送 Unity SDK

移动推送（Mobile Push）是提供给移动开发者的移动端消息推送服务，通过在App中集成推送功能，进行高效、精准、实时的消息推送，从而使业务及时触达用户，提高用户粘性。本SDK为Unity开发者提供了跨平台的推送能力，支持iOS和Android双端。

## 版本信息

当前版本：1.0.0-beta

## 环境要求

- Unity 6000.2.10f1
- iOS 12.0+
- Android 5.0+ (API Level 21+)

## 快速开始

### 1. 前置准备

#### 1.1 创建应用

1. 登录[阿里云移动推送控制台](https://emas.console.aliyun.com/)。
2. 创建项目和应用并获取appKey与appSecret。具体操作请参见[创建项目和应用](https://help.aliyun.com/document_detail/436513.html)。
   
#### 1.2 配置应用

1. Android应用参考[配置厂商通道密钥](https://help.aliyun.com/document_detail/434643.html)在移动推送控制台配置厂商密钥信息。
2. iOS应用参考[APNs鉴权配置](https://help.aliyun.com/document_detail/434644.html)在移动推送控制台配置P8或P12证书。

### 2. 集成SDK

#### 2.1 导入插件

**方式一：使用unitypackage**

1. **下载SDK**
   - 从[下载页面](https://ios-repo.oss-cn-shanghai.aliyuncs.com/unity/alicloud-push/1.0.0-beta/alicloud-push.unitypackage)下载`alicloud-push.unitypackage`

2. **导入到Unity项目**
   - 双击下载的`alicloud-push.unitypackage`，或在Unity中选择`Assets -> Import Package -> Custom Package...`，然后选择下载的文件
   - 在弹出的对话框中点击"Import"

1. **验证安装**
   - 导入成功后，在Project窗口中应该能看到`Assets/Plugins/`目录
   - 包含PushHelper.cs等文件

**方式二：手动集成**

如果您不使用unitypackage，也可以手动将开源仓库中`Assets/Plugins`目录复制到您的Unity项目中。

#### 2.2 Unity Android构建设置

为了确保Android平台的推送功能正常工作，需要在Unity中启用自定义Android配置：

1. **启用自定义Android配置**
   - 打开`File -> Build Profiles -> Android -> Player Settings`
   - 在`Publishing Settings`部分，勾选以下五个选项：
     - `Custom Main Manifest`：启用自定义主清单文件
     - `Custom Main Gradle Template`：启用自定义主Gradle模板
     - `Custom Base Gradle Template`：启用自定义Base Gradle模板
     - `Custom Gradle Settings Template`：启用自定义Settings Gradle 模板
     - `Custom Proguard File`：启用自定义Proguard文件

2. **文件合并注意事项**
   - 如果您的项目已经有自定义的Android XML或Gradle文件，请注意将现有配置与`Assets/Plugins/Android/`目录下的插件文件进行合并
   - 特别注意`AndroidManifest.xml`中的权限声明和组件配置不要冲突
   - Gradle文件中的依赖项需要合并，避免版本冲突

#### 2.3 iOS CocoaPods依赖管理

iOS平台使用CocoaPods管理依赖，需要注意以下配置：

1. **自动Podfile生成**
   - 当前Pod文件的生成依赖于脚本`Assets/Plugins/Editor/iOSAliyunPushPostProcessor.cs`
   - 该脚本会在构建iOS项目时自动生成Podfile文件

2. **Pod依赖合并**
   - 如果您的项目依赖其他的Pod库，需要将现有的Pod配置与`iOSAliyunPushPostProcessor.cs`脚本生成的配置进行合并
   - 建议在`iOSAliyunPushPostProcessor.cs`脚本中添加您的其他Pod依赖，或者修改脚本以兼容现有的Podfile配置
   - 确保Pod版本兼容性，避免依赖冲突

#### 2.4 配置应用包名

确保您的应用包名（Android）或Bundle ID（iOS）与推送控制台中创建的应用一致。

### 3. Android配置

#### 3.1 配置AndroidManifest.xml

在`Assets/Plugins/Android/AndroidManifest.xml`文件中配置AppKey、AppSecret以及厂商通道密钥

```xml
<application>
    <!-- 请填写阿里云的- appKey appSecret -->
    <meta-data android:name="com.alibaba.app.appkey" android:value="您的AppKey" />
    <meta-data android:name="com.alibaba.app.appsecret" android:value="您的AppSecret" />

    <!-- 华为通道的参数appid -->
    <meta-data android:name="com.huawei.hms.client.appid" android:value="appid=您的华为AppId" />

    <!-- 荣耀通道的参数-->
    <meta-data android:name="com.hihonor.push.app_id" android:value="您的荣耀AppId" />

    <!-- 小米通道的参数 -->
    <meta-data android:name="com.aliyun.ams.push.xiaomi.id" android:value="您的小米AppId" />
    <meta-data android:name="com.aliyun.ams.push.xiaomi.key" android:value="您的小米AppKey" />

    <!-- oppo通道的参数 -->
    <meta-data android:name="com.aliyun.ams.push.oppo.key" android:value="您的OPPO AppKey" />
    <meta-data android:name="com.aliyun.ams.push.oppo.secret" android:value="您的OPPO AppSecret" />

    <!-- vivo通道的参数api_key为appkey -->
    <meta-data android:name="com.vivo.push.api_key" android:value="您的vivo AppKey" />
    <meta-data android:name="com.vivo.push.app_id" android:value="您的vivo AppId" />

    <!-- 魅族通道的参数 -->
    <meta-data android:name="com.aliyun.ams.push.meizu.id" android:value="您的魅族AppId" />
    <meta-data android:name="com.aliyun.ams.push.meizu.secret" android:value="您的魅族AppKey" />

    <!-- fcm通道的参数 -->
    <meta-data android:name="com.aliyun.ams.push.gcm.sendid" android:value="您的fcm project_number" />
    <meta-data android:name="com.aliyun.ams.push.gcm.applicationid" android:value="您的fcm mobilesdk_app_id" />
    <meta-data android:name="com.aliyun.ams.push.gcm.projectid" android:value="您的fcm project_id"/>
    <meta-data android:name="com.aliyun.ams.push.gcm.apiKey" android:value="您的fcm current_key"/>
</application>
```

#### 3.2 创建通知渠道

Android 8.0及以上版本需要创建通知渠道：

```csharp
PushHelper.CreateNotificationChannel(
    id: "default_channel",
    name: "默认通知",
    importance: PushHelper.IMPORTANCE_DEFAULT,
    desc: "应用默认通知渠道",
    showBadge: true,
    enableLights: true,
    lightColor: unchecked((int)0xFF0000FF),  // 蓝色
    enableVibration: true,
    vibrationPattern: "0,250,250,250"
);
```

服务端推送时需要设置AndroidNotificationChannel参数（PushV2中为ChannelId参数）与这里客户端创建的channel的id相同。

#### 3.3 配置Android消息接收回调（可选）

如果您希望设置Android消息接收回调，那么需要在调用InitPush之前设置，例如：

```csharp
#if UNITY_ANDROID
PushHelper.SetAndroidMessageReceiver(
    onMessage: (title, body) => {
        Debug.Log("onMessage - title: " + title + ", body: " + body);
    },
    onNotification: (title, body, extraMap) => {
        string extras = extraMap != null ? string.Join(", ", extraMap) : "";
        Debug.Log("onNotification - title: " + title + ", body: " + body + ", extraMap: " + extras);
    },
    onNotificationOpened: (title, body, extraMapString) => {
        Debug.Log("onNotificationOpened - title: " + title + ", body: " + body + ", extraMapString: " + extraMapString);
    },
    onNotificationRemoved: (messageId) => {
        Debug.Log("onNotificationRemoved - messageId: " + messageId);
    },
    onSysNoticeOpened: (title, body, extraMap) => {
        string extras = extraMap != null ? string.Join(", ", extraMap) : "";
        Debug.Log("onSysNoticeOpened - title: " + title + ", body: " + body + ", extraMap: " + extras);
    }
);
#endif
```

### 4. iOS配置

iOS的AppKey和AppSecret通过代码配置，无需修改配置文件。

#### 4.1 前台通知处理模式设置（可选）

iOS应用可以设置前台通知处理模式，选择是否弹出通知以及是否调用回调

```csharp
#if UNITY_IOS
// 展示通知且调用回调
PushHelper.SetIOSForegroundNotificationMode(PushHelper.IOS_FOREGROUND_SHOW_AND_CALLBACK);
#endif
```

#### 4.2 配置iOS消息接收回调（可选）

如果您希望设置Android消息接收回调，那么需要在调用InitPush之前设置，例如：

```csharp
#if UNITY_IOS
PushHelper.SetIOSMessageReceiver(
    onMessageReceived: (title, body) => {
        Debug.Log("onMessageReceived - title: " + title + ", body: " + body);
    },
    onAPNSRegisterSuccess: (deviceToken) => {
        Debug.Log("onAPNSRegisterSuccess - deviceToken: " + deviceToken);
    },
    onAPNSRegisterFailed: (error) => {
        Debug.Log("onAPNSRegisterFailed - error: " + error);
    },
    onForegroundNotificationReceived: (title, body, subtitle, userInfo) => {
        Debug.Log("onForegroundNotificationReceived - title: " + title + ", body: " + body + ", subtitle: " + subtitle + ", userInfo: " + userInfo);
    },
    onNotificationOpened: (title, body, subtitle, userInfo) => {
        Debug.Log("onNotificationOpened - title: " + title + ", body: " + body + ", subtitle: " + subtitle + ", userInfo: " + userInfo);
    },
    onNotificationRemoved: (userInfo) => {
        Debug.Log("onNotificationRemoved - userInfo: " + userInfo);
    },
    onRemoteNotificationReceived: (title, body, subtitle, userInfo) => {
        Debug.Log("onRemoteNotificationReceived - title: " + title + ", body: " + body + ", subtitle: " + subtitle + ", userInfo: " + userInfo);
    }
);
#endif
```

### 5. 推送注册

在完成回调设置等预配置后，您需要调用InitPush方法进行推送注册。此外，Android厂商通道（华为、小米等）需要调用InitAndroidThirdPush方法进行注册：

```csharp
// 推送注册
PushHelper.InitPush("您的AppKey", "您的AppSecret", (success, message) => {
    if (success) {
        Debug.Log("推送初始化成功: " + message);
        // 初始化成功后可以获取设备ID
        // string deviceId = PushHelper.GetDeviceId();
        // Debug.Log("设备ID: " + deviceId);
    } else {
        Debug.LogError("推送初始化失败: " + message);
    }
});
#if UNITY_ANDROID
// 安卓厂商通道注册
PushHelper.InitAndroidThirdPush();
#endif
```

### 6. iOS xcode项目配置

在build出iOS xcode项目后，需要安装pod依赖并配置配置APNs推送能力

#### 6.1 安装pod依赖

`Assets/Plugins/Editor/iOSAliyunPushPostProcessor.cs`脚本会自动生成Podfile文件，您需要在build出iOS xcode项目后，进入项目根目录执行以下命令：

```ruby
pod repo update AliyunRepo
pod install

# 如果您尚未添加阿里云Cocoapods仓库，请先执行以下命令添加仓库
# pod repo add AliyunRepo https://github.com/aliyun/aliyun-specs.git
```

#### 6.2 配置APNs推送能力

在打开xcode工程后，您需要按照以下方式配置工程：

1. Target → Signing & Capabilities → 添加 Push Notifications 和 Background Modes 两个Capability
2. Background Modes → 勾选Remote notifications

详情请参考[Xcode工程配置](https://help.aliyun.com/document_detail/434691.html#239aafaa92bsr)

## API使用指南

本节详细介绍每个API的使用方法、参数说明和注意事项。

### 1. 预配置

#### 1.1 SetLogLevel - 设置日志级别

设置SDK的日志输出级别，用于调试和问题排查。

**方法签名：**
```csharp
public static void SetLogLevel(string logLevel)
```

**参数说明：**
- `logLevel`：日志级别，可选值：
  - `PushHelper.LOG_OFF`：关闭日志
  - `PushHelper.LOG_ERROR`：仅错误日志
  - `PushHelper.LOG_WARN`：警告及以上
  - `PushHelper.LOG_INFO`：信息及以上
  - `PushHelper.LOG_DEBUG`：所有日志（调试用）

**平台支持：** iOS & Android

**使用示例：**
```csharp
// 开发环境使用DEBUG级别
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    PushHelper.SetLogLevel(PushHelper.LOG_DEBUG);
#else
    // 生产环境关闭日志或仅显示错误
    PushHelper.SetLogLevel(PushHelper.LOG_ERROR);
#endif
```

**注意事项：**
- 生产环境应关闭或降低日志级别，避免性能影响和信息泄露

#### 1.2 SetAndroidMessageReceiver - 设置Android消息接收回调

统一设置Android平台的所有消息和通知回调。**必须在InitPush之前调用。**

**方法签名：**
```csharp
public static void SetAndroidMessageReceiver(
    Action<string, string> onMessage = null,
    Action<string, string, Dictionary<string, string>> onNotification = null,
    Action<string, string, string> onNotificationOpened = null,
    Action<string, string, string> onNotificationClickedWithNoAction = null,
    Action<string> onNotificationRemoved = null,
    Action<string, string, Dictionary<string, string>, int, string, string> onNotificationReceivedInApp = null,
    Action<string, string, Dictionary<string, string>> onSysNoticeOpened = null
)
```

**参数说明：**

- `onMessage`：透传消息到达回调
  - **触发时机**：当收到阿里云自有通道的透传消息时
  - **参数1 (string title)**：消息标题
  - **参数2 (string body)**：消息内容
  - **说明**：透传消息不会在通知栏显示，需要开发者自行处理UI展示

- `onNotification`：通知到达回调
  - **触发时机**：当收到阿里云自有通道的通知消息时
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (Dictionary<string, string> extraMap)**：通知扩展字段字典
  - **说明**：此回调用于记录或处理通知到达事件

- `onNotificationOpened`：通知点击回调
  - **触发时机**：用户点击阿里云自有通道或fcm通道的通知时
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (string extraMapString)**：通知扩展字段JSON字符串
  - **说明**：用于处理用户点击通知后的跳转逻辑，如打开特定页面

- `onNotificationClickedWithNoAction`：通知点击无动作回调
  - **触发时机**：用户点击阿里云自有通道的通知时触发。需要推送时设置AndroidOpenType参数（PushV2为Accs下的OpenType参数）为`NONE`
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (string extraMapString)**：通知扩展字段JSON字符串
  - **说明**：当推送配置为点击无动作时触发，开发者可自定义处理逻辑

- `onNotificationRemoved`：通知移除回调
  - **触发时机**：用户手动清除阿里云自有通道的通知时
  - **参数1 (string messageId)**：消息唯一标识ID
  - **说明**：用于统计通知的清除情况

- `onNotificationReceivedInApp`：应用内通知接收回调
  - **触发时机**：应用收到阿里云自有通道的通知，且主动在`Assets/Plugins/Android/alicloud-push/PushMessageReceiver.java`中覆写了showNotificationNow方法返回false，此时不会展示通知
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (Dictionary<string, string> extraMap)**：通知扩展字段字典
  - **参数4 (int openType)**：打开类型（1=Activity, 2=URL, 3=无动作）
  - **参数5 (string openActivity)**：要打开的Activity名称
  - **参数6 (string openUrl)**：要打开的URL地址
  - **说明**：用于自定义应用内通知展示，需要配合拦截通知使用

- `onSysNoticeOpened`：厂商通道（fcm除外）的通知点击回调
  - **触发时机**：用户点击厂商通道（小米、华为、OPPO等）推送的通知时触发。需要推送时设置AndroidPopupActivity参数（PushV2为VendorChannelActivity参数）为`com.alibaba.sdk.android.pushwrapper.ThirdPushActivity`
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (Dictionary<string, string> extraMap)**：通知扩展字段字典
  - **说明**：处理厂商通道通知的点击事件

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
PushHelper.SetAndroidMessageReceiver(
    onMessage: (title, body) => {
        Debug.Log("onMessage - title: " + title + ", body: " + body);
    },
    onNotification: (title, body, extraMap) => {
        string extras = extraMap != null ? string.Join(", ", extraMap) : "";
        Debug.Log("onNotification - title: " + title + ", body: " + body + ", extraMap: " + extras);
    },
    onNotificationOpened: (title, body, extraMapString) => {
        Debug.Log("onNotificationOpened - title: " + title + ", body: " + body + ", extraMapString: " + extraMapString);
    },
    onNotificationClickedWithNoAction: (title, body, extraMapString) => {
        Debug.Log("onNotificationClickedWithNoAction - title: " + title + ", body: " + body);
    },
    onNotificationRemoved: (messageId) => {
        Debug.Log("onNotificationRemoved - messageId: " + messageId);
    },
    onNotificationReceivedInApp: (title, body, extraMap, openType, openActivity, openUrl) => {
        Debug.Log("onNotificationReceivedInApp - title: " + title + ", openType: " + openType);
    },
    onSysNoticeOpened: (title, body, extraMap) => {
        string extras = extraMap != null ? string.Join(", ", extraMap) : "";
        Debug.Log("onSysNoticeOpened - title: " + title + ", body: " + body + ", extraMap: " + extras);
    }
);
#endif
```

**注意事项：**
- 必须在InitPush之前设置回调
- 所有回调参数都是可选的，只需设置需要的回调

#### 1.3 SetIOSMessageReceiver - 设置iOS消息接收回调

统一设置iOS平台的所有消息和通知回调。**必须在InitPush之前调用。**

**方法签名：**
```csharp
public static void SetIOSMessageReceiver(
    Action<string, string> onMessageReceived = null,
    Action<string> onAPNSRegisterSuccess = null,
    Action<string> onAPNSRegisterFailed = null,
    Action<string, string, string, string> onForegroundNotificationReceived = null,
    Action<string, string, string, string> onNotificationOpened = null,
    Action<string> onNotificationRemoved = null,
    Action<string, string, string, string> onRemoteNotificationReceived = null
)
```

**参数说明：**

- `onMessageReceived`：阿里云自有通道消息接收回调
  - **触发时机**：当收到阿里云自有通道的透传消息时
  - **参数1 (string title)**：消息标题
  - **参数2 (string body)**：消息内容
  - **说明**：透传消息不会在通知中心显示，需要开发者自行处理UI展示

- `onAPNSRegisterSuccess`：APNS注册成功回调
  - **触发时机**：设备成功向苹果APNS服务器注册并获取到DeviceToken时
  - **参数1 (string deviceToken)**：苹果推送服务分配的设备令牌
  - **说明**：DeviceToken用于服务端直接推送

- `onAPNSRegisterFailed`：APNS注册失败回调
  - **触发时机**：设备向苹果APNS服务器注册失败时
  - **参数1 (string error)**：注册失败的错误信息
  - **说明**：用于排查APNS注册问题，常见原因包括网络问题、证书配置错误等

- `onForegroundNotificationReceived`：前台通知接收回调
  - **触发时机**：应用在前台时收到APNs通知
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (string subtitle)**：通知副标题（iOS 10+支持）
  - **参数4 (string userInfo)**：通知用户信息JSON字符串，包含自定义字段
  - **说明**：iOS应用在前台时默认不显示通知，可通过此回调自定义处理

- `onNotificationOpened`：通知点击回调
  - **触发时机**：用户点击通知中心的推送通知时
  - **参数1 (string title)**：通知标题
  - **参数2 (string body)**：通知内容
  - **参数3 (string subtitle)**：通知副标题
  - **参数4 (string userInfo)**：通知用户信息JSON字符串
  - **说明**：用于处理用户点击通知后的跳转逻辑，应用会被唤醒或切换到前台

- `onNotificationRemoved`：通知移除回调
  - **触发时机**：用户手动清除通知中心的通知时触发。需要推送时设置iOSNotificationCategory参数（PushV2中为Ios下的Category参数）为`aliyun_push_category`。如果您希望自定义category名称，请同步修改`Assets/Plugins/iOS/alicloud-push/PushWrapper.m`文件中`createCustomNotificationCategory`方法里的category名称
  - **参数1 (string userInfo)**：通知用户信息JSON字符串
  - **说明**：用于统计通知的清除情况，iOS 10+支持

- `onRemoteNotificationReceived`：静默通知接收回调
  - **触发时机**：应用收到静默推送时触发。需要推送时设置iOSSilentNotification参数（PushV2中为Ios下的Silent参数）为`true`
  - **参数1 (string title)**：通知标题（静默推送可能为空）
  - **参数2 (string body)**：通知内容（静默推送可能为空）
  - **参数3 (string subtitle)**：通知副标题（静默推送可能为空）
  - **参数4 (string userInfo)**：通知用户信息JSON字符串
  - **说明**：用于处理静默推送，如数据同步、内容预加载等，不会显示通知

**平台支持：** 仅iOS

**使用示例：**
```csharp
#if UNITY_IOS
PushHelper.SetIOSMessageReceiver(
    onMessageReceived: (title, body) => {
        Debug.Log("onMessageReceived - title: " + title + ", body: " + body);
    },
    onAPNSRegisterSuccess: (deviceToken) => {
        Debug.Log("onAPNSRegisterSuccess - deviceToken: " + deviceToken);
    },
    onAPNSRegisterFailed: (error) => {
        Debug.Log("onAPNSRegisterFailed - error: " + error);
    },
    onForegroundNotificationReceived: (title, body, subtitle, userInfo) => {
        Debug.Log("onForegroundNotificationReceived - title: " + title + ", body: " + body + ", subtitle: " + subtitle + ", userInfo: " + userInfo);
    },
    onNotificationOpened: (title, body, subtitle, userInfo) => {
        Debug.Log("onNotificationOpened - title: " + title + ", body: " + body + ", subtitle: " + subtitle + ", userInfo: " + userInfo);
    },
    onNotificationRemoved: (userInfo) => {
        Debug.Log("onNotificationRemoved - userInfo: " + userInfo);
    },
    onRemoteNotificationReceived: (title, body, subtitle, userInfo) => {
        Debug.Log("onRemoteNotificationReceived - title: " + title + ", body: " + body + ", subtitle: " + subtitle + ", userInfo: " + userInfo);
    }
);
#endif
```

**注意事项：**
- 必须在InitPush之前设置回调
- 所有回调参数都是可选的，只需设置需要的回调

#### 1.4 SetIOSForegroundNotificationMode - 设置iOS前台通知处理模式

设置iOS应用在前台时收到通知的处理方式。

**方法签名：**
```csharp
public static void SetIOSForegroundNotificationMode(string mode)
```

**参数说明：**
- `mode`：处理模式，可选值：
  - `PushHelper.IOS_FOREGROUND_SHOW_AND_CALLBACK`：展示通知且调用回调
  - `PushHelper.IOS_FOREGROUND_SHOW_ONLY`：只展示通知，不调用回调
  - `PushHelper.IOS_FOREGROUND_CALLBACK_ONLY`：只调用回调，不展示通知

**平台支持：** 仅iOS

**使用示例：**
```csharp
#if UNITY_IOS
PushHelper.SetIOSForegroundNotificationMode(PushHelper.IOS_FOREGROUND_SHOW_AND_CALLBACK);
#endif
```

### 2. 初始化与注册

#### 2.1 InitPush - 初始化推送服务

初始化推送SDK，这是使用推送服务的核心步骤。

**方法签名：**
```csharp
public static void InitPush(string appKey, string appSecret, Action<bool, string> callback)
```

**参数说明：**
- `appKey`：应用的AppKey，从阿里云推送控制台获取
- `appSecret`：应用的AppSecret，从阿里云推送控制台获取
- `callback`：初始化结果回调
  - `bool`：是否成功
  - `string`：失败时返回错误信息

**平台支持：** iOS & Android

**使用示例：**
```csharp
PushHelper.InitPush("您的AppKey", "您的AppSecret", (success, message) => {
    if (success) {
        Debug.Log("推送初始化成功: " + message);
    } else {
        Debug.LogError("推送初始化失败: " + message);
    }
});
```

**注意事项：**
- Android的AppKey和AppSecret也需要在AndroidManifest.xml中配置
- 建议在应用启动时尽早调用此方法
- 初始化成功后才能使用其他推送功能

#### 2.2 InitAndroidThirdPush - 初始化Android厂商通道

初始化Android第三方推送通道（小米、华为、OPPO、vivo、魅族等），提高离线推送到达率。

**方法签名：**
```csharp
public static void InitAndroidThirdPush()
```

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
PushHelper.InitAndroidThirdPush();
#endif
```

**注意事项：**
- 需要在AndroidManifest.xml中配置对应厂商的参数
- 仅在对应品牌的设备上生效
- 建议在InitPush成功后调用

### 3. 账号管理

#### 3.1 BindAccount - 绑定账号

将设备与业务账号绑定，绑定后可以通过账号进行推送。

**方法签名：**
```csharp
public static void BindAccount(string account, Action<bool, string> callback)
```

**参数说明：**
- `account`：业务账号，如用户ID
- `callback`：绑定结果回调

**平台支持：** iOS & Android

**使用示例：**
```csharp
PushHelper.BindAccount("user123", (success, message) => {
    if (success) {
        Debug.Log("账号绑定成功");
    } else {
        Debug.LogError("账号绑定失败: " + message);
    }
});
```

**注意事项：**
- 一个设备只能绑定一个账号，重复绑定会覆盖
- 账号推送可以实现精准推送到特定用户

#### 3.2 UnbindAccount - 解绑账号

解除设备与业务账号的绑定关系。

**方法签名：**
```csharp
public static void UnbindAccount(Action<bool, string> callback)
```

**参数说明：**
- `callback`：解绑结果回调

**平台支持：** iOS & Android

**使用示例：**
```csharp
PushHelper.UnbindAccount((success, message) => {
    if (success) {
        Debug.Log("账号解绑成功");
    } else {
        Debug.LogError("账号解绑失败: " + message);
    }
});
```

### 4. 标签管理

标签用于对设备、账号或别名进行分类，便于批量推送。

#### 4.1 BindTag - 绑定标签

给指定目标绑定标签。

**方法签名：**
```csharp
public static void BindTag(int target, string tag, string alias, Action<bool, string> callback)
```

**参数说明：**
- `target`：目标类型
  - `1`：设备（CloudPushService.DEVICE_TARGET）
  - `2`：账号（CloudPushService.ACCOUNT_TARGET）
  - `3`：别名（CloudPushService.ALIAS_TARGET）
- `tag`：标签名称
- `alias`：别名（仅当target=3时需要）
- `callback`：绑定结果回调

**平台支持：** iOS & Android

**使用示例：**

```csharp
// 给设备绑定标签
PushHelper.BindTag(1, "VIP用户", null, (success, message) => {
    if (success) {
        Debug.Log("设备标签绑定成功");
    } else {
        Debug.LogError("标签绑定失败: " + message);
    }
});
```

**注意事项：**
- 一个目标可以绑定多个标签
- 标签名称建议使用有意义的分类名称

#### 4.2 UnbindTag - 解绑标签

解除指定目标的标签绑定。

**方法签名：**
```csharp
public static void UnbindTag(int target, string tag, string alias, Action<bool, string> callback)
```

**参数说明：**
- `target`：目标类型（1=设备, 2=账号, 3=别名）
- `tag`：标签名称
- `alias`：别名（仅当target=3时需要）
- `callback`：解绑结果回调

**平台支持：** iOS & Android

**使用示例：**
```csharp
// 解绑设备标签
PushHelper.UnbindTag(1, "VIP用户", null, (success, message) => {
    if (success) {
        Debug.Log("标签解绑成功");
    } else {
        Debug.LogError("标签解绑失败: " + message);
    }
});
```

**注意事项：**
- 只能解绑已存在的标签

#### 4.3 ListTags - 查询标签列表

查询指定目标绑定的所有标签。

**方法签名：**
```csharp
public static void ListTags(int target, Action<bool, string> callback)
```

**参数说明：**
- `target`：目标类型（1=设备）
- `callback`：查询结果回调，成功时返回标签列表

**平台支持：** iOS & Android

**使用示例：**
```csharp
// 查询设备的所有标签
PushHelper.ListTags(1, (success, tags) => {
    if (success) {
        Debug.Log("标签列表: " + tags);
    } else {
        Debug.LogError("查询失败: " + tagsJson);
    }
});
```

### 5. 别名管理

别名是设备的自定义标识，一个设备可以有多个别名。

#### 5.1 AddAlias - 添加别名

给设备添加别名。

**方法签名：**
```csharp
public static void AddAlias(string alias, Action<bool, string> callback)
```

**参数说明：**
- `alias`：别名名称
- `callback`：添加结果回调

**平台支持：** iOS & Android

**使用示例：**
```csharp
PushHelper.AddAlias("player_001", (success, message) => {
    if (success) {
        Debug.Log("别名添加成功");
    } else {
        Debug.LogError("别名添加失败: " + message);
    }
});
```

**注意事项：**
- 一个设备可以有多个别名
- 别名可以用于更灵活的推送场景
- 别名名称建议使用有意义的标识

#### 5.2 RemoveAlias - 移除别名

移除设备的指定别名。

**方法签名：**
```csharp
public static void RemoveAlias(string alias, Action<bool, string> callback)
```

**参数说明：**
- `alias`：别名名称
- `callback`：移除结果回调

**平台支持：** iOS & Android

**使用示例：**
```csharp
// 移除别名
PushHelper.RemoveAlias("player_001", (success, message) => {
    if (success) {
        Debug.Log("别名移除成功");
    } else {
        Debug.LogError("别名移除失败: " + message);
    }
});
```

**注意事项：**
- 只能移除已存在的别名
- 移除后将无法通过该别名推送到设备

#### 5.3 ListAliases - 查询别名列表

查询设备的所有别名。

**方法签名：**
```csharp
public static void ListAliases(Action<bool, string> callback)
```

**参数说明：**
- `callback`：查询结果回调，成功时返回别名列表

**平台支持：** iOS & Android

**使用示例：**
```csharp
// 查询设备的所有别名
PushHelper.ListAliases((success, aliases) => {
    if (success) {
        Debug.Log("别名列表: " + aliases);
    } else {
        Debug.LogError("查询失败: " + aliasesJson);
    }
});
```

### 6. 手机号管理（仅Android）

手机号绑定用于推送短信融合方案。

#### 6.1 BindPhoneNumber - 绑定手机号

给设备绑定手机号。

**方法签名：**
```csharp
public static void BindPhoneNumber(string phone, Action<bool, string> callback)
```

**参数说明：**
- `phone`：手机号码
- `callback`：绑定结果回调

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
PushHelper.BindPhoneNumber("13800138000", (success, message) => {
    if (success) {
        Debug.Log("手机号绑定成功");
    } else {
        Debug.LogError("手机号绑定失败: " + message);
    }
});
#endif
```

**注意事项：**
- 仅Android平台支持
- 用于推送短信融合方案
- 一个设备只能绑定一个手机号

#### 6.2 UnbindPhoneNumber - 解绑手机号

解除设备与手机号的绑定。

**方法签名：**
```csharp
public static void UnbindPhoneNumber(Action<bool, string> callback)
```

**参数说明：**
- `callback`：解绑结果回调

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
// 解绑手机号
PushHelper.UnbindPhoneNumber((success, message) => {
    if (success) {
        Debug.Log("手机号解绑成功");
    } else {
        Debug.LogError("手机号解绑失败: " + message);
    }
});
#endif
```

**注意事项：**
- 解绑后将无法使用推送短信融合功能

### 7. 设备信息

#### 7.1 GetDeviceId - 获取设备ID

获取推送服务分配的设备唯一标识符。

**方法签名：**
```csharp
public static string GetDeviceId()
```

**返回值：**
- 设备ID字符串

**平台支持：** iOS & Android

**使用示例：**
```csharp
string deviceId = PushHelper.GetDeviceId();
Debug.Log("设备ID: " + deviceId);
```

**注意事项：**
- 需要在InitPush成功后调用
- 设备ID是推送服务的唯一标识
- 可用于服务端API进行设备推送

### 8. 通知权限检查（仅Android）

Android平台提供了通知权限检查和设置跳转功能。

#### 8.1 IsNotificationEnabled - 检查通知权限

检查用户是否允许应用显示通知。

**方法签名：**
```csharp
public static bool IsNotificationEnabled()
```

**返回值：**
- `true`：通知权限已开启
- `false`：通知权限未开启

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
bool isEnabled = PushHelper.IsNotificationEnabled();
Debug.Log("通知权限状态: " + isEnabled);
#endif
```

**注意事项：**
- 建议在应用启动时检查通知权限
- 如果权限未开启，引导用户去设置

#### 8.2 IsNotificationChannelEnabled - 检查通知渠道权限

检查指定通知渠道是否已开启（Android 8.0+）。

**方法签名：**
```csharp
public static bool IsNotificationChannelEnabled(string channelId)
```

**参数说明：**
- `channelId`：通知渠道ID

**返回值：**
- `true`：通知渠道已开启
- `false`：通知渠道未开启

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
bool isEnabled = PushHelper.IsNotificationChannelEnabled("default_channel");
Debug.Log("通知渠道状态: " + isEnabled);
#endif
```

**注意事项：**
- 仅Android 8.0及以上版本有效
- 需要先创建通知渠道

#### 8.3 JumpToNotificationSetting - 跳转通知设置

跳转到应用的通知设置页面。

**方法签名：**
```csharp
public static void JumpToNotificationSetting()
```

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
PushHelper.JumpToNotificationSetting();
#endif
```

**注意事项：**
- 会打开系统设置页面，用户需要手动开启
- 建议在检测到权限未开启时引导用户

#### 8.4 JumpToNotificationChannelSetting - 跳转通知渠道设置

跳转到指定通知渠道的设置页面。

**方法签名：**
```csharp
public static void JumpToNotificationChannelSetting(string channelId)
```

**参数说明：**
- `channelId`：通知渠道ID

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
PushHelper.JumpToNotificationChannelSetting("default_channel");
#endif
```

**注意事项：**
- 仅Android 8.0及以上版本有效
- 部分设备可能不支持，会跳转到通知设置页面

#### 8.5 CreateNotificationChannel - 创建通知渠道

创建Android通知渠道（Android 8.0+必需）。

**方法签名：**
```csharp
public static void CreateNotificationChannel(
    string id,
    string name,
    int importance = 3,
    string desc = "",
    bool showBadge = true,
    bool enableLights = true,
    int lightColor = 0,
    bool enableVibration = true,
    string vibrationPattern = ""
)
```

**参数说明：**
- `id`：渠道ID（唯一标识）
- `name`：渠道名称（用户可见）
- `importance`：重要性级别（0-5）
  - `PushHelper.IMPORTANCE_NONE` (0)：不显示通知
  - `PushHelper.IMPORTANCE_MIN` (1)：最低级别，不发出声音
  - `PushHelper.IMPORTANCE_LOW` (2)：低级别，不发出声音
  - `PushHelper.IMPORTANCE_DEFAULT` (3)：默认级别，发出声音
  - `PushHelper.IMPORTANCE_HIGH` (4)：高级别，发出声音并弹出
  - `PushHelper.IMPORTANCE_MAX` (5)：最高级别
- `desc`：渠道描述
- `showBadge`：是否显示角标
- `enableLights`：是否启用指示灯
- `lightColor`：指示灯颜色（ARGB格式）
- `enableVibration`：是否启用震动
- `vibrationPattern`：震动模式（逗号分隔的毫秒数）

**平台支持：** 仅Android

**使用示例：**
```csharp
#if UNITY_ANDROID
PushHelper.CreateNotificationChannel(
    id: "default_channel",
    name: "默认通知",
    importance: PushHelper.IMPORTANCE_DEFAULT,
    desc: "应用默认通知渠道",
    showBadge: true,
    enableLights: true,
    lightColor: unchecked((int)0xFF0000FF),  // 蓝色
    enableVibration: true,
    vibrationPattern: "0,250,250,250"
);
#endif
```

**注意事项：**
- Android 8.0及以上版本必须创建通知渠道
- 建议在应用启动时创建
- 渠道创建后，用户可以在系统设置中管理
- lightColor使用ARGB格式，注意使用unchecked避免溢出

## 完整使用示例

以下是一个完整的推送集成示例：

```csharp
using Aliyun.Push;
using UnityEngine;

public class PushManager : MonoBehaviour 
{
    void Start() 
    {
        // 设置日志等级
        PushHelper.SetLogLevel(PushHelper.LOG_DEBUG);
#if UNITY_IOS
        // 设置iOS消息接收回调
        PushHelper.SetIOSMessageReceiver(
            onMessageReceived: (title, body) => {
                Log("onMessageReceived\ntitle: " + title + "\nbody: " + body);
            },
            onAPNSRegisterSuccess: (deviceToken) => {
                Log("onAPNSRegisterSuccess\ndeviceToken: " + deviceToken);
            },
            onAPNSRegisterFailed: (error) => {
                Log("onAPNSRegisterFailed\nerror: " + error);
            },
            onForegroundNotificationReceived: (title, body, subtitle, userInfo) => {
                Log("onForegroundNotificationReceived\ntitle: " + title + "\nbody: " + body + "\nsubtitle: " + subtitle + "\nuserInfo: " + userInfo);
            },
            onNotificationOpened: (title, body, subtitle, userInfo) => {
                Log("onNotificationOpened\ntitle: " + title + "\nbody: " + body + "\nsubtitle: " + subtitle + "\nuserInfo: " + userInfo);
            },
            onNotificationRemoved: (userInfo) => {
                Log("onNotificationRemoved\nuserInfo: " + userInfo);
            },
            onRemoteNotificationReceived: (title, body, subtitle, userInfo) => {
                Log("onRemoteNotificationReceived\ntitle: " + title + "\nbody: " + body + "\nsubtitle: " + subtitle + "\nuserInfo: " + userInfo);
            }
        );
        // 设置iOS前台通知处理模式
        PushHelper.SetIOSForegroundNotificationMode(PushHelper.IOS_FOREGROUND_SHOW_AND_CALLBACK);
#elif UNITY_ANDROID
        // 创建通知渠道（Android 8.0+）
        PushHelper.CreateNotificationChannel(
            id: "8.0up",
            name: "Aliyun Channel",
            importance: PushHelper.IMPORTANCE_DEFAULT,
            desc: "aliyun notifications",
            showBadge: true,
            enableLights: true,
            lightColor: unchecked((int)0xFF0000FF),  // 蓝色
            enableVibration: true,
            vibrationPattern: "0,250,250,250"  // 震动模式
        );
        // 设置安卓消息接收回调
        PushHelper.SetAndroidMessageReceiver(
            onMessage: (title, body) => {
                Log("onMessage\ntitle: " + title + "\nbody: " + body);
            },
            onNotification: (title, body, extraMap) => {
                string extras = extraMap != null ? string.Join(", ", extraMap) : "";
                Log("onNotification\ntitle: " + title + "\nbody: " + body + "\nextraMap: " + extras);
            },
            onNotificationOpened: (title, body, extraMapString) => {
                Log("onNotificationOpened\ntitle: " + title + "\nbody: " + body + "\nextraMapString: " + extraMapString);
            },
            onNotificationRemoved: (messageId) => {
                Log("onNotificationRemoved\nmessageId: " + messageId);
            },
            onSysNoticeOpened: (title, body, extraMap) => {
                string extras = extraMap != null ? string.Join(", ", extraMap) : "";
                Log("onSysNoticeOpened\ntitle: " + title + "\nbody: " + body + "\nextraMap: " + extras);
            }
        );
#endif
        // 初始化推送服务
        PushHelper.InitPush("xxxxx", "xxxxxxx", (result, data) => {
            Log("Register result: " + result + " data: " + data);
        });
#if UNITY_ANDROID
        // 初始化安卓厂商通道
        PushHelper.InitAndroidThirdPush();
#endif
    }
}
```

## API快速参考

### 跨平台API

| 方法                                      | 说明           | 平台支持      |
| ----------------------------------------- | -------------- | ------------- |
| `InitPush(appKey, appSecret, callback)`   | 初始化推送服务 | iOS & Android |
| `BindAccount(account, callback)`          | 绑定账号       | iOS & Android |
| `UnbindAccount(callback)`                 | 解绑账号       | iOS & Android |
| `BindTag(target, tag, alias, callback)`   | 绑定标签       | iOS & Android |
| `UnbindTag(target, tag, alias, callback)` | 解绑标签       | iOS & Android |
| `ListTags(target, callback)`              | 查询标签列表   | iOS & Android |
| `AddAlias(alias, callback)`               | 添加别名       | iOS & Android |
| `RemoveAlias(alias, callback)`            | 移除别名       | iOS & Android |
| `ListAliases(callback)`                   | 查询别名列表   | iOS & Android |
| `GetDeviceId()`                           | 获取设备ID     | iOS & Android |
| `SetLogLevel(logLevel)`                   | 设置日志级别   | iOS & Android |

### Android专属API

| 方法                                          | 说明             |
| --------------------------------------------- | ---------------- |
| `InitAndroidThirdPush()`                      | 初始化厂商通道   |
| `BindPhoneNumber(phone, callback)`            | 绑定手机号       |
| `UnbindPhoneNumber(callback)`                 | 解绑手机号       |
| `SetAndroidMessageReceiver(...)`              | 设置消息接收器   |
| `IsNotificationEnabled()`                     | 检查通知权限     |
| `IsNotificationChannelEnabled(channelId)`     | 检查通知渠道权限 |
| `JumpToNotificationSetting()`                 | 跳转通知设置     |
| `JumpToNotificationChannelSetting(channelId)` | 跳转通知渠道设置 |
| `CreateNotificationChannel(...)`              | 创建通知渠道     |

### iOS专属API

| 方法                                     | 说明                 |
| ---------------------------------------- | -------------------- |
| `SetIOSMessageReceiver(...)`             | 设置消息接收器       |
| `SetIOSForegroundNotificationMode(mode)` | 设置前台通知处理模式 |

### 回调说明

大部分API的回调格式为 `Action<bool, string>`：
- 第一个参数 `bool`：表示操作是否成功
- 第二个参数 `string`：成功时返回数据，失败时返回错误信息

## 示例demo

完整的示例demo请参考项目中的`Assets/ClickHandler.cs`文件。

## 更新日志

详见 [CHANGELOG.md](CHANGELOG.md)
