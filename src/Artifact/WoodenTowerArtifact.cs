using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class WoodenTowerArtifact : Artifact
    {
        public WoodenTowerArtifact() : base("wooden_tower", Rarity.RARE)
        {
            SetHighlightRequirement(ShouldTriggerEffect);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (ShouldTriggerEffect(tile, player))
            {
                effects.Add(new TextEffect("effect_wooden_tower"));
                effects.Add(new DiscardTileAppendEffect(player, player.playHandEffectStack, tile, false));
            }
        }

        private bool ShouldTriggerEffect(Tile t, Player _)
        {
            return t.GetCategory() == Tile.Category.Jian || (t.IsNumbered() && t.GetOrder() % 2 == 1);
        }
    }
}