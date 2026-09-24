namespace Aotenjo
{
    public class OnPalewoodAnimationEffect : IAnimationEffect
    {
        public readonly Tile tile;
        public readonly Effect effect;
        public readonly bool isClone;

        public OnPalewoodAnimationEffect(Tile tile, Effect effect, bool isClone = false)
        {
            this.tile = tile;
            this.effect = effect;
            this.isClone = isClone;
        }

        public OnPalewoodAnimationEffect Clone()
        {
            return new OnPalewoodAnimationEffect(tile, effect, true);
        }

        public Effect GetEffect()
        {
            return effect.GetEffect();
        }
    }
}
