using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aotenjo
{
    public class CollectorsAmuletArtifact : Artifact
    {
        private const float MULTIPLIER_PER_COMMON = 0.25f;
        private const float MULTIPLIER_PER_EPIC = 0.5f;

        public List<string> collectedMaterials;

        public CollectorsAmuletArtifact() : base("collectors_amulet", Rarity.EPIC)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            if (tile.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName()) return false;
            string matName = tile.properties.material.GetRegName();
            return collectedMaterials.All(c => !c.Equals(matName));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer),
                MULTIPLIER_PER_COMMON, MULTIPLIER_PER_EPIC, GetMult(player), GetMaterialNames());
        }

        public override string GetInShopDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(localizer("artifact_collectors_amulet_descrtiption_inshop"),
                MULTIPLIER_PER_COMMON, MULTIPLIER_PER_EPIC);
        }

        private string GetMaterialNames()
        {
            StringBuilder sb = new();

            foreach (string matName in collectedMaterials)
            {
                sb.Append(GameLocalizationManager.GetLocalizedText(TileMaterial.GetMaterial(matName).GetLocalizeKey()));
                sb.Append("\n");
            }

            if (collectedMaterials == null || collectedMaterials.Count == 0)
            {
                sb.Append(GameLocalizationManager.GetLocalizedText("ui_none"));
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            collectedMaterials = new List<string>();
        }

        public override string Serialize()
        {
            return base.Serialize() + "[COLLECTED]" + string.Join(",", collectedMaterials) + "[COLLECTED]";
        }

        public override void Deserialize(string data)
        {
            base.Deserialize(data);
            string[] parts = data.Split(new[] { "[COLLECTED]" }, StringSplitOptions.None);
            collectedMaterials = parts[1].Split(',').Where(n => !n.Equals("")).ToList();
        }

        private float GetMult(Player player)
        {
            float baseMult = 1f;
            return baseMult + collectedMaterials.Sum(mName => (
                    TileMaterial.GetMaterial(mName).GetRarity() == Rarity.COMMON)
                    ? MULTIPLIER_PER_COMMON
                    : MULTIPLIER_PER_EPIC
            );
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(() => GetMult(player), this));
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;
            if (tile.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName()) return;

            List<string> matNames = TileMaterial.Materials()
                .Where(m => m != TileMaterial.PLAIN && player.DetermineMaterialCompactbility(tile, m))
                .Select(m => m.GetRegName()).ToList();

            bool collected = false;
            foreach (string matName in matNames)
            {
                if (collectedMaterials.All(c => !c.Equals(matName)))
                {
                    collectedMaterials.Add(matName);
                    collected = true;
                }
            }

            if (collected)
            {
                effects.Add(new FractureEffect(tile, this));
            }
        }

        private class FractureEffect : Effect
        {
            private Tile tile;
            private Artifact artifact;

            public FractureEffect(Tile tile, Artifact artifact)
            {
                this.tile = tile;
                this.artifact = artifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_break");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                tile.SetMask(TileMask.Fractured(), player);
            }
        }
    }
}