using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class LuckySevenArtifact : LevelingArtifact
    {
        private static readonly int CHANCE = 7;

        public LuckySevenArtifact() : base("lucky_seven", Rarity.COMMON, 0)
        {
            SetHighlightRequirement((t, _) => t.GetOrder() == 7 && t.IsNumbered());
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level, GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;
            if (tile.IsNumbered() && tile.GetOrder() == 7 && player.GenerateRandomDeterminationResult(CHANCE))
            {
                effects.Add(new LuckyEffect(this, tile));
            }
        }

        public class LuckyEffect : EarnMoneyEffect
        {
            private readonly Tile tile;

            public LuckyEffect(Artifact artifact, Tile tile) : base(7, artifact)
            {
                this.tile = tile;
            }

            public override void Ingest(Player player)
            {
                base.Ingest(player);
                if(artifact is LuckySevenArtifact luckySevenArtifact)
                    luckySevenArtifact.Level++;
                tile.addonFu += 17;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_lucky");
            }
        }
    }
}