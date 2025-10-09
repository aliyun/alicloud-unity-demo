using Alicloud.Apm;
using UnityEngine;

namespace AlicloudApmDemo
{
    public class ApmDemoUI : MonoBehaviour
    {
        private const float MaxFontScale = 1f;
        private const int PanelPadding = 10;

        private readonly ApmDemoGame _game = new ApmDemoGame();
        private readonly ApmDemoActions _actions = new ApmDemoActions();

        private GUIStyle _tipsStyle;
        private Vector2 _scroll;
        private bool _panelCollapsed;
        private float _currentFontScale = 1f;
        private float _panelWidthGui = 420f;
        private string _status = string.Empty;
        private string _lastLog = string.Empty;

        private void Awake()
        {
            SetupLogEcho();
            _actions.Build();
            _game.EnsureScene();
            _status = Apm.IsStarted() ? "SDK 已启动" : "SDK 未启动";
        }

        private void OnDestroy()
        {
            TeardownLogEcho();
        }

        private void Update()
        {
            _game.Tick();
        }

        private void OnGUI()
        {
            var safe = Screen.safeArea;
            float scale = GetGuiScale(safe);
            var fontSnapshot = new GuiFontSnapshot(GUI.skin);
            _currentFontScale = ComputeFontScale(scale);
            ApplyFontScaling(GUI.skin, _currentFontScale);

            var origin = new Vector3(safe.x + PanelPadding, safe.y + PanelPadding, 0);
            var prevMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(origin, Quaternion.identity, new Vector3(scale, scale, 1));

            bool isPortrait = safe.height >= safe.width;
            float availableWidthGui = (safe.width / scale) - PanelPadding * 2;
            float scaleT =
                MaxFontScale > 1.0001f
                    ? Mathf.InverseLerp(1f, MaxFontScale, _currentFontScale)
                    : 0f;
            float widthBoost = Mathf.Lerp(0f, 40f, scaleT);
            float targetWidthGui = Mathf.Min(420f + widthBoost, availableWidthGui);
            if (!isPortrait)
            {
                float landscapeCap = Mathf.Max(320f, (safe.width * 0.45f) / scale);
                targetWidthGui = Mathf.Min(targetWidthGui, landscapeCap);
            }
            else
            {
                targetWidthGui = Mathf.Min(targetWidthGui, availableWidthGui * 0.92f);
            }

            float panelWidth = Mathf.Clamp(targetWidthGui, 320f, availableWidthGui);
            float panelHeight = (safe.height / scale) - PanelPadding * 2;
            if (isPortrait)
            {
                float heightCapRatio = Mathf.Lerp(
                    0.65f,
                    0.8f,
                    Mathf.InverseLerp(1f, 1.85f, _currentFontScale)
                );
                float maxHeightGui = (safe.height * heightCapRatio) / scale;
                panelHeight = Mathf.Min(panelHeight, maxHeightGui);
            }

            bool collapseActive = _panelCollapsed;
            float collapsedWidth = Mathf.Clamp(
                panelWidth * 0.55f,
                160f,
                Mathf.Max(200f, panelWidth * 0.75f)
            );
            float collapsedHeight = Mathf.Max(70f, GUI.skin.button.lineHeight * 3.2f);
            float panelDrawWidth = collapseActive ? collapsedWidth : panelWidth;
            float panelDrawHeight = collapseActive
                ? Mathf.Min(panelHeight, collapsedHeight)
                : panelHeight;

            _panelWidthGui = panelDrawWidth;

            var rect = new Rect(0, 0, panelDrawWidth, panelDrawHeight);
            GUILayout.BeginArea(rect, GUI.skin.window);

            if (collapseActive)
            {
                DrawCollapsedHandle(panelDrawWidth);
            }
            else
            {
                bool collapseNow = DrawGameHeader();
                if (!collapseNow)
                {
                    GUILayout.Space(6f);
                    _scroll = GUILayout.BeginScrollView(_scroll);
                    DrawGameScrollableContent();
                    GUILayout.EndScrollView();
                }
                else
                {
                    _panelCollapsed = true;
                    _scroll = Vector2.zero;
                }
            }

            GUILayout.EndArea();

            if (!string.IsNullOrEmpty(_status))
            {
                DrawStatusBar(safe, scale, panelDrawWidth, panelDrawHeight, isPortrait);
            }

            GUI.matrix = prevMatrix;

            var panelScreenRect = new Rect(
                safe.x + PanelPadding,
                safe.y + PanelPadding,
                panelDrawWidth * scale,
                panelDrawHeight * scale
            );
            _game.SetUiBlockRect(panelScreenRect);

            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    if (!panelScreenRect.Contains(t.position))
                        continue;
                    if (t.phase == TouchPhase.Moved)
                    {
                        var dx = t.deltaPosition.x / Mathf.Max(0.01f, scale);
                        var dy = t.deltaPosition.y / Mathf.Max(0.01f, scale);
                        _scroll.x = Mathf.Max(0, _scroll.x - dx);
                        _scroll.y = Mathf.Max(0, _scroll.y - dy);
                    }
                }
            }

