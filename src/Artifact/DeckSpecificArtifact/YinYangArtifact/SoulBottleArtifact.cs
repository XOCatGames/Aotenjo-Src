using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class SoulBottleArtifact : Artifact
    {
        private const int FU_PER_TILE = 1;
        private const string STAT_KEY = "tile_destoryed";

        public SoulBottleArtifact() : base("soul_bottle", Rarity.RARE)
        {
        }

        public override string GetDescription(Player p, Func<string, string> loc)
        {
            int cnt = p?.stats.GetCustomStats(STAT_KEY) ?? 0;
            return string.Format(base.GetDescription(loc), cnt * FU_PER_TILE);
        }

        public override void AddOnBlockEffects(Player player, Permutation perm, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, perm, block, effects);

            if (!block.IsYinSeq()) return;

            int cnt = player.stats.GetCustomStats(STAT_KEY);
            if (cnt > 0) effects.Add(ScoreEffect.AddFu(cnt * FU_PER_TILE, this));
        }
    }
}