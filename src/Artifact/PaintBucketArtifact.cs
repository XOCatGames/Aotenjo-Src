using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Aotenjo
{
    public class PaintBucketArtifact : LevelingArtifact, IMultiplierProvider
    {
        private const double MUL_PER_LEVEL = 2D;
        
        public PaintBucketArtifact() : base("paint_bucket", Rarity.EPIC, 0)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return Level == 0 ? 
                localizer($"artifact_{GetRegName()}_description_empty") : 
                string.Format(base.GetDescription(player, localizer), ((int) math.pow(MUL_PER_LEVEL, Level)));
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) => ToMulFanFormat(GetMul(player));
        

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);

            var tiles = player.GetAccumulatedPermutation()?.ToTiles();
            if (tiles != null && tiles.Any(t => t.IsNumbered()))
            {
                Tile.Category mostPlayedCategory = permutation.GetMostlyPlayedCategory();
                //获得万灵刷效果
                effects.Add(
                    new SimpleEffect(
                        "effect_paint_bucket", 
                        this, 
                        p => p.AddGadget(new MagicBrushGadget(mostPlayedCategory)),
                        "WaterSplash"
                    ));
            }
            
            if (Level == 0) return;
            //重置层数效果
            effects.Add(new SimpleEffect("effect_paint_bucket_reset", this, _ => this.Level = 0));
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            Level = 0;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (Level == 0) return;
            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
        }

        public double GetMul(Player player)
        {
            return math.pow(MUL_PER_LEVEL, Level);
        }

        public void ReceiveEmpower()
        {
            Level++;
        }
    }
}