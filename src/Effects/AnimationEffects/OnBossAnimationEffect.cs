using Aotenjo;

namespace Aotenjo
{
    public class OnBossAnimationEffect : IAnimationEffect
    {
        private Effect effect;

        public OnBossAnimationEffect(Effect effect)
        {
            this.effect = effect;
        }

        public Effect GetEffect()
        {
            return effect;
        }
    }
}