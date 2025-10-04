using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MalachiteDaggerArtifact : CraftableArtifact
    {
        private const int FU = 40;

        public MalachiteDaggerArtifact() : base("malachite_dagger", Rarity.RARE)
        {
            SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.COPPER, player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.CompactWithMaterial(TileMaterial.COPPER, player))
            {
                effects.Add(ScoreEffect.AddFu(FU, this));
            }
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);

            foreach (Artifact a in player.GetArtifacts())
            {
                if (a.GetNameKey().Contains("copper") || a.GetNameKey().Contains("bronze") || a == Artifacts.WindVane ||
                    a == Artifacts.Censer)
                {
                    effects.Add(ScoreEffect.AddFu(FU, a));
                }
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.DetermineMaterialCompactbilityEvent += SuperCopper;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DetermineMaterialCompactbilityEvent -= SuperCopper;
        }

        private static void SuperCopper(PlayerDetermineMaterialCompactibilityEvent eventData)
        {
            Tile tile = eventData.tile;
            TileMaterial mat = eventData.mat;
            bool rawRes = eventData.res;
            if (rawRes) return;
            if (!tile.properties.material.GetRegName().Equals(TileMaterial.COPPER.GetRegName())) return;
            if (mat.GetRegName() == TileMaterial.PLAIN.GetRegName() || MaterialSet.Ore.GetMaterials().Any(t =>
                    t.GetRegName() != TileMaterial.Ore().GetRegName() && t.GetRegName().Equals(mat.GetRegName())))
            {
                eventData.res = true;
            }
        }
    }
}