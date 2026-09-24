using System;
using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 全局事件总线，支持泛型事件、优先级、一次性订阅及 Player 实例作用域。
    /// </summary>
    public static class EventBus
    {
        private class Subscriber
        {
            public Delegate Handler;
            public Action<object> Invoker;
            public Player Player;
            public int Priority;
            public bool Once;
        }

        private static readonly Dictionary<Type, List<Subscriber>> subscribers = new();

        public static void Subscribe<T>(Action<T> handler, int priority = 0, bool once = false)
            where T : PlayerEvent
        {
            Subscribe(null, handler, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<T> handler, int priority = 0, bool once = false)
            where T : PlayerEvent
        {
            if (handler == null) return;
            AddSubscriber<T>(player, handler, evt => handler((T)evt), priority, once);
        }

        private static void AddSubscriber<T>(Player player, Delegate handler, Action<object> invoker,
            int priority, bool once) where T : PlayerEvent
        {
            var type = typeof(T);
            if (!subscribers.TryGetValue(type, out var list))
            {
                list = new List<Subscriber>();
                subscribers[type] = list;
            }

            list.Add(new Subscriber
            {
                Handler = handler,
                Invoker = invoker,
                Player = player,
                Priority = priority,
                Once = once
            });
            list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public static void Subscribe<T>(Player player,
            Action<Permutation, Player, List<IAnimationEffect>> handler, int priority = 0, bool once = false)
            where T : PlayerEvents.PermutationAnimationEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.PermutationAnimationEvent)evt;
                handler(data.permutation, data.player, data.effects);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player,
            Action<Permutation, Player, List<OnTileAnimationEffect>> handler, int priority = 0, bool once = false)
            where T : PlayerEvents.OnPostAddOnTileAnimationEffectEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.OnPostAddOnTileAnimationEffectEvent)evt;
                handler(data.permutation, data.player, data.effects);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player,
            Action<Permutation, Player, List<OnTileAnimationEffect>, OnTileAnimationEffect, Tile> handler,
            int priority = 0, bool once = false)
            where T : PlayerEvents.PostAddSingleTileAnimationEffectEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.PostAddSingleTileAnimationEffectEvent)evt;
                handler(data.permutation, data.player, data.effects, data.effect, data.tile);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<Player, List<IAnimationEffect>, IAnimationEffect> handler,
            int priority = 0, bool once = false) where T : PlayerEvents.OnAddSingleAnimationEffectEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.OnAddSingleAnimationEffectEvent)evt;
                handler(data.player, data.effects, data.effect);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<Player, List<IAnimationEffect>, Tile, bool> handler,
            int priority = 0, bool once = false) where T : PlayerEvents.OnAddSingleDiscardTileAnimationEffectEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.OnAddSingleDiscardTileAnimationEffectEvent)evt;
                handler(data.player, data.effects, data.tile, data.forced);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<Permutation, Player, Effect> handler,
            int priority = 0, bool once = false) where T : PlayerEvents.PostIngestEffectEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.PostIngestEffectEvent)evt;
                handler(data.permutation, data.player, data.effect);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<Player, Gadget> handler,
            int priority = 0, bool once = false) where T : PlayerEvents.ObtainGadgetEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.ObtainGadgetEvent)evt;
                handler(data.player, data.gadget);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<Player, List<Destination>> handler,
            int priority = 0, bool once = false) where T : PlayerEvents.PostGenerateDestinationEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.PostGenerateDestinationEvent)evt;
                handler(data.player, data.destinations);
            }, priority, once);
        }

        public static void Subscribe<T>(Player player, Action<Player, Tile, TileMaterialDessert> handler,
            int priority = 0, bool once = false) where T : PlayerEvents.OnDessertTileConsumedEvent
        {
            AddSubscriber<T>(player, handler, evt =>
            {
                var data = (PlayerEvents.OnDessertTileConsumedEvent)evt;
                handler(data.player, data.tile, data.dessert);
            }, priority, once);
        }

        public static void SubscribeOnce<T>(Action<T> handler, int priority = 0) where T : PlayerEvent
        {
            Subscribe(handler, priority, true);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            UnsubscribeHandler<T>(null, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<T> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Permutation, Player, List<IAnimationEffect>> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player,
            Action<Permutation, Player, List<OnTileAnimationEffect>> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player,
            Action<Permutation, Player, List<OnTileAnimationEffect>, OnTileAnimationEffect, Tile> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Player, List<IAnimationEffect>, IAnimationEffect> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Player, List<IAnimationEffect>, Tile, bool> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Permutation, Player, Effect> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Player, Gadget> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Player, List<Destination>> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        public static void Unsubscribe<T>(Player player, Action<Player, Tile, TileMaterialDessert> handler)
        {
            UnsubscribeHandler<T>(player, handler);
        }

        private static void UnsubscribeHandler<T>(Player player, Delegate handler)
        {
            var type = typeof(T);
            if (subscribers.TryGetValue(type, out var list))
            {
                list.RemoveAll(s => ReferenceEquals(s.Player, player) && s.Handler.Equals(handler));
            }
        }

        public static void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (!subscribers.TryGetValue(type, out var list) || list.Count == 0)
                return;

            var snapshot = new List<Subscriber>(list);
            foreach (var sub in snapshot)
            {
                if (sub.Player != null &&
                    (evt is not PlayerEvent playerEvent || !ReferenceEquals(sub.Player, playerEvent.player)))
                    continue;

                try
                {
                    sub.Invoker(evt);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[EventBus] Exception in subscriber of {type.Name}: {ex}");
                }

                if (sub.Once)
                {
                    UnsubscribeHandler<T>(sub.Player, sub.Handler);
                }
            }
        }

        public static void ClearAll()
        {
            subscribers.Clear();
        }
    }
}
