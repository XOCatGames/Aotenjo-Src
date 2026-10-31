using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class AgateDaggerArtifact : Artifact
    {
        private const float MUL_PER_AGATE = 0.1f;

        public AgateDaggerArtifact() : base("crystal_dagger", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, player) => tile.CompatWithMaterial(TileMaterial.Agate(), player));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), MUL_PER_AGATE,
                Utils.NumberToFormat(1f + AgateEffect.GetAgateCount(player) * MUL_PER_AGATE));
        }

        public override void AppendOnTileEffects(Player player, Permutation perm, Tile tile, List<Effect> lst)
        {
            base.AppendOnTileEffects(player, perm, tile, lst);
            if (tile.CompatWithMaterial(TileMaterial.Agate(), player))
            {
                // 显示与执行时都重新读取玛瑙数量，并沿用公共番符查询，避免独立结算绕过宿主词条。
                lst.Add(ScoreEffect.MulFan(() => 1f + MUL_PER_AGATE * AgateEffect.GetAgateCount(player), this));
            }
        }
    }
}
