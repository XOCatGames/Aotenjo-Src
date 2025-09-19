using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class SettleScoringEffectAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Permutation permutation;
        private readonly Player player;

        public SettleScoringEffectAppendEffect(Stack<IAnimationEffect> effectStack, Permutation permutation, Player player) : base(effectStack)
        {
            this.permutation = permutation;
            this.player = player;
        }

        public override List<IAnimationEffect> GetEffects()
        {
            return new List<IAnimationEffect>
            {
                //计分前计分效果
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerPreAddScoringEffectEvent(effects);
                    return effects;
                }),
                
                //手牌计分效果
                new SimpleAppendEffect(effectStack, () => permutation.ToTiles()
                    .OrderBy(t => player.TileSettlingOrder(t, permutation))
                    .Select(t => new TileScoringEffectAppendEffect(player, t, permutation, effectStack))
                    .ToList<IAnimationEffect>()),
                
                //幽魂木
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerPrePostAddOnTileAnimationEffect(effects);
                    return effects;
                }),
                
                //Boss效果
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<OnTileAnimationEffect> effects = new List<OnTileAnimationEffect>();
                    player.TriggerPostAddOnTileAnimationEffect(effects);
                    return effects.ConvertAll<IAnimationEffect>(e => e);
                }),
                
                //观赏计分效果
                new SimpleAppendEffect(effectStack, () => player.GetUnusedTilesInHand().Select(t => 
                        new TileUnusedScoringEffectAppendEffect(player, t, permutation, effectStack)).ToList<IAnimationEffect>()
                ),
                
                //面子计分效果
                new SimpleAppendEffect(effectStack, () => permutation.blocks
                    .OrderBy(block => player.TileSettlingOrder(block.tiles[0], permutation))
                    .Select(b => new BlockScoringEffectAppendEffect(player, b, permutation, effectStack))
                    .ToList<IAnimationEffect>()),
                
                //Post面子事件，目前只有不染
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerPostAddOnBlockAnimationEffect(effects);
                    return effects;
                }),
                
                //遗物计分效果
                new ArtifactOnSelfAppendEffect(effectStack, permutation, player, 0),
                
                //最后计分效果(现在只有古筝)
                new SimpleAppendEffect(effectStack, () => permutation.ToTiles()
                    .OrderBy(t => player.TileSettlingOrder(t, permutation))
                    .Select(t => 
                        new SimpleAppendEffect(effectStack, () => 
                            player.GetPostScoreEffectsFromTile(permutation, t)
                                .Select(e => e.OnTile(t))
                                .ToList<IAnimationEffect>()
                        )
                    ).ToList<IAnimationEffect>()),
                
                //Extra计分效果，目前用于花牌
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerAddExtraScoringEffects(effects);
                    return effects;
                }),
                //
                // //Post Extra计分效果事件，不能在此新增计分效果
                // new SimpleAppendEffect(effectStack, () =>
                // {
                //     List<IAnimationEffect> effects = new List<IAnimationEffect>();
                //     player.(effects);
                //     return effects;
                // }),
                
                //PostAddOnArtifactAnimation事件，多用于重复触发、统计数据和成就判断
                new SimpleAppendEffect(effectStack, () =>
                {
                    List<IAnimationEffect> effects = new List<IAnimationEffect>();
                    player.TriggerPostAddOnArtifactAnimationEffect(effects);
                    return effects;
                })
            };
        }
    }
}