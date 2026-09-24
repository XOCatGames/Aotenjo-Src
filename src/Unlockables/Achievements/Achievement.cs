using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aotenjo
{
    public abstract class Achievement
    {
        public readonly string id;


        // 保存自动注册的delegate，保证取消订阅时使用同一个实例
        private readonly List<(Type eventType, Delegate handler)> autoSubscriptions = new();


        public Achievement(string id)
        {
            this.id = id;
        }


        /// <summary>
        /// 默认自动扫描 [SubscribeToEvent] 方法。
        /// 子类仍然可以 override 进行手动 Subscribe。
        /// </summary>
        public virtual void SubscribeToPlayer(Player player)
        {
            SubscribeAttributeEvents();
        }


        /// <summary>
        /// 默认取消自动扫描注册的事件。
        /// </summary>
        public virtual void UnsubscribeFromPlayer(Player player)
        {
            UnsubscribeAttributeEvents();
        }


        private void SubscribeAttributeEvents()
        {
            var methods = GetType().GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );


            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(
                    method,
                    typeof(SubscribeToEventAttribute)))
                {
                    continue;
                }


                var parameters = method.GetParameters();


                // Event handler 必须是 void Handler(EventType)
                if (parameters.Length != 1)
                {
                    UnityEngine.Debug.LogError(
                        $"[Achievement] {GetType().Name}.{method.Name} must have exactly one parameter."
                    );
                    continue;
                }


                var eventType = parameters[0].ParameterType;


                RegisterEvent(eventType, method);
            }
        }


        private void RegisterEvent(Type eventType, MethodInfo method)
        {
            // 防止重复订阅
            foreach (var subscription in autoSubscriptions)
            {
                if (subscription.eventType == eventType &&
                    subscription.handler.Method == method)
                {
                    return;
                }
            }


            var actionType = typeof(Action<>)
                .MakeGenericType(eventType);


            var handler = Delegate.CreateDelegate(
                actionType,
                this,
                method
            );


            var subscribeMethod = typeof(EventBus)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method =>
                    method.Name == nameof(EventBus.Subscribe) &&
                    method.IsGenericMethodDefinition &&
                    method.GetGenericArguments().Length == 1 &&
                    method.GetParameters().Length == 3)
                .MakeGenericMethod(eventType);


            subscribeMethod.Invoke(
                null,
                new object[]
                {
                    handler,
                    0,
                    false
                }
            );


            autoSubscriptions.Add(
                (eventType, handler)
            );
        }


        private void UnsubscribeAttributeEvents()
        {
            foreach (var subscription in autoSubscriptions)
            {
                var unsubscribeMethod = typeof(EventBus)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(method =>
                        method.Name == nameof(EventBus.Unsubscribe) &&
                        method.IsGenericMethodDefinition &&
                        method.GetGenericArguments().Length == 1 &&
                        method.GetParameters().Length == 1)
                    .MakeGenericMethod(subscription.eventType);


                unsubscribeMethod.Invoke(
                    null,
                    new object[]
                    {
                        subscription.handler
                    }
                );
            }


            autoSubscriptions.Clear();
        }


        protected void SetComplete()
        {
            MessageManager.Instance.OnCompleteAchievement(id);
        }
    }
}
