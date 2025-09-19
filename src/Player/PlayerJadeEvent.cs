namespace Aotenjo
{
    public class PlayerJadeEvent : PlayerEvent
    {
        public IJade jade;
        public PlayerJadeEvent(Player player, IJade jade) : base(player)
        {
            this.jade = jade;
        }

        public class RetrieveEffectiveStack : PlayerJadeEvent
        {
            public int effectiveStack;

            public RetrieveEffectiveStack(Player player, IJade jade, int effectiveStack) : base(player, jade)
            {
                this.effectiveStack = effectiveStack;
            }
        }
    }
}