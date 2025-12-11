package com.alibaba.sdk.android.pushwrapper;

import android.content.Context;

import com.alibaba.sdk.android.push.MessageReceiver;
import com.alibaba.sdk.android.push.notification.CPushMessage;

import java.util.Map;

public class PushMessageReceiver extends MessageReceiver {

    @Override
    protected void onNotification(Context context, String title, String summary, Map<String, String> extraMap) {
        PushWrapper.receiveNotification(title, summary, extraMap);
    }

    @Override
    protected void onMessage(Context context, CPushMessage message) {
        PushWrapper.receiveMessage(message.getTitle(), message.getContent());
    }

    @Override
    protected void onNotificationOpened(Context context, String title, String summary, String extraMap) {
        PushWrapper.receiveNotificationOpened(title, summary, extraMap);
    }

    @Override
    protected void onNotificationClickedWithNoAction(Context context, String title, String summary, String extraMap) {
        PushWrapper.receiveNotificationClickedWithNoAction(title, summary, extraMap);
    }

    @Override
    protected void onNotificationRemoved(Context context, String messageId) {
        PushWrapper.receiveNotificationRemoved(messageId);
    }

    @Override
    protected void onNotificationReceivedInApp(Context context, String title, String summary, Map<String, String> extraMap, 
            int openType, String openActivity, String openUrl) {
        PushWrapper.receiveNotificationReceivedInApp(title, summary, extraMap, openType, openActivity, openUrl);
    }
}
