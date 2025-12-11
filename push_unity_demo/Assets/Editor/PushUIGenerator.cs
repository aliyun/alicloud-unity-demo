using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.Events;

public class PushUIGenerator : EditorWindow
{
    // 创建圆角矩形精灵（使用Sliced模式，设置边界）
    static Sprite CreateRoundedRectSprite(int width, int height, int cornerRadius)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 判断是否在圆角区域
                bool inCorner = false;
                float distance = 0;
                
                // 左上角
                if (x < cornerRadius && y >= height - cornerRadius)
                {
                    float dx = cornerRadius - x;
                    float dy = y - (height - cornerRadius);
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                    inCorner = true;
                }
                // 右上角
                else if (x >= width - cornerRadius && y >= height - cornerRadius)
                {
                    float dx = x - (width - cornerRadius);
                    float dy = y - (height - cornerRadius);
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                    inCorner = true;
                }
                // 左下角
                else if (x < cornerRadius && y < cornerRadius)
                {
                    float dx = cornerRadius - x;
                    float dy = cornerRadius - y;
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                    inCorner = true;
                }
                // 右下角
                else if (x >= width - cornerRadius && y < cornerRadius)
                {
                    float dx = x - (width - cornerRadius);
                    float dy = cornerRadius - y;
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                    inCorner = true;
                }
                
                // 设置像素颜色
                if (inCorner)
                {
                    pixels[y * width + x] = distance <= cornerRadius ? Color.white : Color.clear;
                }
                else
                {
                    pixels[y * width + x] = Color.white;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // 创建Sprite并设置边界，使圆角不会被拉伸
        Sprite sprite = Sprite.Create(
            texture, 
            new Rect(0, 0, width, height), 
            new Vector2(0.5f, 0.5f), 
            100, 
            0, 
            SpriteMeshType.FullRect,
            new Vector4(cornerRadius, cornerRadius, cornerRadius, cornerRadius) // 设置边界
        );
        
        return sprite;
    }

    [MenuItem("Tools/生成推送测试UI")]
    static void GeneratePushUI()
    {
        try
        {
            // 创建Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 1f;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            
            canvasObj.AddComponent<GraphicRaycaster>();

            Debug.Log("Canvas创建成功");

            // 创建EventSystem
            GameObject existingEventSystem = GameObject.Find("EventSystem");
            if (existingEventSystem != null)
            {
                DestroyImmediate(existingEventSystem);
                Debug.Log("删除旧的EventSystem");
            }
            
            GameObject eventSystem = new GameObject("EventSystem");
            var es = eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            var inputModule = eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("EventSystem创建成功");
            Debug.Log("EventSystem: " + es + ", InputModule: " + inputModule);

            // 创建ScrollView（放在左侧，占60%宽度）
            GameObject scrollViewObj = new GameObject("ScrollView");
            scrollViewObj.transform.SetParent(canvasObj.transform, false);
            RectTransform scrollViewRect = scrollViewObj.AddComponent<RectTransform>();
            scrollViewRect.anchorMin = new Vector2(0, 0);
            scrollViewRect.anchorMax = new Vector2(0.6f, 1);
            scrollViewRect.offsetMin = new Vector2(20, 20);
            scrollViewRect.offsetMax = new Vector2(-10, -20);

            Image scrollViewImage = scrollViewObj.AddComponent<Image>();
            scrollViewImage.color = new Color(1f, 1f, 1f, 1f);
            
            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            scrollRect.scrollSensitivity = 30;

            // 创建Viewport
            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollViewObj.transform, false);
            RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;
            viewportObj.AddComponent<Image>();

            // 创建Content
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);

            // 使用VerticalLayoutGroup来组织多行
            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 15;
            layoutGroup.padding = new RectOffset(15, 15, 20, 50);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;

            ContentSizeFitter contentSizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            Debug.Log("ScrollView创建成功");

