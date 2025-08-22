using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class HttpDnsUICreator : MonoBehaviour
{
    [MenuItem("HttpDNS/Create DNS Test UI")]
    public static void CreateDNSTestUI()
    {
        // 创建Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // 创建EventSystem
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        // 创建主面板 - 增加安全区域边距，适配手机屏幕
        GameObject mainPanel = CreateUIElement("MainPanel", canvasGO.transform);
        RectTransform mainPanelRect = mainPanel.GetComponent<RectTransform>();
        mainPanelRect.anchorMin = Vector2.zero;
        mainPanelRect.anchorMax = Vector2.one;
        mainPanelRect.offsetMin = new Vector2(30, 80); // 增加底部边距
        mainPanelRect.offsetMax = new Vector2(-30, -30);
        
        // 创建标题 - 调整位置和大小
        GameObject title = CreateTextElement("Title", "HttpDNS 测试工具", mainPanel.transform);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.92f);
        titleRect.anchorMax = new Vector2(1, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        Text titleText = title.GetComponent<Text>();
        titleText.fontSize = 24; // 减小字体
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        // 创建URL输入框
        GameObject urlInputField = CreateInputField("URLInputField", "请输入测试URL...", mainPanel.transform);
        RectTransform urlInputRect = urlInputField.GetComponent<RectTransform>();
        urlInputRect.anchorMin = new Vector2(0.05f, 0.82f);
        urlInputRect.anchorMax = new Vector2(0.95f, 0.9f);
        urlInputRect.offsetMin = Vector2.zero;
        urlInputRect.offsetMax = Vector2.zero;
        
        // 创建按钮区域 - 调整位置和高度
        GameObject buttonPanel = CreateUIElement("ButtonPanel", mainPanel.transform);
        RectTransform buttonPanelRect = buttonPanel.GetComponent<RectTransform>();
        buttonPanelRect.anchorMin = new Vector2(0.05f, 0.62f);
        buttonPanelRect.anchorMax = new Vector2(0.95f, 0.8f);
        buttonPanelRect.offsetMin = Vector2.zero;
        buttonPanelRect.offsetMax = Vector2.zero;
        
        // 创建DNS解析按钮 - 增加间距
        GameObject dnsButton = CreateButton("DNSResolveButton", "DNS解析", buttonPanel.transform);
        RectTransform dnsButtonRect = dnsButton.GetComponent<RectTransform>();
        dnsButtonRect.anchorMin = new Vector2(0f, 0.55f);
        dnsButtonRect.anchorMax = new Vector2(0.22f, 1f);
        dnsButtonRect.offsetMin = Vector2.zero;
        dnsButtonRect.offsetMax = Vector2.zero;
        
        // 创建请求按钮 - 调整间距
        GameObject unityWebRequestButton = CreateButton("UnityWebRequestButton", "UnityWeb", buttonPanel.transform);
        RectTransform unityWebRequestRect = unityWebRequestButton.GetComponent<RectTransform>();
        unityWebRequestRect.anchorMin = new Vector2(0.25f, 0.55f);
        unityWebRequestRect.anchorMax = new Vector2(0.47f, 1f);
        unityWebRequestRect.offsetMin = Vector2.zero;
        unityWebRequestRect.offsetMax = Vector2.zero;
        
        GameObject httpWebRequestButton = CreateButton("HttpWebRequestButton", "HttpWeb", buttonPanel.transform);
        RectTransform httpWebRequestRect = httpWebRequestButton.GetComponent<RectTransform>();
        httpWebRequestRect.anchorMin = new Vector2(0.5f, 0.55f);
        httpWebRequestRect.anchorMax = new Vector2(0.72f, 1f);
        httpWebRequestRect.offsetMin = Vector2.zero;
        httpWebRequestRect.offsetMax = Vector2.zero;
        
        GameObject httpClientButton = CreateButton("HttpClientButton", "HttpClient", buttonPanel.transform);
        RectTransform httpClientRect = httpClientButton.GetComponent<RectTransform>();
        httpClientRect.anchorMin = new Vector2(0.75f, 0.55f);
        httpClientRect.anchorMax = new Vector2(1f, 1f);
        httpClientRect.offsetMin = Vector2.zero;
        httpClientRect.offsetMax = Vector2.zero;
        
        // 创建清除按钮（放在第二行右侧）
        GameObject clearButton = CreateButton("ClearButton", "清空结果", buttonPanel.transform);
        RectTransform clearButtonRect = clearButton.GetComponent<RectTransform>();
        clearButtonRect.anchorMin = new Vector2(0.75f, 0f);
        clearButtonRect.anchorMax = new Vector2(1f, 0.45f);
        clearButtonRect.offsetMin = Vector2.zero;
        clearButtonRect.offsetMax = Vector2.zero;
        
        // 创建结果显示区域 - 调整位置，确保手机上可见
        GameObject resultScrollView = CreateScrollView("ResultScrollView", mainPanel.transform);
        RectTransform resultScrollRect = resultScrollView.GetComponent<RectTransform>();
        resultScrollRect.anchorMin = new Vector2(0.05f, 0.08f); // 增加底部边距
        resultScrollRect.anchorMax = new Vector2(0.95f, 0.6f);
        resultScrollRect.offsetMin = Vector2.zero;
        resultScrollRect.offsetMax = Vector2.zero;
        
        // 创建HttpDnsDemo组件控制器
        GameObject controller = new GameObject("HttpDnsController");
        HttpDnsDemo demoScript = controller.AddComponent<HttpDnsDemo>();
        
        // 连接UI组件
        demoScript.urlInputField = urlInputField.GetComponent<InputField>();
        demoScript.dnsResolveButton = dnsButton.GetComponent<Button>();
        demoScript.unityWebRequestButton = unityWebRequestButton.GetComponent<Button>();
        demoScript.httpWebRequestButton = httpWebRequestButton.GetComponent<Button>();
        demoScript.httpClientButton = httpClientButton.GetComponent<Button>();
        demoScript.clearButton = clearButton.GetComponent<Button>();
        demoScript.resultText = resultScrollView.transform.Find("Viewport/Content/ResultText").GetComponent<Text>();
        demoScript.resultScrollRect = resultScrollView.GetComponent<ScrollRect>();
        
        // 清除按钮事件已在HttpDnsDemo.Start()中自动绑定
        Debug.Log("HttpDNS 测试UI创建完成！清除按钮已连接。");
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
        textComponent.fontSize = 16;
        textComponent.color = Color.black;
        return go;
    }
    
    static GameObject CreateButton(string name, string text, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        
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
        textComponent.fontSize = 12; // 减小字体以适应按钮大小
        
        return go;
    }
    
    static GameObject CreateInputField(string name, string placeholder, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        image.type = Image.Type.Sliced;
        
        InputField inputField = go.AddComponent<InputField>();
        
        GameObject textGO = CreateTextElement("Text", "", go.transform);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);
        
        GameObject placeholderGO = CreateTextElement("Placeholder", placeholder, go.transform);
        RectTransform placeholderRect = placeholderGO.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(10, 5);
        placeholderRect.offsetMax = new Vector2(-10, -5);
        
        Text placeholderText = placeholderGO.GetComponent<Text>();
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.fontStyle = FontStyle.Italic;
        
        inputField.textComponent = textGO.GetComponent<Text>();
        inputField.placeholder = placeholderText;
        inputField.text = "https://cube.elemecdn.com/favicon.ico";
        
        return go;
    }
    
    static GameObject CreateScrollView(string name, Transform parent)
    {
        GameObject go = CreateUIElement(name, parent);
        
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        image.type = Image.Type.Sliced;
        
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
        contentRect.sizeDelta = new Vector2(0, 300); // 减小初始高度
        
        // 简化Content设置，不使用VerticalLayoutGroup
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        GameObject resultText = CreateTextElement("ResultText", "等待测试结果...", content.transform);
        RectTransform resultTextRect = resultText.GetComponent<RectTransform>();
        // 设置文本占满整个Content区域
        resultTextRect.anchorMin = Vector2.zero;
        resultTextRect.anchorMax = Vector2.one;
        resultTextRect.offsetMin = new Vector2(10, 10); // 添加内边距
        resultTextRect.offsetMax = new Vector2(-10, -10);
        
        Text textComponent = resultText.GetComponent<Text>();
        textComponent.alignment = TextAnchor.UpperLeft;
        textComponent.fontSize = 12; // 稍微减小字体以适应更多内容
        textComponent.verticalOverflow = VerticalWrapMode.Overflow; // 允许垂直溢出
        
        ContentSizeFitter textSizeFitter = resultText.AddComponent<ContentSizeFitter>();
        textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.verticalScrollbar = null; // 暂时不添加滚动条
        scrollRect.horizontalScrollbar = null;
        
        return go;
    }
}