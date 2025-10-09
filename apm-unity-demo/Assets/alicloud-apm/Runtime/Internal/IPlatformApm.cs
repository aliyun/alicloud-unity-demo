using System.Collections.Generic;

namespace Alicloud.Apm
{
    internal interface IPlatformApm
    {
        /// 指定选项启动 APM
        /// </summary>
        /// <param name="options">配置选项</param>
        public void Start(ApmOptions options);

        /// <summary>
        /// 更新 userNick
        /// </summary>
        /// <param name="userNick">用户昵称</param>
        public void SetUserNick(string userNick);

        /// <summary>
        /// 更新 userId
        /// </summary>
        /// <param name="userId">用户标识</param>
        public void SetUserId(string userId);

        /// <summary>
        /// 设置自定义维度
        /// </summary>
        /// <param name="key">维度</param>
        /// <param name="value">值</param>
        public void SetCustomKeyValue(string key, object value);

        /// <summary>
        /// 批量设置自定义维度
        /// </summary>
        /// <param name="keysAndValues">键值对</param>
        public void SetCustomKeysAndValues(IDictionary<string, object> keysAndValues);
    }
}