            // 创建日志ScrollView（放在右侧，占40%宽度）
            GameObject logScrollViewObj = new GameObject("LogScrollView");
            logScrollViewObj.transform.SetParent(canvasObj.transform, false);
            RectTransform logScrollViewRect = logScrollViewObj.AddComponent<RectTransform>();
            logScrollViewRect.anchorMin = new Vector2(0.6f, 0);
            logScrollViewRect.anchorMax = new Vector2(1, 1);
            logScrollViewRect.offsetMin = new Vector2(10, 20);
            logScrollViewRect.offsetMax = new Vector2(-20, -20);

            Image logScrollViewImage = logScrollViewObj.AddComponent<Image>();
            logScrollViewImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            ScrollRect logScrollRect = logScrollViewObj.AddComponent<ScrollRect>();
            logScrollRect.scrollSensitivity = 30;

            // 创建日志Viewport
            GameObject logViewportObj = new GameObject("Viewport");
            logViewportObj.transform.SetParent(logScrollViewObj.transform, false);
            RectTransform logViewportRect = logViewportObj.AddComponent<RectTransform>();
            logViewportRect.anchorMin = Vector2.zero;
            logViewportRect.anchorMax = Vector2.one;
            logViewportRect.sizeDelta = Vector2.zero;
            logViewportObj.AddComponent<Mask>().showMaskGraphic = false;
            logViewportObj.AddComponent<Image>();

            // 创建日志Content
            GameObject logContentObj = new GameObject("Content");
            logContentObj.transform.SetParent(logViewportObj.transform, false);
            RectTransform logContentRect = logContentObj.AddComponent<RectTransform>();
            logContentRect.anchorMin = new Vector2(0, 1);
            logContentRect.anchorMax = new Vector2(1, 1);
            logContentRect.pivot = new Vector2(0.5f, 1);
            logContentRect.sizeDelta = new Vector2(0, 100);  // 设置初始高度

            // 添加VerticalLayoutGroup来自动布局子元素
            VerticalLayoutGroup logLayoutGroup = logContentObj.AddComponent<VerticalLayoutGroup>();
            logLayoutGroup.childForceExpandWidth = true;
            logLayoutGroup.childForceExpandHeight = false;
            logLayoutGroup.childControlWidth = true;
            logLayoutGroup.childControlHeight = true;
            logLayoutGroup.padding = new RectOffset(0, 0, 0, 0);

            ContentSizeFitter logContentSizeFitter = logContentObj.AddComponent<ContentSizeFitter>();
            logContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Debug.Log("准备添加Text组件");
            // 创建日志Text
            GameObject logTextObj = new GameObject("LogText");
            logTextObj.transform.SetParent(logContentObj.transform, false);
            RectTransform logTextRect = logTextObj.AddComponent<RectTransform>();
            logTextRect.anchorMin = new Vector2(0, 0);
            logTextRect.anchorMax = new Vector2(1, 1);
            logTextRect.pivot = new Vector2(0.5f, 1);
            logTextRect.offsetMin = new Vector2(10, 10);
            logTextRect.offsetMax = new Vector2(-30, -10);  // 右侧留出滚动条空间

            Text logText = logTextObj.AddComponent<Text>();
            if (logText == null)
            {
                Debug.LogError("Text组件添加失败！");
                return;
            }
            
            Debug.Log("Text组件添加成功，设置属性");
            logText.fontSize = 20;
            logText.color = Color.white;
            logText.alignment = TextAnchor.UpperLeft;
            logText.horizontalOverflow = HorizontalWrapMode.Wrap;
            logText.verticalOverflow = VerticalWrapMode.Overflow;
            logText.text = "[ClickHandler] Log area";
            logText.supportRichText = false;