            fontSnapshot.Restore(GUI.skin);
        }

        private bool DrawGameHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("游戏 + SDK测试", EditorTitleStyle());
            EnsureTipsStyle();
            GUILayout.Label("Tips: 使用方向键/WASD移动场景中的方块。", _tipsStyle);
            GUILayout.EndVertical();

            float buttonWidth = Mathf.Clamp(_panelWidthGui * 0.33f, 88f, 132f);
            bool collapseRequested = GUILayout.Button("隐藏面板", GUILayout.Width(buttonWidth));
            GUILayout.EndHorizontal();
            return collapseRequested;
        }

        private void DrawGameScrollableContent()
        {
            _actions.DrawGroups();
            GUILayout.Space(8f);
        }

        private void DrawCollapsedHandle(float panelWidthGui)
        {
            GUILayout.Space(6f);
            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
            };
            GUILayout.Label("控制面板已隐藏", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(4f);
            float buttonWidth = Mathf.Clamp(panelWidthGui - 20f, 90f, 160f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("展开面板", GUILayout.Width(buttonWidth)))
            {
                _panelCollapsed = false;
                _scroll = Vector2.zero;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawStatusBar(
            Rect safe,
            float scale,
            float panelDrawWidth,
            float panelDrawHeight,
            bool isPortrait
        )
        {
            const int pad = PanelPadding;
            string statusText = $"状态: {_status}";
            string logText = string.IsNullOrEmpty(_lastLog) ? null : $"最近日志: {_lastLog}";

            float barX = panelDrawWidth + pad * 2;
            float barAvailableWidth = Mathf.Max(0f, (safe.width / scale) - barX - pad);
            bool useSideBar = !isPortrait && barAvailableWidth >= 160f;
            float barWidth = useSideBar
                ? barAvailableWidth
                : Mathf.Min(panelDrawWidth, (safe.width / scale) - pad * 2);
            float barTop = useSideBar ? 0f : panelDrawHeight + pad * 1.5f;
            float barLeft = useSideBar ? barX : 0f;

            var boxStyle = GUI.skin.box;
            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
            };
            float contentWidth = Mathf.Max(32f, barWidth - boxStyle.padding.horizontal);
            float spacing = Mathf.Max(4f, labelStyle.lineHeight * 0.25f);
            float measuredHeight = labelStyle.CalcHeight(new GUIContent(statusText), contentWidth);
            if (!string.IsNullOrEmpty(logText))
            {
                measuredHeight +=
                    spacing + labelStyle.CalcHeight(new GUIContent(logText), contentWidth);
            }
            float minHeight = Mathf.Max(60f, GUI.skin.label.lineHeight * 2.4f);
            float barHeight = Mathf.Max(minHeight, measuredHeight + boxStyle.padding.vertical);
            var barRect = new Rect(barLeft, barTop, barWidth, barHeight);

            GUILayout.BeginArea(barRect, GUI.skin.box);
            GUILayout.Label(statusText, labelStyle);
            if (!string.IsNullOrEmpty(logText))
            {
                GUILayout.Space(spacing);
                GUILayout.Label(logText, labelStyle);
            }
            GUILayout.EndArea();
        }

        private void EnsureTipsStyle()
        {
            if (_tipsStyle == null)
            {
                _tipsStyle = new GUIStyle(GUI.skin.label)
                {
                    wordWrap = true,
                    alignment = TextAnchor.UpperLeft,
                };
            }
            _tipsStyle.fontSize = Mathf.Max(
                10,
                Mathf.RoundToInt(11f * Mathf.Clamp(_currentFontScale, 1f, 1.1f))
            );
        }

        private static float GetGuiScale(Rect safe)
        {
            float baseWidth = 1080f;
            float s = Mathf.Clamp(safe.width / baseWidth, 0.75f, 2.0f);
            if (Screen.dpi > 0)
            {
                float dpiScale = Mathf.Clamp(Screen.dpi / 160f, 0.75f, 2.0f);
                s = Mathf.Max(s, dpiScale * 0.9f);
            }
            return s;
        }

        private static float ComputeFontScale(float guiScale)
        {
            if (!Application.isMobilePlatform)
                return 1f;
            float dpiScale = Screen.dpi > 0 ? Mathf.Clamp(Screen.dpi / 160f, 1.1f, 1.8f) : 1f;
            float combined = Mathf.Max(guiScale * 1.0f, dpiScale * 0.9f);
            return Mathf.Clamp(combined, 1.0f, MaxFontScale);
        }

        private static void ApplyFontScaling(GUISkin skin, float fontScale)
        {
            if (fontScale <= 1.0f)
                return;
            float clamped = Mathf.Min(fontScale, MaxFontScale);
            skin.label.fontSize = Mathf.RoundToInt(12f * clamped);
            skin.button.fontSize = Mathf.RoundToInt(13f * clamped);
            skin.toggle.fontSize = Mathf.RoundToInt(13f * clamped);
            skin.textField.fontSize = Mathf.RoundToInt(13f * clamped);
            skin.textArea.fontSize = Mathf.RoundToInt(13f * clamped);
        }

        private GUIStyle EditorTitleStyle()
        {
            int size = Mathf.RoundToInt(18f * (_currentFontScale > 1f ? _currentFontScale : 1f));
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
            };
        }

        private void SetupLogEcho()
        {
#if UNITY_5 || UNITY_5_3_OR_NEWER
            Application.logMessageReceivedThreaded += OnAnyLog;
#else
            Application.RegisterLogCallbackThreaded(OnAnyLog);
#endif
        }

        private void TeardownLogEcho()
        {
#if UNITY_5 || UNITY_5_3_OR_NEWER
            Application.logMessageReceivedThreaded -= OnAnyLog;
#else
            Application.RegisterLogCallbackThreaded(null);
#endif
        }

        private void OnAnyLog(string condition, string stacktrace, LogType type)
        {
            _lastLog = $"{type}: {condition}";
            if (type == LogType.Exception)
            {
                _status = "捕获到异常日志";
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoSpawn()
        {
#if UNITY_2023_1_OR_NEWER
            if (Object.FindFirstObjectByType<ApmDemoUI>() != null)
                return;
#else
            if (Object.FindObjectOfType<ApmDemoUI>() != null)
                return;
#endif
            var go = new GameObject("ApmDemoUI");
            go.AddComponent<ApmDemoUI>();
            Object.DontDestroyOnLoad(go);
        }

        private readonly struct GuiFontSnapshot
        {
            private readonly int _label;
            private readonly int _button;
            private readonly int _toggle;
            private readonly int _textField;
            private readonly int _textArea;

            public GuiFontSnapshot(GUISkin skin)
            {
                _label = skin.label.fontSize;
                _button = skin.button.fontSize;
                _toggle = skin.toggle.fontSize;
                _textField = skin.textField.fontSize;
                _textArea = skin.textArea.fontSize;
            }

            public void Restore(GUISkin skin)
            {
                skin.label.fontSize = _label;
                skin.button.fontSize = _button;
                skin.toggle.fontSize = _toggle;
                skin.textField.fontSize = _textField;
                skin.textArea.fontSize = _textArea;
            }
        }
    }
}
