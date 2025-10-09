#nullable enable

using System;

namespace Alicloud.Apm
{
    /// <summary>
    /// APM 配置选项
    /// </summary>
    public class ApmOptions : ICloneable
    {
        /// <summary>
        /// APM appKey
        /// </summary>
        public string AppKey { get; set; } = string.Empty;

        /// <summary>
        /// APM appSecret
        /// </summary>
        public string AppSecret { get; set; } = string.Empty;

        /// <summary>
        /// APM appRsaSecret
        /// </summary>
        public string AppRsaSecret { get; set; } = string.Empty;

        /// <summary>
        /// 标识 application 与 application extensions
        /// </summary>
        public string? AppGroupId { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; } = string.Empty;

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserNick { get; set; } = string.Empty;

        /// <summary>
        /// 用户 ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// SDK 组件类型列表
        /// </summary>
        public SDKComponents SdkComponents { get; set; } = SDKComponents.CrashAnalysis;

        public ApmOptions() { }

        public ApmOptions(string appKey, string appSecret, string appRsaSecret)
        {
            AppKey = appKey;
            AppSecret = appSecret;
            AppRsaSecret = appRsaSecret;
        }

        public ApmOptions(
            string appKey,
            string appSecret,
            string appRsaSecret,
            SDKComponents sdkComponents
        )
        {
            AppKey = appKey;
            AppSecret = appSecret;
            AppRsaSecret = appRsaSecret;
            SdkComponents = sdkComponents;
        }

        public object Clone()
        {
            return new ApmOptions
            {
                AppKey = this.AppKey,
                AppSecret = this.AppSecret,
                AppGroupId = this.AppGroupId,
                Channel = this.Channel,
                UserNick = this.UserNick,
                UserId = this.UserId,
                AppRsaSecret = this.AppRsaSecret,
                SdkComponents = this.SdkComponents,
            };
        }
    }

    [Flags]
    public enum SDKComponents
    {
        None = 0,
        CrashAnalysis = 1 << 0,
        All = CrashAnalysis,
    }
}
