using System.Threading;
using UnityEngine;

namespace Alicloud.Apm
{
    internal static class MainThreadHelper
    {
        private static int mainThreadId;

        public static int MainThreadId
        {
            get
            {
                if (mainThreadId == 0)
                {
                    mainThreadId = Thread.CurrentThread.ManagedThreadId;
                }
                return mainThreadId;
            }
        }

        // 提供一个静态方法在游戏启动时强制初始化
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// 检查当前代码是否运行在主线程上。
        /// </summary>
        public static bool IsMainThread
        {
            get { return Thread.CurrentThread.ManagedThreadId == MainThreadId; }
        }
    }
}
