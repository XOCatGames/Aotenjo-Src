namespace Aotenjo
{
    public class PlatinumCoffinAchievement : Achievement
    {
        public PlatinumCoffinAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.OnEndRunEvent += OnLostGame;
        }

        private void OnLostGame(Player player, bool won)
        {
            if (!won || player.Level > 16)
            {
                if (player.GetMoney() >= 100)
                {
                    SetComplete();
                }
            }
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnEndRunEvent -= OnLostGame;
        }
    }
}