using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class PiggyBankArtifact : LevelingArtifact
    {
        public PiggyBankArtifact() : base("piggy_bank", Rarity.COMMON, 0)
        {
        }

        public override int GetSellingPrice()
        {
            return base.GetSellingPrice() + Level;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!tile.CompactWithCategory(Tile.Category.Bing)) return;
            if (!player.Selecting(tile)) return;
            effects.Add(new PiggyBankEffect(this));
        }

        private class PiggyBankEffect : Effect
        {
            private readonly PiggyBankArtifact artifact;

            public PiggyBankEffect(PiggyBankArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override void Ingest(Player player)
            {
                artifact.Level++;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return string.Format(func("effect_piggy_deposit"), 1);
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override string GetSoundEffectName()
            {
                return "EarnMoney";
            }
        }
    }
}