using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class RecyclingBinArtifact : Artifact
    {
        public Tile dupTile;

        public RecyclingBinArtifact() : base("recycling_bin", Rarity.EPIC)
        {
            SetHighlightRequirement((t, _) => t.properties.material is TileMaterialWood);
        }

        protected override int GetSpriteID()
        {
            if (dupTile == null)
                return base.GetSpriteID();
            return dupTile.properties.material.GetRegName() switch
            {
                "nanmu_wood_material" => 270,
                "pale_wood_material" => 271,
                "mist_wood_material" => 272,
                "emerald_wood_material" => 273,
                "jacaranda_wood_material" => 274,
                "pao_rosa_wood_material" => 275,
                "hell_wood_material" => 276,
                _ => base.GetSpriteID()
            };
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            dupTile = null;
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            if (dupTile == null)
                return string.Format(base.GetDescription(localizer), localizer("ui_none"));
            return string.Format(base.GetDescription(localizer),
                localizer(dupTile.properties.material.GetLocalizeKey()));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new SimpleEffect("effect_reset", this, _ => dupTile = null));
        }

        public override void AddDiscardTileEffects(Player player, Tile tile,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            if (tile.properties.material is not TileMaterialWood) return;
            if (dupTile == null)
                onDiscardTileEffects.Add(new RecycleEffect(tile, this).OnTile(tile));
            else
            {
                onDiscardTileEffects.Add(new RetriggerEffect(this, dupTile));
                dupTile.AppendDiscardEffects(player, player.GetAccumulatedPermutation(), onDiscardTileEffects, false,
                    tile, false);
            }
        }

        private class RecycleEffect : TextEffect
        {
            private Tile tile;
            private readonly RecyclingBinArtifact recyclingBinArtifact;

            public RecycleEffect(Tile tile, RecyclingBinArtifact recyclingBinArtifact) : base("effect_recycling",
                recyclingBinArtifact)
            {
                this.tile = tile;
                this.recyclingBinArtifact = recyclingBinArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return string.Format(base.GetEffectDisplay(func), func(tile.properties.material.GetLocalizeKey()));
            }

            public override void Ingest(Player player)
            {
                base.Ingest(player);
                recyclingBinArtifact.dupTile = tile;
            }
        }

        private class RetriggerEffect : TextEffect
        {
            private Tile tile;

            public RetriggerEffect(Artifact artifact, Tile tile) : base("effect_retrigger_discard", artifact)
            {
                this.tile = tile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return string.Format(base.GetEffectDisplay(func), func(tile.properties.material.GetLocalizeKey()));
            }
        }
    }
}