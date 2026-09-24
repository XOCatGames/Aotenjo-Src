using System;
using System.Collections.Generic;
using Aotenjo;
using UnityEngine;

[Serializable]
public class BambooFlowerTile : OneTimeUseFlowerTile
{
    private const float FU_BASE = 20f;
    private const float FU_PER_LEVEL = 10f;

    [SerializeField] public int level;

    public BambooFlowerTile() : base(Category.JunZi, 4)
    {
    }

    public override FlowerTile CopyFlowerEffect()
    {
        FlowerTile flowerTile = base.CopyFlowerEffect();
        ((BambooFlowerTile)flowerTile).level = level;
        return flowerTile;
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), FU_BASE + level * FU_PER_LEVEL, FU_PER_LEVEL);
    }

    public override double GetBaseFu()
    {
        // Keep saved growth levels and other permanent tile bonuses without migrating save data.
        return FU_BASE + level * FU_PER_LEVEL + base.GetBaseFu();
    }

    public override void AppendPostScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm,
        Tile scoringTile)
    {
        base.AppendPostScoringEffect(effects, player, perm, scoringTile);
        if (used) return;
        effects.Add(new UpgradeFlowerEffect(this, scoringTile).OnTile(scoringTile));
        used = true;
    }

    private class UpgradeFlowerEffect : Effect
    {
        private readonly BambooFlowerTile tile;
        private readonly Tile scoringTile;

        public UpgradeFlowerEffect(BambooFlowerTile tile, Tile scoringTile)
        {
            this.tile = tile;
            this.scoringTile = scoringTile;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_grow_bamboo_segment");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            tile.level++;
            // Music copies the growth ability, not Bamboo's intrinsic base fu.
            if (!ReferenceEquals(scoringTile, tile)) scoringTile.addonFu += FU_PER_LEVEL;
        }

        public override string GetSoundEffectName() => "AddExtraFu";

        public override string GetEffectAnimationTrigger() => "fu_increase";
    }
}
