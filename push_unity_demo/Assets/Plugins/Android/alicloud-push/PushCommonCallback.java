package com.alibaba.sdk.android.pushwrapper;

import com.alibaba.sdk.android.logger.ILog;
import com.alibaba.sdk.android.push.CommonCallback;

public class PushCommonCallback implements CommonCallback {

	private final ILog mLogger = PushWrapperLog.getLogger(this);
	private final int mActionId;
	private final PushCallback mPushCallback;

	public PushCommonCallback(int actionId, PushCallback pushCallback) {
		this.mActionId = actionId;
		this.mPushCallback = pushCallback;
	}

	@Override
	public void onSuccess(String response) {
		mLogger.d("actionId " + mActionId + " success " + response);
		if (mPushCallback != null) {
			mPushCallback.callback(mActionId, true, response);
		}
	}

	@Override
	public void onFailed(String errorCode, String errorMessage) {
		mLogger.d("actionId " + mActionId + " fail " + errorCode + " " + errorMessage);
		if (mPushCallback != null) {
			mPushCallback.callback(mActionId, false, errorCode + " " + errorMessage);
		}
	}
}
