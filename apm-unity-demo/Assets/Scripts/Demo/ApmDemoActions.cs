using System;
using System.Collections.Generic;
using System.Threading;
using Alicloud.Apm;
using Alicloud.Apm.CrashAnalysis;
using UnityEngine;

namespace AlicloudApmDemo
{
    public class ApmDemoActions
    {
        private readonly Dictionary<string, Action> _actions = new();
        private bool _foldLogs = true;
        private bool _foldExceptions = true;
        private bool _foldApm = true;

        public void Build()
        {
            _actions.Clear();

            _actions["LogError"] = () => Debug.LogError("[Demo] Error log test");
            _actions["LogException"] = () =>
                Debug.LogException(new NullReferenceException("[Demo] Exception log test"));

            _actions["Record Log"] = () =>
                CrashAnalysis.Log("[Demo] CrashAnalysis.Log custom message");
            _actions["Record Exception"] = () =>
            {
                try
                {
                    throw new InvalidOperationException("[Demo] Manually thrown exception");
                }
                catch (Exception ex)
                {
                    CrashAnalysis.RecordException(ex);
                }
            };

            _actions["Record ExceptionModel"] = () =>
            {
                var model = new ExceptionModel(
                    "LuaRuntimeError",
                    "attempt to index a nil value",
                    SourceLanguage.Lua
                );
                model.StackTrace.Add(StackFrame.FromSymbol("main", "script.lua", 15, "lua"));
                model.StackTrace.Add(StackFrame.FromSymbol("doSomething", "utils.lua", 8, "lua"));
                CrashAnalysis.RecordExceptionModel(model);
            };

            _actions["UnhandledException (Thread)"] = () =>
            {
                new Thread(() =>
                    throw new InvalidOperationException(
                        "[Demo] Unhandled InvalidOperationException from background thread"
                    )
                ).Start();
            };

            _actions["UnhandledException (Task)"] = () =>
            {
                System.Threading.Tasks.Task.Run(() =>
                    throw new Exception("[Demo] Unhandled exception from Task")
                );
                GC.Collect();
                GC.WaitForPendingFinalizers();
            };

            _actions["SetUserNick (随机)"] = () =>
            {
                var nick = $"Player_{DateTime.UtcNow:HHmmss}";
                Apm.SetUserNick(nick);
                Debug.Log($"[Demo] Updated UserNick to {nick}");
            };
            _actions["SetUserId (随机)"] = () =>
            {
                var uid = $"user_{UnityEngine.Random.Range(1000, 9999)}";
                Apm.SetUserId(uid);
                Debug.Log($"[Demo] Updated UserId to {uid}");
            };
            _actions["SetCustomKeyValue"] = () =>
            {
                Apm.SetCustomKeyValue("level", 1);
                Apm.SetCustomKeyValue("mode", "demo");
                Debug.Log("[Demo] Set custom key-values: level=1, mode=demo");
            };
            _actions["SetCustomKeysAndValues"] = () =>
            {
                var map = new Dictionary<string, object>
                {
                    { "hp", 100 },
                    { "scene", "field" },
                    { "hasBuff", true },
                };
                Apm.SetCustomKeysAndValues(map);
                Debug.Log("[Demo] Set multiple custom keys and values");
            };
        }

        public void DrawGroups()
        {
            DrawUnityLogGroup();
            DrawCrashAnalysisGroup();
            DrawCustomDimensionsGroup();
        }

        private void DrawUnityLogGroup()
        {
            _foldLogs = Foldout("Unity 日志", _foldLogs);
            if (_foldLogs)
            {
                Button("LogError");
                Button("LogException");
            }
        }

        private void DrawCrashAnalysisGroup()
        {
            _foldExceptions = Foldout("崩溃分析", _foldExceptions);
            if (_foldExceptions)
            {
                Button("Record Log");
                Button("Record Exception");
                Button("Record ExceptionModel");
                Button("UnhandledException (Thread)");
                Button("UnhandledException (Task)");
            }
        }

        private void DrawCustomDimensionsGroup()
        {
            _foldApm = Foldout("自定义维度", _foldApm);
            if (_foldApm)
            {
                Button("SetUserNick (随机)");
                Button("SetUserId (随机)");
                Button("SetCustomKeyValue");
                Button("SetCustomKeysAndValues");
            }
        }

        private void Button(string key)
        {
            if (!_actions.TryGetValue(key, out var action))
                return;
            if (GUILayout.Button(key))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Test action '{key}' threw: {ex.Message}");
                }
            }
        }

        private bool Foldout(string title, bool state)
        {
            var prev = state;
            GUILayout.BeginHorizontal(GUI.skin.box);
            float arrowWidth = Mathf.Lerp(
                18f,
                26f,
                Mathf.InverseLerp(16f, 24f, GUI.skin.button.fontSize)
            );
            if (GUILayout.Button(prev ? "▼" : "►", GUILayout.Width(arrowWidth)))
            {
                state = !state;
            }
            var label = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                wordWrap = false,
                clipping = TextClipping.Overflow,
            };
            GUILayout.Label(title, label, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            return state;
        }
    }
}
