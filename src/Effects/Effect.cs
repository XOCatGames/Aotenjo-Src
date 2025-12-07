using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public abstract class Effect : IAnimationEffect
    {
        public List<EffectTag> tags = new List<EffectTag>();
        
        public abstract string GetEffectDisplay(Func<string, string> func);
        public abstract Artifact GetEffectSource();
        public abstract void Ingest(Player player);

        public virtual string GetEffectDisplay(Player player, Func<string, string> localizationMethod)
            => GetEffectDisplay(localizationMethod);

        public virtual bool NoDefaultSound() => false;

        public virtual string GetSoundEffectName() => "AddFu";

        public virtual Effect GetEffect() => this;

        public virtual bool WillTrigger() => true;
        public virtual bool ShouldWaitUntilFinished() => true;

        public OnTileAnimationEffect OnTile(Tile tile, bool isClone = false)
            => new(tile, this, isClone);

        public OnBlockAnimationEffect OnBlock(Block block, bool isClone = false)
            => new(block, this);

        public OnMultipleTileAnimationEffect OnMultipleTiles(List<Tile> tiles, Tile mainTile)
            => new(this, tiles, mainTile);

        public MaybeEffect MaybeTriggerWithChance(int chance, string locKey)
            => new(locKey, chance, this);
        
        public DerivedEffect AsDerivedEffect()
        {
            return new DerivedEffect(this);
        }
        
        public bool HasTag(EffectTag tag)
            => tags.Contains(tag);

        public OnBossAnimationEffect OnBoss()
            => new(this);

        public void TriggerPostSyncEffect()
        {
        }
    }
}