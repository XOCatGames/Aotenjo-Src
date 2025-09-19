namespace Aotenjo
{
    public class FirstSparrowAchievement : Achievement
    {
        public FirstSparrowAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.OnEndRunEvent += OnWonGame;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnEndRunEvent -= OnWonGame;
        }

        private void OnWonGame(Player player, bool arg2)
        {
            if (arg2)
                SetComplete();
        }
    }
}