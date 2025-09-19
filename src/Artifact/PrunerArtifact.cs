using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class PrunerArtifact : Artifact
    {
        private const int FU_DECREMENT = 2;
        private const int MONEY = 1;

        public PrunerArtifact() : base("pruner", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MONEY, FU_DECREMENT);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (permutation.JiangFulfillAny(t => t == tile))
            {
                effects.Add(new DecreaseFuEffect(tile, FU_DECREMENT, this));
            }
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(new EarnMoneyEffect(1, this));
        }

        private class DecreaseFuEffect : Effect
        {
            private Tile tile;
            private int fuDecrement;
            private PrunerArtifact prunerArtifact;

            public DecreaseFuEffect(Tile tile, int fU_DECREMENT, PrunerArtifact prunerArtifact)
            {
                this.tile = tile;
                fuDecrement = fU_DECREMENT;
                this.prunerArtifact = prunerArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return string.Format(func("effect_decrease_fu"), Math.Abs(fuDecrement));
            }

            public override Artifact GetEffectSource()
            {
                return prunerArtifact;
            }

            public override void Ingest(Player player)
            {
                tile.addonFu -= fuDecrement;
            }
        }
    }
}