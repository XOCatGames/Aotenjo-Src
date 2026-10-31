using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Aotenjo
{
    /// <summary>
    /// 管理单个对象通过 [SubscribeToEvent] 标记的方法所建立的事件订阅。
    /// 处理方法必须是返回 void 的实例方法，且只接收一个 PlayerEvent 类型的参数。
    /// 参数类型须与 EventBus 发布时使用的事件类型一致。
    /// </summary>
    public sealed class AttributeEventSubscriptions : IDisposable
    {
        private static readonly MethodInfo SubscribeHandlerMethod = typeof(AttributeEventSubscriptions)
            .GetMethod(nameof(SubscribeHandler), BindingFlags.NonPublic | BindingFlags.Static);

        private readonly List<Action> unsubscribeActions = new();

        /// <summary>
        /// 替换本组订阅。player 为 null 时订阅全局事件。
        /// EventBus.ClearAll() 后再次订阅，可恢复所有处理方法。
        /// throwOnFailure 为 true 时向调用方传播绑定异常，供运行遗物回滚已登记的订阅。
        /// </summary>
        public void Subscribe(object subscriber, Player player = null, bool throwOnFailure = false)
        {
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));

            Unsubscribe();
            foreach (MethodInfo method in GetHandlers(subscriber.GetType()))
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (method.IsStatic || method.ContainsGenericParameters || method.ReturnType != typeof(void) ||
                    parameters.Length != 1 || !typeof(PlayerEvent).IsAssignableFrom(parameters[0].ParameterType))
                {
                    Debug.LogError($"[AttributeEventSubscriptions] {method.DeclaringType?.Name}.{method.Name} " +
                        "must be a non-static, non-generic void method with exactly one PlayerEvent parameter.");
                    continue;
                }

                try
                {
                    var unsubscribe = (Action)SubscribeHandlerMethod.MakeGenericMethod(parameters[0].ParameterType)
                        .Invoke(null, new object[] { subscriber, method, player });
                    unsubscribeActions.Add(unsubscribe);
                }
                catch (Exception exception)
                {
                    Exception cause = exception is TargetInvocationException invocation && invocation.InnerException != null
                        ? invocation.InnerException : exception;
                    // 遗物激活失败必须交给玩家生命周期清理；其他调用方保留记录错误后继续的行为。
                    if (throwOnFailure)
                        throw new InvalidOperationException($"事件处理方法绑定失败：{method.DeclaringType?.Name}.{method.Name}", cause);
                    Debug.LogError($"[AttributeEventSubscriptions] Failed to subscribe " +
                        $"{method.DeclaringType?.Name}.{method.Name}: {cause}");
                }
            }
        }

        /// <summary>按注册时的玩家作用域和委托，仅取消本组订阅。</summary>
        public void Unsubscribe()
        {
            foreach (Action unsubscribe in unsubscribeActions) unsubscribe();
            unsubscribeActions.Clear();
        }

        public void Dispose() => Unsubscribe();

        private static IEnumerable<MethodInfo> GetHandlers(Type subscriberType)
        {
            var virtualMethods = new HashSet<MethodInfo>();
            for (Type type = subscriberType; type != null; type = type.BaseType)
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    // 同时扫描基类私有方法；虚方法只绑定继承链中最末级的重写实现。
                    if (method.IsVirtual && !virtualMethods.Add(method.GetBaseDefinition())) continue;
                    if (method.IsDefined(typeof(SubscribeToEventAttribute), inherit: true)) yield return method;
                }
            }
        }

        private static Action SubscribeHandler<T>(object subscriber, MethodInfo method, Player player)
            where T : PlayerEvent
        {
            var handler = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), subscriber, method);
            EventBus.Subscribe(player, handler);
            return () => EventBus.Unsubscribe(player, handler);
        }
    }
}
