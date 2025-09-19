namespace Aotenjo
{
    public interface IAnimationEffect
    {
        public Effect GetEffect();

        public virtual void TriggerPostSyncEffect()
        {
            GetEffect().TriggerPostSyncEffect();
        }
    }
}