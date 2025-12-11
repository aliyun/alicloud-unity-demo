package com.alibaba.sdk.android.pushwrapper;

public interface PushCallback {
    void callback(int actionId, boolean success, String data);
}
