using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BicolorSilkArtifact : Artifact
    {
        private const double MUL = 2.0D;
        
        public BicolorSilkArtifact() : base("bicolor_silk", Rarity.RARE)
        {
        }
        
        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL);
        }
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (player is OraclePlayer oraclePlayer)
            {
                var oracleBlock = oraclePlayer.oracleBlock;
                if (permutation.blocks
                    .Where(b => b != oracleBlock)
                    .All(b => b.GetCategory() != oracleBlock.GetCategory()))
                {
                    effects.Add(ScoreEffect.MulFan(MUL, this));
                }
            }
        }
    }
}