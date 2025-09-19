using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class DiscardTileAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Tile tile;
        private readonly bool forced;
        private readonly Player player;

        public DiscardTileAppendEffect(Player player, Stack<IAnimationEffect> effectStack, Tile tile, bool forced) : base(effectStack)
        {
            this.tile = tile;
            this.forced = forced;
            this.player = player;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            return new List<IAnimationEffect>()
            {
                //强打
                new SimpleAppendEffect(effectStack,
                    () => forced
                        ? new List<IAnimationEffect>()
                            { new TextEffect("effect_force_discard", null, "DiscardForced").OnTile(tile) }
                        : new List<IAnimationEffect>()),
                
                //牌效果
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    tile.AppendDiscardEffects(player, player.GetAccumulatedPermutation(), effects, forced, tile, false);
                    return effects;
                }),
                
                //遗物效果
                new ArtifactOnTileDiscardAppendEffect(effectStack, player.GetAccumulatedPermutation(), player, tile, forced, 0),
                
                //弃牌效果事件
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerOnAddDiscardTileAnimationEffectEvent(effects, tile, forced);
                    return effects;
                })
            };
        }
    }
}