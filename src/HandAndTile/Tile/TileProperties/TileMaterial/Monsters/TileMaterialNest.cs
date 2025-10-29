using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialNest : TileMaterial
    {
        public TileMaterialNest(int ID) : base(ID, "nest", null)
        {
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialNest(spriteID);
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            if (player.Selecting(tile))
            {
                effects.Add(new SpawnEffect(this, tile));
            }
        }

        private class SpawnEffect : Effect
        {
            private TileMaterialNest tileMat;
            private Tile tile;

            public SpawnEffect(TileMaterialNest tileMaterialTaotie, Tile tile)
            {
                tileMat = tileMaterialTaotie;
                this.tile = tile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_nest_spawn");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                if (tile.properties.material != tileMat) return;
                tile.SetMask(TileMask.Fractured(), player, true);

                List<Tile> cands = player.GetSelectedTilesCopy().Where(t => t.CompatWithMaterial(PLAIN, player))
                    .ToList();
                if (cands.Count == 0) cands = player.GetSelectedTilesCopy();
                Tile newTile = cands[player.GenerateRandomInt(cands.Count)];
                newTile.SetMaterial(tileMat.DrawMaterial(player), player);
            }
        }

        private TileMaterial DrawMaterial(Player player)
        {
            if (player.GetArtifacts().Contains(Artifacts.MonsterBirthCertificate))
            {
                return ((MonsterBirthCertificateArtifact)Artifacts.MonsterBirthCertificate).monsterMaterial.Copy();
            }

            LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();

            pool.Add(Ghost(), 20);
            pool.Add(Succubus(), 20);
            pool.Add(GoldMouse(), 20);
            pool.Add(Taotie(), 20);
            pool.Add(Mo(), 3);
            pool.Add(Demon(), 4);

            return pool.Draw(player.GenerateRandomInt);
        }
    }
}