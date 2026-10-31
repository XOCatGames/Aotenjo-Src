using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialTaotie : TileMaterial
    {
        private const double FAN = 5f;
        private const double FAN_PER_LEVEL = 2f;

        [SerializeField] private int level;

        public TileMaterialTaotie(int ID) : base(ID, "taotie", null)
        {
        }

        private TileMaterialTaotie(int ID, int level) : base(ID, "taotie", null)
        {
            this.level = level;
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialTaotie(spriteID, level);
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), GetFan(), FAN_PER_LEVEL);
        }

        public double GetFan()
        {
            return FAN + level * FAN_PER_LEVEL;
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            if (player.IsPlayingTile(tile))
            {
                effects.Add(new FractureAndUpgradeEffect(this,
                    player.GetArtifacts().Any(a => a.GetRegName() == Artifacts.EssencePot.Id) ? player.GetRunArtifact(Artifacts.EssencePot.Id) : null, tile));
            }

            effects.Add(ScoreEffect.AddFan(FAN, null));
        }

        private class FractureAndUpgradeEffect : Effect
        {
            private TileMaterialTaotie tileMaterialTaotie;
            private Artifact artifact;
            private Tile taotieTile;

            public FractureAndUpgradeEffect(TileMaterialTaotie tileMaterialTaotie, Artifact artifact, Tile tile)
            {
                this.tileMaterialTaotie = tileMaterialTaotie;
                this.artifact = artifact;
                taotieTile = tile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_taotie_consume");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override string GetSoundEffectName()
            {
                return "Food";
            }

            public override void Ingest(Player player)
            {
                List<Tile> cands = player.GetPlayingTiles().Where(t => t != taotieTile).ToList();

                if (taotieTile.properties.material != tileMaterialTaotie) return;

                if (player.HasArtifact(Artifacts.SilverDogLeash.Id))
                {
                    cands = cands.Where(t => taotieTile.GetCategory() != t.GetCategory()).ToList();
                }
                else
                {
                    cands = cands.Where(t => taotieTile.GetCategory() == t.GetCategory()).ToList();
                }

                if (cands.Count == 0) return;

                Tile tile = cands[player.GenerateRandomInt(cands.Count)];
                tile.SetMask(TileMask.Fractured(), player);

                if (player.GetArtifacts().Any(a => a.GetRegName() == Artifacts.EssencePot.Id))
                {
                    taotieTile.AddFu(20, player);
                    tile.addonFu -= 20;
                }

                tileMaterialTaotie.level++;
            }
        }
    }
}