using System.Collections.Generic;

namespace Aotenjo
{
    public class TileRoundEndEffectAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Player player;
        private readonly Tile tile;
        private readonly Permutation permutation;

        public TileRoundEndEffectAppendEffect(Player player, Tile tile, Permutation permutation, Stack<IAnimationEffect> effectStack) : base(effectStack)
        {
            this.player = player;
            this.tile = tile;
            this.permutation = permutation;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            List<IAnimationEffect> lst = new  List<IAnimationEffect>();
            tile.AppendOnRoundEndEffects(player, permutation, lst);
            return lst;
        }
    }
}