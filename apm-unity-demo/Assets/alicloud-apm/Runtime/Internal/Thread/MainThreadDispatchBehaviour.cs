using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Alicloud.Apm
{
    internal class MainThreadDispatchBehaviour
    {
        private const int QueueCapacity = 1024;

        private static readonly BlockingCollection<Action> _executionQueue =
            new BlockingCollection<Action>(QueueCapacity);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeLoop()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var newSystem = new PlayerLoopSystem { updateDelegate = OnUpdate };

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

        public static void Enqueue(Action action)
        {
            _executionQueue.Add(action);
        }
    }
}
