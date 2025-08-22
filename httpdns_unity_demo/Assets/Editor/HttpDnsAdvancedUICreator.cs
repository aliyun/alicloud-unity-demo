using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// HttpDNS高级功能UI创建器 - 生成完整的测试界面
/// 覆盖所有高级接口的测试功能
/// </summary>
public class HttpDnsAdvancedUICreator : MonoBehaviour
{
    [MenuItem("HttpDNS/Create Advanced Test UI")]
    public static void CreateAdvancedTestUI()
    {
        // 创建Canvas
        GameObject canvasGO = new GameObject("AdvancedCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // 添加CanvasScaler实现UI自适应
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920); // 基于1080p手机屏幕
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; // 平衡宽度和高度缩放
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // 创建EventSystem
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        // 创建主面板 - 适配手机屏幕
        GameObject mainPanel = CreateUIElement("AdvancedMainPanel", canvasGO.transform);
        RectTransform mainPanelRect = mainPanel.GetComponent<RectTransform>();
        mainPanelRect.anchorMin = Vector2.zero;
        mainPanelRect.anchorMax = Vector2.one;
        mainPanelRect.offsetMin = new Vector2(15, 60); // 增加底部边距
        mainPanelRect.offsetMax = new Vector2(-15, -15);
        
        // 创建标题
        GameObject title = CreateTextElement("AdvancedTitle", "HttpDNS 高级功能测试", mainPanel.transform);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.95f);
        titleRect.anchorMax = new Vector2(1, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        Text titleText = title.GetComponent<Text>();
        titleText.fontSize = 28; // 增大标题字体，适配Android
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = Color.blue;
        
        // 创建状态显示
        GameObject status = CreateTextElement("StatusText", "状态: 未初始化", mainPanel.transform);
        RectTransform statusRect = status.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0.91f);
        statusRect.anchorMax = new Vector2(1, 0.94f);
        statusRect.offsetMin = Vector2.zero;
        statusRect.offsetMax = Vector2.zero;
        Text statusText = status.GetComponent<Text>();
        statusText.fontSize = 18; // 增大状态文字字体
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.red;
        
        // ========== 左侧：功能控制区 ==========
        GameObject leftPanel = CreateUIElement("LeftPanel", mainPanel.transform);
        RectTransform leftPanelRect = leftPanel.GetComponent<RectTransform>();
        leftPanelRect.anchorMin = new Vector2(0, 0.05f);
        leftPanelRect.anchorMax = new Vector2(0.55f, 0.9f);
        leftPanelRect.offsetMin = Vector2.zero;
        leftPanelRect.offsetMax = Vector2.zero;
        
        // === 第一组：配置管理区 ===
        float currentY = 0.95f;
        GameObject configGroupTitle = CreateTextElement("ConfigGroupTitle", "【配置管理组】", leftPanel.transform);
        SetRect(configGroupTitle, 0, currentY - 0.04f, 1, currentY);
        configGroupTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;
        configGroupTitle.GetComponent<Text>().color = new Color(0.2f, 0.4f, 0.6f); // 深蓝色，更低调
        currentY -= 0.06f;
        
        // 阶段初始化
        GameObject initTitle = CreateTextElement("InitTitle", "分阶段初始化:", leftPanel.transform);
        SetRect(initTitle, 0, currentY - 0.03f, 1, currentY);
        initTitle.GetComponent<Text>().fontSize = 14;
        currentY -= 0.04f;
        
        GameObject initButton = CreateButton("InitButton", "阶段1:初始化", leftPanel.transform);
        SetRect(initButton, 0, currentY - 0.04f, 0.48f, currentY);
        
