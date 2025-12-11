package com.alibaba.sdk.android.pushwrapper;

import com.alibaba.sdk.android.logger.BaseSdkLogApi;
import com.alibaba.sdk.android.logger.ILog;
import com.alibaba.sdk.android.logger.ILogger;
import com.alibaba.sdk.android.logger.LogLevel;

/**
 * SDK日志接口
 */
public class PushWrapperLog {

    private static class Holder {
        private static final BaseSdkLogApi instance = new BaseSdkLogApi("PushWrapper", true);
    }

    private PushWrapperLog() {
    }

    /**
     * 是否输出日志
     *
     * @param enableLog false关闭，true打开，默认打开
     */
    public static void enable(boolean enableLog) {
        Holder.instance.enable(enableLog);
    }

    /**
     * 设置输出的日志级别
     *
     * @param level
     */
    public static void setLevel(LogLevel level) {
        Holder.instance.setLevel(level);
    }

    /**
     * 设置日志输出接口，默认输出到logcat
     * 调用之后会替换前面的日志
     *
     * @param logger
     */
    public static void setILogger(ILogger logger) {
        Holder.instance.setILogger(logger);
    }

    /**
     * 添加日志输出接口，与{@link #setILogger(ILogger)}不同，add的接口，需要实现自己控制日志是否输出和输出的日志级别
     * 不会影响{@link #setILogger(ILogger)}
     *
     * @param logger
     */
    public static void addILogger(ILogger logger) {
        Holder.instance.addILogger(logger);
    }

    /**
     * 移除日志输出接口, 与{@link #addILogger(ILogger)}操作相对
     *
     * @param logger
     */
    public static void removeILogger(ILogger logger) {
        Holder.instance.removeILogger(logger);
    }

    /**
     * 基于日志使用类，创建接口
     *
     * @param instanceObject
     * @return
     */
    public static ILog getLogger(Object instanceObject) {
        return Holder.instance.getLogger(instanceObject);
    }
}
