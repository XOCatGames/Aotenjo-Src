using System.Linq;

namespace Aotenjo
{
    public class GadgetCollectorAchievement : Achievement
    {
        public GadgetCollectorAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.PostRunEndEvent += PostRunEnd;
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.PostRunEndEvent -= PostRunEnd;
        }

        private void PostRunEnd(Player player, bool won, PlayerStats stats)
        {
            if (Gadgets.GadgetCompleteList().All(g => stats.GetGadgetObtainedCount(g) > 0))
                SetComplete();
        }
    }
}