            // 添加ContentSizeFitter到Text，让它根据内容自动调整高度
            ContentSizeFitter logTextSizeFitter = logTextObj.AddComponent<ContentSizeFitter>();
            logTextSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // 配置ScrollRect
            logScrollRect.content = logContentRect;
            logScrollRect.viewport = logViewportRect;
            logScrollRect.horizontal = false;
            logScrollRect.vertical = true;
            logScrollRect.movementType = ScrollRect.MovementType.Elastic;
            logScrollRect.elasticity = 0.1f;
            logScrollRect.inertia = true;
            logScrollRect.decelerationRate = 0.135f;
            logScrollRect.scrollSensitivity = 30;
            logScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            logScrollRect.verticalScrollbarSpacing = -3;

            // 创建垂直滚动条
            GameObject scrollbarObj = new GameObject("Scrollbar Vertical");
            scrollbarObj.transform.SetParent(logScrollViewObj.transform, false);
            RectTransform scrollbarRect = scrollbarObj.AddComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = new Vector2(1, 1);
            scrollbarRect.pivot = new Vector2(1, 1);
            scrollbarRect.sizeDelta = new Vector2(20, 0);

            Image scrollbarBg = scrollbarObj.AddComponent<Image>();
            scrollbarBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

            Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            // 创建滚动条滑块区域
            GameObject slidingAreaObj = new GameObject("Sliding Area");
            slidingAreaObj.transform.SetParent(scrollbarObj.transform, false);
            RectTransform slidingAreaRect = slidingAreaObj.AddComponent<RectTransform>();
            slidingAreaRect.anchorMin = Vector2.zero;
            slidingAreaRect.anchorMax = Vector2.one;
            slidingAreaRect.sizeDelta = new Vector2(-20, -20);

            // 创建滚动条滑块
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(slidingAreaObj.transform, false);
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;

            // 将滚动条关联到ScrollRect
            logScrollRect.verticalScrollbar = scrollbar;

            Debug.Log("日志区域（带滚动条）创建成功");

            // 按钮分组定义：每个数组代表一行
            string[][] buttonGroups = new string[][]
            {
                // 第一部分：通用方法
                new string[] { "InitPush|初始化推送", "GetDeviceId|获取设备ID" },
                new string[] { "BindAccount|绑定账号", "UnbindAccount|解绑账号" },
                new string[] { "BindTag|绑定标签", "UnbindTag|解绑标签", "ListTags|标签列表" },
                new string[] { "AddAlias|添加别名", "RemoveAlias|移除别名", "ListAliases|别名列表" },
                new string[] { "ClearLog|清空日志" },
                
                // 第二部分：安卓方法
                new string[] { "InitAndroidThirdPush|初始化厂商通道" },
                new string[] { "BindPhoneNumber|绑定手机号", "UnbindPhoneNumber|解绑手机号" },
                new string[] { "IsNotificationEnabled|检查通知权限", "IsNotificationChannelEnabled|检查通知渠道", "JumpToNotificationSetting|跳转通知设置", "JumpToNotificationChannelSetting|跳转渠道设置" }
            };

            Debug.Log("开始创建按钮");

            // 创建4个独立的输入对话框
            GameObject accountDialog = CreateSingleInputDialog(canvasObj.transform, "AccountDialog", "绑定账号", "请输入账号");
            GameObject tagDialog = CreateSingleInputDialog(canvasObj.transform, "TagDialog", "标签", "请输入标签");
            GameObject aliasDialog = CreateSingleInputDialog(canvasObj.transform, "AliasDialog", "别名", "请输入别名");
            GameObject phoneDialog = CreateSingleInputDialog(canvasObj.transform, "PhoneDialog", "绑定手机号", "请输入手机号");
            
            // 添加ClickHandler组件（先添加，以便绑定事件）
            ClickHandler clickHandler = canvasObj.AddComponent<ClickHandler>();
            clickHandler.txt = logText;
            clickHandler.accountInput = accountDialog.transform.Find("Panel/InputField").GetComponent<InputField>();
            clickHandler.tagInput = tagDialog.transform.Find("Panel/InputField").GetComponent<InputField>();
            clickHandler.aliasInput = aliasDialog.transform.Find("Panel/InputField").GetComponent<InputField>();
            clickHandler.phoneInput = phoneDialog.transform.Find("Panel/InputField").GetComponent<InputField>();

