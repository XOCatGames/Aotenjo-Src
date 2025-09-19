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

    public override FlowerTile Copy()
    {
        FlowerTile flowerTile = base.Copy();
        ((BambooFlowerTile)flowerTile).level = level;
        return flowerTile;
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        return string.Format(base.GetFlowerDescription(loc), FU_BASE + level * FU_PER_LEVEL, FU_PER_LEVEL);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        if (used) return;
        effects.Add(new OnTileAnimationEffect(this, new TextEffect("effect_orchid_name")));
        effects.Add(new OnTileAnimationEffect(this, ScoreEffect.AddFu(FU_BASE + level * FU_PER_LEVEL, null)));
        effects.Add(new OnTileAnimationEffect(this, new UpgradeFlowerEffect(this)));
        used = true;
    }

    private class UpgradeFlowerEffect : Effect
    {
        private BambooFlowerTile tile;

        public UpgradeFlowerEffect(BambooFlowerTile tile)
        {
            this.tile = tile;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_chrys_upgrade");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            tile.level++;
        }
    }
}