using System.Collections.Generic;

namespace Aotenjo
{
    public class BlockScoringEffectAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Player player;
        private readonly Block block;
        private readonly Permutation permutation;

        public BlockScoringEffectAppendEffect(Player player, Block block, Permutation permutation, Stack<IAnimationEffect> effectStack) : base(effectStack)
        {
            this.player = player;
            this.block = block;
            this.permutation = permutation;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            return new List<IAnimationEffect>()
            {
                new ArtifactOnBlockAppendEffect(effectStack, permutation, player, 0, block)
            };
        }
    }
}