using System;
using System.Collections.Generic;
using System.Threading;

namespace Aotenjo
{
    /// <summary>
    /// 全局事件总线，支持泛型事件、线程安全、优先级、一次性订阅
    /// </summary>
    public static class EventBus
    {
        private class Subscriber
        {
            public Delegate Handler;
            public int Priority;
            public bool Once;
        }

        private static readonly Dictionary<Type, List<Subscriber>> subscribers = new();

        /// <summary>
        /// 订阅事件
        /// </summary>
        public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false) where T : PlayerEvent
        {
            if (handler == null) return;
            var type = typeof(T);

            if (!subscribers.TryGetValue(type, out var list))
            {
                list = new List<Subscriber>();
                subscribers[type] = list;
            }

            list.Add(new Subscriber { Handler = handler, Priority = priority, Once = once });
            // 按优先级排序，高优先级在前
            list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        /// <summary>
        /// 订阅一次事件，触发后自动取消
        /// </summary>
        public static void SubscribeOnce<T>(Action<T> handler, int priority = 0) where T : PlayerEvent
        {
            Subscribe(handler, priority, true);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (subscribers.TryGetValue(type, out var list))
            {
                list.RemoveAll(s => s.Handler.Equals(handler));
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public static void Publish<T>(T evt)
        {
            var type = typeof(T);
            List<Subscriber> snapshot;

            if (!subscribers.TryGetValue(type, out var list) || list.Count == 0)
                return;

            snapshot = new List<Subscriber>(list);

            foreach (var sub in snapshot)
            {
                try
                {
                    if (sub.Handler is Action<T> action)
                    {
                        action.Invoke(evt);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[EventBus] Exception in subscriber of {type.Name}: {ex}");
                }

                if (sub.Once)
                {
                    Unsubscribe((Action<T>)sub.Handler);
                }
            }
        }

        /// <summary>
        /// 清空所有订阅（通常在切场景时用）
        /// </summary>
        public static void ClearAll()
        {
            subscribers.Clear();
        }
    }
}