            // 创建分组标题和按钮
            int buttonCount = 0;
            int groupIndex = 0;
            
            // 第一部分标题
            CreateSectionTitle(contentObj.transform, "通用方法");
            
            foreach (string[] buttonGroup in buttonGroups)
            {
                // 在第6组（索引5）前添加第二部分标题
                if (groupIndex == 5)
                {
                    CreateSectionTitle(contentObj.transform, "安卓方法");
                }
                
                // 创建一行按钮
                GameObject rowObj = CreateButtonRow(contentObj.transform, buttonGroup, clickHandler, ref buttonCount);
                groupIndex++;
            }

            Debug.Log("推送测试UI生成完成！共创建 " + buttonCount + " 个按钮");
            
            // 标记场景为已修改，确保事件绑定被保存
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            
            Selection.activeGameObject = canvasObj;
        }
        catch (System.Exception e)
        {
            Debug.LogError("生成UI时出错: " + e.Message + "\n" + e.StackTrace);
        }
    }

    static GameObject CreateSectionTitle(Transform parent, string titleText)
    {
        GameObject titleObj = new GameObject("SectionTitle_" + titleText);
        titleObj.transform.SetParent(parent, false);
        
        float totalWidth = 1920 * 0.6f - 30; // ScrollView宽度减去左右padding
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(totalWidth, 50);
        
        LayoutElement layoutElement = titleObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 50;
        layoutElement.minHeight = 50;
        layoutElement.preferredWidth = totalWidth;
        
        // 添加背景色以便更好地区分
        Image bgImage = titleObj.AddComponent<Image>();
        bgImage.color = new Color(0.9f, 0.9f, 0.95f, 1f);
        
        // 创建文本子对象
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(titleObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text titleTextComponent = textObj.AddComponent<Text>();
        titleTextComponent.text = titleText;
        titleTextComponent.fontSize = 32;
        titleTextComponent.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        titleTextComponent.alignment = TextAnchor.MiddleCenter;
        titleTextComponent.fontStyle = FontStyle.Bold;
        titleTextComponent.supportRichText = false;
        
        return titleObj;
    }

    static GameObject CreateButtonRow(Transform parent, string[] buttonInfos, ClickHandler clickHandler, ref int buttonCount)
    {
        GameObject rowObj = new GameObject("ButtonRow");
        rowObj.transform.SetParent(parent, false);
        
        RectTransform rowRect = rowObj.AddComponent<RectTransform>();
        
        // 计算每个按钮的宽度：总宽度减去左右padding和按钮间距，然后平分
        float totalWidth = 1920 * 0.6f - 30; // ScrollView宽度减去左右padding
        float spacing = 20;
        int numButtons = buttonInfos.Length;
        float buttonWidth = (totalWidth - spacing * (numButtons - 1)) / numButtons;
        
        rowRect.sizeDelta = new Vector2(totalWidth, 90);
        
        HorizontalLayoutGroup rowLayout = rowObj.AddComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = spacing;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = true;
        rowLayout.childControlWidth = true;  // 改为true，让布局控制宽度
        rowLayout.childControlHeight = true;
        
        LayoutElement rowLayoutElement = rowObj.AddComponent<LayoutElement>();
        rowLayoutElement.preferredHeight = 90;
        rowLayoutElement.minHeight = 90;
        rowLayoutElement.preferredWidth = totalWidth;
        
        foreach (string buttonInfo in buttonInfos)
        {
            string[] parts = buttonInfo.Split('|');
            string methodName = parts[0];
            string displayName = parts[1];
            
            GameObject buttonObj = CreateButton(rowObj.transform, displayName, methodName, clickHandler, buttonWidth);
            buttonCount++;
            Debug.Log("创建按钮 " + buttonCount + ": " + displayName);
        }
        
        return rowObj;
    }

    static GameObject CreateButton(Transform parent, string buttonText, string methodName, ClickHandler clickHandler, float buttonWidth)
    {
        GameObject buttonObj = new GameObject(buttonText);
        buttonObj.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        
        // 设置按钮的固定宽度
        LayoutElement buttonLayoutElement = buttonObj.AddComponent<LayoutElement>();
        buttonLayoutElement.preferredWidth = buttonWidth;
        buttonLayoutElement.minWidth = buttonWidth;

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.sprite = CreateRoundedRectSprite(200, 100, 20);
        buttonImage.type = Image.Type.Sliced;
        buttonImage.color = new Color(0.85f, 0.85f, 0.85f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.interactable = true;
        button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.highlightedColor = new Color(0.75f, 0.85f, 0.95f, 1f);
        colors.pressedColor = new Color(0.65f, 0.75f, 0.85f, 1f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        button.targetGraphic = buttonImage;

        // 使用UnityEditor方式绑定点击事件（这样会被序列化保存）
        UnityAction action = null;
        switch (methodName)
        {
            case "InitPush": action = clickHandler.InitPush; break;
            case "InitAndroidThirdPush": action = clickHandler.InitAndroidThirdPush; break;
            case "GetDeviceId": action = clickHandler.GetDeviceId; break;
            case "BindAccount": action = clickHandler.ShowInputDialogForAccount; break;
            case "UnbindAccount": action = clickHandler.UnbindAccount; break;
            case "BindTag": action = clickHandler.ShowInputDialogForTag; break;
            case "UnbindTag": action = clickHandler.ShowInputDialogForUnbindTag; break;
            case "ListTags": action = clickHandler.ListTags; break;
            case "AddAlias": action = clickHandler.ShowInputDialogForAddAlias; break;
            case "RemoveAlias": action = clickHandler.ShowInputDialogForRemoveAlias; break;
            case "ListAliases": action = clickHandler.ListAliases; break;
            case "BindPhoneNumber": action = clickHandler.ShowInputDialogForBindPhoneNumber; break;
            case "UnbindPhoneNumber": action = clickHandler.UnbindPhoneNumber; break;
            case "IsNotificationEnabled": action = clickHandler.IsNotificationEnabled; break;
            case "IsNotificationChannelEnabled": action = clickHandler.IsNotificationChannelEnabled; break;
            case "JumpToNotificationSetting": action = clickHandler.JumpToNotificationSetting; break;
            case "JumpToNotificationChannelSetting": action = clickHandler.JumpToNotificationChannelSetting; break;
            case "ClearLog": action = clickHandler.ClearLog; break;
        }
        
        if (action != null)
        {
            UnityEventTools.AddPersistentListener(button.onClick, action);
        }

        // 创建按钮文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = buttonText;
        text.fontSize = 22;
        text.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        text.alignment = TextAnchor.MiddleCenter;
        text.resizeTextForBestFit = false;
        text.fontStyle = FontStyle.Bold;
        text.supportRichText = false;

        return buttonObj;
    }

    static GameObject CreateSingleInputDialog(Transform parent, string dialogName, string title, string placeholder) {
        // 创建对话框背景
        GameObject dialogObj = new GameObject(dialogName);
        dialogObj.transform.SetParent(parent, false);
        
        RectTransform dialogRect = dialogObj.AddComponent<RectTransform>();
        dialogRect.anchorMin = Vector2.zero;
        dialogRect.anchorMax = Vector2.one;
        dialogRect.sizeDelta = Vector2.zero;
        
        Image dialogBg = dialogObj.AddComponent<Image>();
        dialogBg.color = new Color(0, 0, 0, 0.8f);
        
        // 创建面板
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(dialogObj.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(600, 300);
        
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        // 创建标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(20, 10);
        titleRect.offsetMax = new Vector2(-20, -10);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = title;
        titleText.fontSize = 32;
        titleText.color = Color.black;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        titleText.supportRichText = false;
        
        // 创建输入框
        GameObject inputObj = new GameObject("InputField");
        inputObj.transform.SetParent(panel.transform, false);
        
        RectTransform inputRect = inputObj.AddComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0, 0.4f);
        inputRect.anchorMax = new Vector2(1, 0.7f);
        inputRect.offsetMin = new Vector2(20, 10);
        inputRect.offsetMax = new Vector2(-20, -10);
        
        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = Color.white;
        
        InputField inputField = inputObj.AddComponent<InputField>();
        
        // 创建文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);
        
        Text text = textObj.AddComponent<Text>();
        text.fontSize = 28;
        text.color = Color.black;
        text.supportRichText = false;
        
        // 创建占位符
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(10, 5);
        placeholderRect.offsetMax = new Vector2(-10, -5);
        
        Text placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 28;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.fontStyle = FontStyle.Italic;
        placeholderText.supportRichText = false;
        
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;
        
        // 创建确定按钮
        GameObject okButton = new GameObject("OkButton");
        okButton.transform.SetParent(panel.transform, false);
        
        RectTransform okRect = okButton.AddComponent<RectTransform>();
        okRect.anchorMin = new Vector2(0, 0);
        okRect.anchorMax = new Vector2(0.45f, 0.3f);
        okRect.offsetMin = new Vector2(20, 20);
        okRect.offsetMax = new Vector2(-10, -20);
        
        Image okImage = okButton.AddComponent<Image>();
        okImage.color = new Color(0.3f, 0.7f, 0.3f, 1f);
        
        Button okBtn = okButton.AddComponent<Button>();
        okBtn.targetGraphic = okImage;
        
        GameObject okTextObj = new GameObject("Text");
        okTextObj.transform.SetParent(okButton.transform, false);
        RectTransform okTextRect = okTextObj.AddComponent<RectTransform>();
        okTextRect.anchorMin = Vector2.zero;
        okTextRect.anchorMax = Vector2.one;
        okTextRect.sizeDelta = Vector2.zero;
        
        Text okText = okTextObj.AddComponent<Text>();
        okText.text = "确定";
        okText.fontSize = 28;
        okText.color = Color.white;
        okText.alignment = TextAnchor.MiddleCenter;
        okText.fontStyle = FontStyle.Bold;
        
        // 创建取消按钮
        GameObject cancelButton = new GameObject("CancelButton");
        cancelButton.transform.SetParent(panel.transform, false);
        
        RectTransform cancelRect = cancelButton.AddComponent<RectTransform>();
        cancelRect.anchorMin = new Vector2(0.55f, 0);
        cancelRect.anchorMax = new Vector2(1, 0.3f);
        cancelRect.offsetMin = new Vector2(10, 20);
        cancelRect.offsetMax = new Vector2(-20, -20);
        
        Image cancelImage = cancelButton.AddComponent<Image>();
        cancelImage.color = new Color(0.7f, 0.3f, 0.3f, 1f);
        
        Button cancelBtn = cancelButton.AddComponent<Button>();
        cancelBtn.targetGraphic = cancelImage;
        
        GameObject cancelTextObj = new GameObject("Text");
        cancelTextObj.transform.SetParent(cancelButton.transform, false);
        RectTransform cancelTextRect = cancelTextObj.AddComponent<RectTransform>();
        cancelTextRect.anchorMin = Vector2.zero;
        cancelTextRect.anchorMax = Vector2.one;
        cancelTextRect.sizeDelta = Vector2.zero;
        
        Text cancelText = cancelTextObj.AddComponent<Text>();
        cancelText.text = "取消";
        cancelText.fontSize = 28;
        cancelText.color = Color.white;
        cancelText.alignment = TextAnchor.MiddleCenter;
        cancelText.fontStyle = FontStyle.Bold;
        
        dialogObj.SetActive(false);
        Debug.Log(dialogName + " 创建完成");
        
        return dialogObj;
    }
}
