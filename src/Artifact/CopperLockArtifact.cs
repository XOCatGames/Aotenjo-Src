using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class CopperLockArtifact : BaseLockArtifact
    {
        public CopperLockArtifact() : base("copper_lock", Rarity.COMMON)
        {
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(MUL_BASE, this));
        }
        
    }
}