namespace Aotenjo.TileMaterialGlobalEffect
{
    public interface IPlayerEventSubscriber
    {
        public void SubscribeToPlayerEvents(Player player);
        public void UnsubscribeToPlayerEvents(Player player);
    }
}