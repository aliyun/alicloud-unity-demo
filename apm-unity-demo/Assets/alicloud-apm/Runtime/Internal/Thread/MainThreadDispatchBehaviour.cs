using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Alicloud.Apm
{
    internal class MainThreadDispatchBehaviour
    {
        private const int QueueCapacity = 4096;

        private static readonly BlockingCollection<Action> _executionQueue = new(QueueCapacity);
        private static int _droppedActionCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeLoop()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var newSystem = new PlayerLoopSystem
            {
                type = typeof(MainThreadDispatchBehaviour),
                updateDelegate = OnUpdate,
            };

            var systems = new List<PlayerLoopSystem>(playerLoop.subSystemList);
            systems.Insert(0, newSystem);
            playerLoop.subSystemList = systems.ToArray();
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static void OnUpdate()
        {
            while (_executionQueue.TryTake(out Action action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    ApmLogger.Error($"Error executing action: {e}");
                }
            }
        }

        public static bool Enqueue(Action action)
        {
            if (_executionQueue.TryAdd(action))
            {
                return true;
            }

            var dropped = Interlocked.Increment(ref _droppedActionCount);
            ApmLogger.Warning(
                $"Main thread dispatch queue overflowed; dropping action. Total dropped: {dropped}."
            );

            return false;
        }
    }
}
