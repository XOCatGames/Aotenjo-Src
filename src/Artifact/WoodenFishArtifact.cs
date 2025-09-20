using System;
using System.Collections.Generic;
using Aotenjo;

public class WoodenFishArtifact : Artifact
{
    public WoodenFishArtifact() : base("wooden_fish", Rarity.COMMON)
    {
    }

    public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
    {
        base.AddOnRoundEndEffects(player, permutation, effects);
        effects.Add(new WoodenFishEffect(this));
    }

    private class WoodenFishEffect : Effect
    {
        private readonly WoodenFishArtifact artifact;

        public WoodenFishEffect(WoodenFishArtifact artifact)
        {
            this.artifact = artifact;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_wooden_fish_upgrade_name");
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override string GetSoundEffectName()
        {
            return "WoodenFish";
        }

        public override void Ingest(Player player)
        {
            player.GetSkillSet().IncreaseLevel(FixedYakuType.Base);
        }
    }
}