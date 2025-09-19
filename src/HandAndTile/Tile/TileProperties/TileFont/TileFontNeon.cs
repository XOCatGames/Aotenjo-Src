using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class TileFontNeon : TileFont
    {
        private const double FU = 10;

        public TileFontNeon() : base(3, "neon", ScoreEffect.AddFu(FU, null))
        {
        }

        public override void AppendToListRoundEndEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile)
        {
            if (tile.IsNumbered())
                effects.Add(new OnTileAnimationEffect(tile, new RealChangeSuitEffect(tile)));
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        private class RealChangeSuitEffect : Effect
        {
            private Tile tile;

            public RealChangeSuitEffect(Tile tile)
            {
                this.tile = tile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_change_suit");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                if (tile.IsNumbered())
                    tile.ModifyCategory(
                        (Tile.Category)(((int)tile.GetBaseCategory() + 1 + player.GenerateRandomInt(2)) % 3), player);
            }
        }
    }
}