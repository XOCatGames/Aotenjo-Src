using System;

namespace Aotenjo
{
    public abstract class Achievement
    {
        public readonly string id;
        [NonSerialized] private AttributeEventSubscriptions attributeSubscriptions;

        public Achievement(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// 默认自动扫描 [SubscribeToEvent] 方法，保留全局事件范围。
        /// 子类重写时必须调用 base.SubscribeToPlayer(player) 才会自动扫描。
        /// </summary>
        public virtual void SubscribeToPlayer(Player player)
        {
            (attributeSubscriptions ??= new AttributeEventSubscriptions()).Subscribe(this);
        }

        /// <summary>
        /// 默认取消自动注册的事件。子类重写时必须调用 base.UnsubscribeFromPlayer(player)。
        /// </summary>
        public virtual void UnsubscribeFromPlayer(Player player)
        {
            attributeSubscriptions?.Unsubscribe();
        }

        protected void SetComplete()
        {
            MessageManager.Instance.OnCompleteAchievement(id);
        }
    }
}