        GameObject buildButton = CreateButton("BuildButton", "阶段2:构建", leftPanel.transform);
        SetRect(buildButton, 0.52f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.06f;
        
        // 基础配置
        GameObject basicConfigTitle = CreateTextElement("BasicConfigTitle", "基础配置:", leftPanel.transform);
        SetRect(basicConfigTitle, 0, currentY - 0.03f, 1, currentY);
        basicConfigTitle.GetComponent<Text>().fontSize = 14;
        currentY -= 0.04f;
        
        GameObject debugButton = CreateButton("DebugButton", "调试:关", leftPanel.transform);
        SetRect(debugButton, 0, currentY - 0.04f, 0.32f, currentY);
        
        GameObject httpsButton = CreateButton("HttpsButton", "HTTPS:开", leftPanel.transform);
        SetRect(httpsButton, 0.34f, currentY - 0.04f, 0.66f, currentY);
        
        GameObject timeoutButton = CreateButton("TimeoutButton", "设置超时", leftPanel.transform);
        SetRect(timeoutButton, 0.68f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.06f;
        
        // 高级配置
        GameObject advancedConfigTitle = CreateTextElement("AdvancedConfigTitle", "高级配置:", leftPanel.transform);
        SetRect(advancedConfigTitle, 0, currentY - 0.03f, 1, currentY);
        advancedConfigTitle.GetComponent<Text>().fontSize = 14;
        currentY -= 0.04f;
        
        GameObject persistentCacheButton = CreateButton("PersistentCacheButton", "持久缓存:开", leftPanel.transform);
        SetRect(persistentCacheButton, 0, currentY - 0.04f, 0.48f, currentY);
        
        GameObject reuseExpiredButton = CreateButton("ReuseExpiredButton", "过期复用:开", leftPanel.transform);
        SetRect(reuseExpiredButton, 0.52f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.05f;
        
        GameObject networkChangedButton = CreateButton("NetworkChangedButton", "网络预解析:开", leftPanel.transform);
        SetRect(networkChangedButton, 0, currentY - 0.04f, 0.48f, currentY);
        
        GameObject sessionIdButton = CreateButton("SessionIdButton", "获取会话ID", leftPanel.transform);
        SetRect(sessionIdButton, 0.52f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.05f;
        
        GameObject batchPreResolveButton = CreateButton("BatchPreResolveButton", "批量预解析", leftPanel.transform);
        SetRect(batchPreResolveButton, 0, currentY - 0.04f, 0.48f, currentY);
        
        GameObject clearCacheButton = CreateButton("ClearCacheButton", "清除缓存", leftPanel.transform);
        SetRect(clearCacheButton, 0.52f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.05f;
        
        GameObject cleanAllCacheButton = CreateButton("CleanAllCacheButton", "清理所有缓存", leftPanel.transform);
        SetRect(cleanAllCacheButton, 0, currentY - 0.04f, 0.48f, currentY);
        
        GameObject clearResultButton = CreateButton("ClearResultButton", "清空结果", leftPanel.transform);
        SetRect(clearResultButton, 0.52f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.08f;
        
        // === 第二组：解析和请求区 ===
        GameObject requestGroupTitle = CreateTextElement("RequestGroupTitle", "【解析和请求组】", leftPanel.transform);
        SetRect(requestGroupTitle, 0, currentY - 0.04f, 1, currentY);
        requestGroupTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;
        requestGroupTitle.GetComponent<Text>().color = new Color(0.2f, 0.6f, 0.3f); // 深绿色，更低调
        currentY -= 0.06f;
        
        // 域名输入
        GameObject hostInputField = CreateInputField("HostInputField", "输入域名...", leftPanel.transform);
        SetRect(hostInputField, 0, currentY - 0.04f, 1f, currentY);
        currentY -= 0.06f;
        
        // DNS解析方法
        GameObject resolveTitle = CreateTextElement("ResolveTitle", "DNS解析方法:", leftPanel.transform);
        SetRect(resolveTitle, 0, currentY - 0.03f, 1, currentY);
        resolveTitle.GetComponent<Text>().fontSize = 14;
        currentY -= 0.04f;
        
        GameObject singleResolveButton = CreateButton("SingleResolveButton", "单域名解析", leftPanel.transform);
        SetRect(singleResolveButton, 0, currentY - 0.04f, 0.32f, currentY);
        
        GameObject asyncResolveButton = CreateButton("AsyncResolveButton", "异步解析", leftPanel.transform);
        SetRect(asyncResolveButton, 0.34f, currentY - 0.04f, 0.66f, currentY);
        
        GameObject preResolveButton = CreateButton("PreResolveButton", "预解析", leftPanel.transform);
        SetRect(preResolveButton, 0.68f, currentY - 0.04f, 1f, currentY);
        currentY -= 0.06f;
        
        // 网络请求方法
        GameObject requestTitle = CreateTextElement("RequestTitle", "网络请求方法:", leftPanel.transform);
        SetRect(requestTitle, 0, currentY - 0.03f, 1, currentY);
        requestTitle.GetComponent<Text>().fontSize = 14;
        currentY -= 0.04f;
        
        GameObject httpClientButton = CreateButton("HttpClientButton", "HttpClient(推荐)", leftPanel.transform);
        SetRect(httpClientButton, 0, currentY - 0.04f, 1f, currentY);
        currentY -= 0.05f;
        
        GameObject httpWebRequestButton = CreateButton("HttpWebRequestButton", "HttpWebRequest", leftPanel.transform);
        SetRect(httpWebRequestButton, 0, currentY - 0.04f, 1f, currentY);
        currentY -= 0.05f;
        
        GameObject unityWebRequestButton = CreateButton("UnityWebRequestButton", "UnityWebRequest(不推荐)", leftPanel.transform);
        SetRect(unityWebRequestButton, 0, currentY - 0.04f, 1f, currentY);
        
        // ========== 右侧：结果显示区 ==========
        GameObject rightPanel = CreateUIElement("RightPanel", mainPanel.transform);
        RectTransform rightPanelRect = rightPanel.GetComponent<RectTransform>();
        rightPanelRect.anchorMin = new Vector2(0.57f, 0.05f);
        rightPanelRect.anchorMax = new Vector2(1f, 0.9f);
        rightPanelRect.offsetMin = Vector2.zero;
        rightPanelRect.offsetMax = Vector2.zero;
        
        GameObject resultTitle = CreateTextElement("ResultTitle", "【测试结果】", rightPanel.transform);
        RectTransform resultTitleRect = resultTitle.GetComponent<RectTransform>();
        resultTitleRect.anchorMin = new Vector2(0, 0.95f);
        resultTitleRect.anchorMax = new Vector2(1, 1f);
        resultTitleRect.offsetMin = Vector2.zero;
        resultTitleRect.offsetMax = Vector2.zero;
        resultTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;
        resultTitle.GetComponent<Text>().color = Color.black;
        
        GameObject resultScrollView = CreateScrollView("ResultScrollView", rightPanel.transform);
        RectTransform resultScrollRect = resultScrollView.GetComponent<RectTransform>();
        resultScrollRect.anchorMin = new Vector2(0, 0);
        resultScrollRect.anchorMax = new Vector2(1, 0.94f);
        resultScrollRect.offsetMin = Vector2.zero;
        resultScrollRect.offsetMax = Vector2.zero;
        
        // 创建HttpDnsAdvancedDemo组件控制器
        GameObject controller = new GameObject("HttpDnsAdvancedController");
        HttpDnsAdvancedDemo advancedScript = controller.AddComponent<HttpDnsAdvancedDemo>();
        
        // 连接UI组件 - 基础功能
        advancedScript.initButton = initButton.GetComponent<Button>();
        advancedScript.buildButton = buildButton.GetComponent<Button>();
        advancedScript.debugToggleButton = debugButton.GetComponent<Button>();
        advancedScript.httpsToggleButton = httpsButton.GetComponent<Button>();
        advancedScript.timeoutButton = timeoutButton.GetComponent<Button>();
        advancedScript.clearCacheButton = clearCacheButton.GetComponent<Button>();
        
        // 连接UI组件 - 高级功能
        advancedScript.persistentCacheToggleButton = persistentCacheButton.GetComponent<Button>();
        advancedScript.reuseExpiredToggleButton = reuseExpiredButton.GetComponent<Button>();
        advancedScript.networkChangedToggleButton = networkChangedButton.GetComponent<Button>();
        advancedScript.sessionIdButton = sessionIdButton.GetComponent<Button>();
        advancedScript.batchPreResolveButton = batchPreResolveButton.GetComponent<Button>();
        advancedScript.cleanAllCacheButton = cleanAllCacheButton.GetComponent<Button>();
        
        // 连接UI组件 - 解析测试
        advancedScript.hostInputField = hostInputField.GetComponent<InputField>();
        advancedScript.singleResolveButton = singleResolveButton.GetComponent<Button>();
        advancedScript.asyncResolveButton = asyncResolveButton.GetComponent<Button>();
        advancedScript.preResolveButton = preResolveButton.GetComponent<Button>();
        advancedScript.clearButton = clearResultButton.GetComponent<Button>();
        
        // 连接UI组件 - 网络请求方法（新增）
        advancedScript.httpClientButton = httpClientButton.GetComponent<Button>();
        advancedScript.httpWebRequestButton = httpWebRequestButton.GetComponent<Button>();
        advancedScript.unityWebRequestButton = unityWebRequestButton.GetComponent<Button>();
        
        // 连接结果显示组件
        advancedScript.resultText = resultScrollView.transform.Find("Viewport/Content/ResultText").GetComponent<Text>();
        advancedScript.resultScrollRect = resultScrollView.GetComponent<ScrollRect>();
        advancedScript.statusText = status.GetComponent<Text>();
        
        // 添加简化版字体自适应组件
        GameObject fontAdapter = new GameObject("UIFontAdapter");
        UIFontAdapter adapter = fontAdapter.AddComponent<UIFontAdapter>();
        adapter.fontScale = 1.2f;
        
        Debug.Log("HttpDNS 高级功能测试UI创建完成！");
        Debug.Log("已添加字体自适应功能，Android设备上字体会自动放大。");
        Debug.Log("覆盖所有高级接口的完整测试功能已就绪。");
    }
    
    static GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        return go;
    }
    
    static GameObject CreateTextElement(string name, string text, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        Text textComponent = go.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 16; // 增大文本字体，适配Android
        textComponent.color = Color.black;
        return go;
    }
    
    static GameObject CreateButton(string name, string text, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f); // 浅灰色按钮背景
        
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        
        GameObject textGO = CreateTextElement("Text", text, go.transform);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text textComponent = textGO.GetComponent<Text>();
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.fontSize = 14; // 增大按钮字体，适配Android
        textComponent.color = Color.black; // 黑色文字
        
        return go;
    }
    
    static GameObject CreateInputField(string name, string placeholder, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        image.type = Image.Type.Sliced;
        image.color = Color.white; // 白色输入框背景
        
        InputField inputField = go.AddComponent<InputField>();
        
        GameObject textGO = CreateTextElement("Text", "", go.transform);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 2);
        textRect.offsetMax = new Vector2(-5, -2);
        
        // 设置输入文字为黑色
        Text inputTextComponent = textGO.GetComponent<Text>();
        inputTextComponent.color = Color.black;
        
        GameObject placeholderGO = CreateTextElement("Placeholder", placeholder, go.transform);
        RectTransform placeholderRect = placeholderGO.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(5, 2);
        placeholderRect.offsetMax = new Vector2(-5, -2);
        
        Text placeholderText = placeholderGO.GetComponent<Text>();
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.fontStyle = FontStyle.Italic;
        
        inputField.textComponent = inputTextComponent;
        inputField.placeholder = placeholderText;
        inputField.text = "www.aliyun.com";
        
        return go;
    }
    
    static GameObject CreateScrollView(string name, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        image.type = Image.Type.Sliced;
        image.color = Color.white; // 设置为白色背景
        
        ScrollRect scrollRect = go.AddComponent<ScrollRect>();
        
        GameObject viewport = CreateUIElement("Viewport", go.transform);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        viewport.AddComponent<Image>();
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        
        GameObject content = CreateUIElement("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0, 1);
        contentRect.sizeDelta = new Vector2(0, 500);
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        GameObject resultText = CreateTextElement("ResultText", "等待测试开始...\n\n请按照顺序测试：\n\n1. 点击'阶段1:初始化'\n2. 点击'阶段2:构建'\n3. 测试各种高级功能\n\n📱 界面已优化手机显示", content.transform);
        RectTransform resultTextRect = resultText.GetComponent<RectTransform>();
        resultTextRect.anchorMin = Vector2.zero;
        resultTextRect.anchorMax = Vector2.one;
        resultTextRect.offsetMin = new Vector2(10, 10);  // 增加左边距和底边距
        resultTextRect.offsetMax = new Vector2(-10, -35); // 进一步增加顶部边距，彻底解决初始文字偏上问题
        
        Text textComponent = resultText.GetComponent<Text>();
        textComponent.alignment = TextAnchor.UpperLeft;
        textComponent.fontSize = 14; // 增大结果显示字体，适配Android
        textComponent.verticalOverflow = VerticalWrapMode.Overflow;
        
        ContentSizeFitter textSizeFitter = resultText.AddComponent<ContentSizeFitter>();
        textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        
        return go;
    }
    
    // 设置RectTransform位置的辅助方法
    static void SetRect(GameObject go, float left, float bottom, float right, float top)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}