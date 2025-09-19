using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class MinerHatArtifact : Artifact
    {
        private static readonly int CHANCE = 8;

        public MinerHatArtifact() : base("miner_hat", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;
            if (tile.properties.material.GetRegName().Equals("plain_material"))
            {
                if (player.GenerateRandomDeterminationResult(CHANCE))
                {
                    effects.Add(new UpgradeEffect(this, tile));
                }
            }
        }

        private class UpgradeEffect : Effect
        {
            private readonly MinerHatArtifact artifact;
            private readonly Tile tile;

            public UpgradeEffect(MinerHatArtifact artifact, Tile tile)
            {
                this.artifact = artifact;
                this.tile = tile;
            }

            public override void Ingest(Player player)
            {
                tile.SetMaterial(TileMaterial.Ore(), player);
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_indicated_ore");
            }

            public override string GetSoundEffectName()
            {
                return "Ore";
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}