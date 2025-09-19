using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class RoundEndEffectAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Player player;
        private readonly Permutation permutation;

        public RoundEndEffectAppendEffect(Stack<IAnimationEffect> effectStack, Player player, Permutation permutation) : base(effectStack)
        {
            this.player = player;
            this.permutation = permutation;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            return new List<IAnimationEffect>()
            {
                
                new ArtifactOnRoundEndAppendEffect(effectStack, permutation, player, 0),
                
                new SimpleAppendEffect(effectStack, () => permutation == null? new List<IAnimationEffect>() : permutation.ToTiles().Union(player.GetHandDeckCopy())
                    .OrderBy(t => player.TileSettlingOrder(t, permutation))
                    .Select(t => new TileRoundEndEffectAppendEffect(player, t, permutation, effectStack))
                    .ToList<IAnimationEffect>()),
                
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerOnAddRoundEndAnimationEffectEvent(effects);
                    return effects;
                })
            };
        }
    }
}