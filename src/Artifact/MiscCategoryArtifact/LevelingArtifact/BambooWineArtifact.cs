using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BambooWineArtifact : LevelingArtifact, IPersistantAura
    {
        private const int INIT_LEVEL = 13;

        public BambooWineArtifact() : base("bamboo_wine", Rarity.EPIC, INIT_LEVEL)
        {
        }

        public override string Serialize()
        {
            return Level.ToString();
        }

        public override void Deserialize(string data)
        {
            Level = Int32.Parse(data);
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level);
        }

        public override bool IsUnique()
        {
            return true;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (tile.GetCategory() != Tile.Category.Suo || !player.Selecting(tile)) return;
            effects.Add(new ConsumeEffect(this, tile));
        }

        public bool IsAffecting(Player player)
        {
            return Level > 0;
        }

        private class ConsumeEffect : Effect
        {
            private readonly BambooWineArtifact artifact;

            private readonly Tile tile;

            public ConsumeEffect(BambooWineArtifact artifact, Tile tile)
            {
                this.artifact = artifact;
                this.tile = tile;
            }

            public override void Ingest(Player player)
            {
                if (artifact.Level > 0)
                {
                    artifact.Level--;
                    tile.addonFu += player.GetBaseFuOfTile(tile);
                }
            }

            public override string GetSoundEffectName()
            {
                return "Food";
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return artifact.Level <= 0 ? func("effect_bamboo_wine_drained") : func("effect_bamboo_wine_consume");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}