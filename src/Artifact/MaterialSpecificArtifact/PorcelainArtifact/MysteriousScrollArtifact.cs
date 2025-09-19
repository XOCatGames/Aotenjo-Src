using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MysteriousScrollArtifact : Artifact
    {
        private static readonly int CHANCE = 4;

        public MysteriousScrollArtifact() : base("mysterious_scroll", Rarity.COMMON)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);

            foreach (var tile in player.GetUnusedTilesInHand().Where(t => t.IsYaoJiu(player)))
            {
                effects.Add(
                    new TransformMaterialEffect(TileMaterial.MysteriousColorPorcelain(), this, tile, "")
                        .MaybeTriggerWithChance(CHANCE, "mysterious_scroll")
                        .OnTile(tile)
                    );
            }
        }
    }
}