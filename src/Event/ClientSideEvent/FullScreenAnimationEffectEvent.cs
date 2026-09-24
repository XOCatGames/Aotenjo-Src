namespace Aotenjo.ClientSideEvent
{
    public class FullScreenAnimationEffectEvent : PlayerEvent
    {
        public readonly FullScreenAnimationEffectType type;
        public FullScreenAnimationEffectEvent(Player player, FullScreenAnimationEffectType type) : base(player)
        {
        }
    }

    public enum FullScreenAnimationEffectType
    {
        TIMELESS
    }
}