using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class HufuArtifact : Artifact
    {
        private const double MUL = 1.5D;
        public HufuArtifact() : base("hufu", Rarity.EPIC)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (player is OraclePlayer oraclePlayer)
            {
                if (block.GetCategory() == oraclePlayer.oracleBlock.GetCategory())
                {
                    effects.Add(ScoreEffect.MulFan(MUL, this));
                }
            }
        }
    }
}