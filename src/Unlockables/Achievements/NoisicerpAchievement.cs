using System;

namespace Aotenjo
{
    public class NoisicerpAchievement : Achievement
    {
        public NoisicerpAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.OnEndRunEvent += OnLost;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.OnEndRunEvent -= OnLost;
        }

        private void OnLost(Player player, bool won)
        {
            if (!won || player.Level > 16)
            {
                double diff = Math.Floor(player.CurrentAccumulatedScore) - Math.Floor(player.GetLevelTarget());
                if (Math.Abs(diff / Math.Floor(player.GetLevelTarget())) <= 0.1f && diff < 0)
                {
                    SetComplete();
                }
            }
        }
    }
}