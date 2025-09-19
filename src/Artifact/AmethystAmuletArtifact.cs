using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class AmethystAmuletArtifact : Artifact
    {
        public AmethystAmuletArtifact() : base("amethyst_amulet", Rarity.RARE)
        {
            SetHighlightRequirement((tile, _) =>
                tile.properties.mask.GetRegName() == TileMask.Corrupted().GetRegName());
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostAddSingleTileAnimationEffectEvent += Decorrupt;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostAddSingleTileAnimationEffectEvent -= Decorrupt;
        }

        private void Decorrupt(Permutation arg1, Player arg2, List<OnTileAnimationEffect> arg3, OnTileAnimationEffect arg4, Tile arg5)
        {
            if(arg4.effect is TileMaskCorrupted.CorruptedEffect cefff)
            {
                cefff.coefficient = 1f;
            }
        }
    }
}