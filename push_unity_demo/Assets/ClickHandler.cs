using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aliyun.Push;

public class ClickHandler : MonoBehaviour {
    public Text txt;
    public InputField accountInput;
    public InputField tagInput;
    public InputField aliasInput;
    public InputField phoneInput;
    
    private string allLog;
    
    void Start() {   
        PushHelper.SetLogLevel(PushHelper.LOG_DEBUG);
        
        // 设置iOS回调（使用统一API）
#if UNITY_IOS
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
        PushHelper.SetIOSForegroundNotificationMode(PushHelper.IOS_FOREGROUND_SHOW_AND_CALLBACK);
#elif UNITY_ANDROID
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

#if UNITY_ANDROID
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
#endif
    }

    void Update() {
        txt.text = allLog;
    }

    public void IsNotificationEnabled() {
#if UNITY_ANDROID
        bool enabled = PushHelper.IsNotificationEnabled();
        Log("IsNotificationEnabled " + enabled);
#endif
    }

    public void IsNotificationChannelEnabled() {
 #if UNITY_ANDROID
        bool enabled = PushHelper.IsNotificationChannelEnabled("8.0up");
        Log("IsNotificationChannelEnabled " + enabled);
#endif
    }
    
    public void JumpToNotificationSetting() {
#if UNITY_ANDROID
        PushHelper.JumpToNotificationSetting();
        Log("JumpToNotificationSetting");
#endif
    }
    
    public void JumpToNotificationChannelSetting() {
#if UNITY_ANDROID
        PushHelper.JumpToNotificationChannelSetting("8.0up");
        Log("JumpToNotificationChannelSetting");
#endif
    }

    public void InitPush() {
        Log("InitPush");
        PushHelper.InitPush("您的阿里云AppKey", "您的阿里云AppSecret", (result, data) => {
            Log("InitPush result: " + result + " data: " + data);
        });
    }

    public void InitAndroidThirdPush() {
#if UNITY_ANDROID
        Log("InitAndroidThirdPush");
        PushHelper.InitAndroidThirdPush();
#endif
    }

    public void BindAccount(string account) {
        if (string.IsNullOrEmpty(account)) {
            Log("Account cannot be empty");
            return;
        }
        Log("BindAccount: " + account);
        PushHelper.BindAccount(account, (result, data) => Log("BindAccount " + account + " " + result + " " + data));
    }

    public void UnbindAccount() {
        Log("UnbindAccount");
        PushHelper.UnbindAccount((result, data) => Log("UnbindAccount " + result + " " + data));
    }

    public void BindTag(string tag) {
        if (string.IsNullOrEmpty(tag)) {
            Log("Tag cannot be empty");
            return;
        }
        Log("BindTag: " + tag);
        PushHelper.BindTag(1, tag, null, (result, data) => Log("BindTag device " + tag + " " + result + " " + data));
    }

    public void UnbindTag(string tag) {
        if (string.IsNullOrEmpty(tag)) {
            Log("Tag cannot be empty");
            return;
        }
        Log("UnbindTag: " + tag);
        PushHelper.UnbindTag(1, tag, null, (result, data) => Log("UnbindTag " + tag + " " + result + " " + data));
    }

    public void ListTags() {
        Log("ListTags");
        PushHelper.ListTags(1, (result, data) => Log("ListTags " + result + " " + data));
    }

    public void AddAlias(string alias) {
        if (string.IsNullOrEmpty(alias)) {
            Log("Alias cannot be empty");
            return;
        }
        Log("AddAlias: " + alias);
        PushHelper.AddAlias(alias, (result, data) => Log("AddAlias " + alias + " " + result + " " + data));
    }

    public void RemoveAlias(string alias) {
        if (string.IsNullOrEmpty(alias)) {
            Log("Alias cannot be empty");
            return;
        }
        Log("RemoveAlias: " + alias);
        PushHelper.RemoveAlias(alias, (result, data) => Log("RemoveAlias " + alias + " " + result + " " + data));
    }

    public void ListAliases() {
        Log("ListAliases");
        PushHelper.ListAliases((result, data) => Log("ListAliases " + result + " " + data));
    }

    public void BindPhoneNumber(string phone) {
#if UNITY_ANDROID
        if (string.IsNullOrEmpty(phone)) {
            Log("Phone number cannot be empty");
            return;
        }
        Log("BindPhoneNumber: " + phone);
        PushHelper.BindPhoneNumber(phone, (result, data) => Log("BindPhoneNumber " + phone + " " + result + " " + data));
#endif
    }

    public void UnbindPhoneNumber() {
#if UNITY_ANDROID
        Log("UnbindPhoneNumber");
        PushHelper.UnbindPhoneNumber((result, data) => Log("UnbindPhoneNumber " + result + " " + data));
#endif
    }

    public void GetDeviceId() {
        Log("Device ID: " + PushHelper.GetDeviceId());
    }

    public void ClearLog() {
        allLog = "[ClickHandler] Log cleared";
    }

    private void Log(string log) {
        string formattedLog = "[ClickHandler] " + log;
        Debug.Log(formattedLog);
        allLog = allLog + '\n' + formattedLog;
    }
    
    // 输入对话框相关方法
    public void ShowInputDialogForAccount() {
        ShowInputDialog(accountInput, (value) => BindAccount(value));
    }
    
    public void ShowInputDialogForTag() {
        ShowInputDialog(tagInput, (value) => BindTag(value));
    }
    
    public void ShowInputDialogForUnbindTag() {
        ShowInputDialog(tagInput, (value) => UnbindTag(value));
    }
    
    public void ShowInputDialogForAddAlias() {
        ShowInputDialog(aliasInput, (value) => AddAlias(value));
    }
    
    public void ShowInputDialogForRemoveAlias() {
        ShowInputDialog(aliasInput, (value) => RemoveAlias(value));
    }
    
    public void ShowInputDialogForBindPhoneNumber() {
        ShowInputDialog(phoneInput, (value) => BindPhoneNumber(value));
    }
    
    private void ShowInputDialog(InputField inputField, Action<string> callback) {
        if (inputField != null) {
            GameObject dialog = inputField.transform.parent.parent.gameObject;
            inputField.text = "";
            
            // 清除之前的监听器
            Button okButton = dialog.transform.Find("Panel/OkButton").GetComponent<Button>();
            Button cancelButton = dialog.transform.Find("Panel/CancelButton").GetComponent<Button>();
            
            okButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            
            // 添加新的监听器
            okButton.onClick.AddListener(() => {
                if (!string.IsNullOrEmpty(inputField.text)) {
                    callback(inputField.text);
                }
                dialog.SetActive(false);
            });
            
            cancelButton.onClick.AddListener(() => {
                dialog.SetActive(false);
            });
            
            // 显示对话框
            dialog.transform.SetAsLastSibling();
            dialog.SetActive(true);
            inputField.ActivateInputField();
        }
    }
}
