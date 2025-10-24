using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BrushArtifact : Artifact
    {
        private static readonly int CHANCE = 9;

        public BrushArtifact() : base("brush", Rarity.COMMON)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.Selecting(tile)) return;

            if ((player.DetermineFontCompatibility(tile, TileFont.COLORLESS) ||
                 player.DetermineFontCompatibility(tile, TileFont.PLAIN)) &&
                player.GenerateRandomDeterminationResult(CHANCE))
            {
                effects.Add(new UpgradeEffect(this, tile));
            }
        }

        private class UpgradeEffect : TextEffect
        {
            public Tile tile;

            public UpgradeEffect(Artifact artifact, Tile tile) : base("effect_brushed", artifact)
            {
                this.tile = tile;
            }

            public override void Ingest(Player player)
            {
                LotteryPool<TileFont> fontPool = new LotteryPool<TileFont>();
                fontPool.Add(TileFont.BLUE, 1);
                fontPool.Add(TileFont.RED, 9);
                TileFont fontToBrush = fontPool.Draw(player.GenerateRandomInt);
                tile.SetFont(fontToBrush, player);
            }
        }
    }
}