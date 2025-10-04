using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BranchOfYggdrasilArtifact : LevelingArtifact
    {
        private const int MAX_LEVEL = 60;
        private const int LEVEL_PER_DISCARD = 2;

        public BranchOfYggdrasilArtifact() : base("branch_of_yggdrasil", Rarity.EPIC, 0)
        {
            SetHighlightRequirement((t, p) => t.ContainsGreen(p));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), LEVEL_PER_DISCARD, MAX_LEVEL, Level);
        }

        public override void AddDiscardTileEffects(Player player, Tile tile,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AddDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            if (tile.ContainsGreen(player) && Level < MAX_LEVEL)
            {
                onDiscardTileEffects.Add(new ReduceTargetEffect(this).OnTile(tile));
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new SimpleEffect("effect_reset", this, _ => Level = 0));
        }

        private class ReduceTargetEffect : TextEffect
        {
            private BranchOfYggdrasilArtifact branchOfYggdrasilArtifact;

            public ReduceTargetEffect(BranchOfYggdrasilArtifact branchOfYggdrasilArtifact) : base(
                "effect_branch_of_yggdrasil", branchOfYggdrasilArtifact)
            {
                this.branchOfYggdrasilArtifact = branchOfYggdrasilArtifact;
            }

            public override void Ingest(Player player)
            {
                base.Ingest(player);
                if (branchOfYggdrasilArtifact.Level >= MAX_LEVEL) return;

                player.levelTarget /= (100f - branchOfYggdrasilArtifact.Level) / (100f);
                branchOfYggdrasilArtifact.Level += LEVEL_PER_DISCARD;
                player.levelTarget *= (100f - branchOfYggdrasilArtifact.Level) / (100f);

                float ratio = (100f - branchOfYggdrasilArtifact.Level) /
                              (100f - branchOfYggdrasilArtifact.Level + LEVEL_PER_DISCARD);

                EventManager.Instance.OnSetProgressBarLength(ratio);
            }
        }
    }
}