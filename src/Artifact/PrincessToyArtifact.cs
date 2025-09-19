using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PrincessToyArtifact : Artifact, IActivable
    {
        private static readonly int CHANCE = 3;

        public PrincessToyArtifact() : base("princess_toy", Rarity.RARE)
        {
            SetHighlightRequirement((t, player) => !t.IsYaoJiu(player));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (permutation.blocks.Count() == 1 || permutation is SevenPairsPermutation ||
                permutation is ThirteenOrphansPermutation)
            {
                if (permutation.jiang.Contains(tile) && permutation.jiang.All(t => !t.IsYaoJiu(player)))
                {
                    if (player.GenerateRandomDeterminationResult(CHANCE))
                    {
                        effects.Add(new PrincessToyEffect(tile, this));
                    }
                }
            }
        }

        public bool IsActivating(Player player)
        {
            return player.GetAccumulatedPermutation() == null;
        }

        public bool IsActivating()
        {
            throw new ArgumentNullException();
        }

        private class PrincessToyEffect : Effect
        {
            public Tile tile;
            public Artifact artifact;

            public PrincessToyEffect(Tile tile, Artifact artifact)
            {
                this.tile = tile;
                this.artifact = artifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_copied");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                player.AddTileToPool(new Tile(tile));
            }
        }
    }
}