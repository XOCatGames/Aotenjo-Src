using System;

namespace Aotenjo
{
    public class GoldenScytheArtifact : Artifact
    {
        private const int MONEY = 2;

        public GoldenScytheArtifact() : base("golden_scythe", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostRemoveTileEvent += DetermineBamboo;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostRemoveTileEvent -= DetermineBamboo;
        }

        public void DetermineBamboo(PlayerTileEvent evt)
        {
            if (evt.tile.ContainsGreen(evt.player))
            {
                evt.player.EarnMoney(MONEY);
                EventManager.Instance.OnArtifactEarnMoney(MONEY, this);
            }
        }
    }
}