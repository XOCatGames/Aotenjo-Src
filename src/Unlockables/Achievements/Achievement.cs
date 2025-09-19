namespace Aotenjo
{
    public abstract class Achievement
    {
        public readonly string id;

        public Achievement(string id)
        {
            this.id = id;
        }

        public abstract void SubscribeToPlayer(Player player);
        public abstract void UnsubscribeFromPlayer(Player player);

        protected void SetComplete()
        {
            MessageManager.Instance.OnCompleteAchievement(id);
        }
    }
}