using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialMistWood : TileMaterialWood
    {
        public TileMaterialMistWood(int ID) : base(ID, "mist_wood")
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer));
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            effects.Add(new MistWoodEffect(tile).OnTile(tile));
            if (withForce)
                effects.Add(new MistWoodForceEffect(tile).OnTile(tile));
        }

        public static TileMaterial DrawRandomWoodMaterial(Player player)
        {
            LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();
            List<TileMaterial> items = MaterialSet.Wood.GetMaterials()
                .Where(t => t.GetRegName() != MistWood().GetRegName()).ToList();
            pool.AddRange(items.Where(t => t.GetRarity() == Rarity.COMMON), 10);
            pool.AddRange(items.Where(t => t.GetRarity() == Rarity.RARE));
            return pool.Draw(player.GenerateRandomInt, false);
        }
    }

    internal class MistWoodEffect : TextEffect
    {
        private Tile tile;

        public MistWoodEffect(Tile tile) : base("effect_mistwood")
        {
            this.tile = tile;
        }

        public override void Ingest(Player player)
        {
            base.Ingest(player);
            tile.SetMaterial(TileMaterialMistWood.DrawRandomWoodMaterial(player), player);
        }
    }

    internal class MistWoodForceEffect : TextEffect
    {
        private Tile tile;

        public MistWoodForceEffect(Tile tile) : base("effect_mistwood_force")
        {
            this.tile = tile;
        }

        public override void Ingest(Player player)
        {
            base.Ingest(player);

            List<Tile> cands = player.GetHandDeckCopy().Where(t => t.GetCategory() == tile.GetCategory()).ToList();
            cands.Remove(tile);

            List<Tile> plains = cands.Where(t => t.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName())
                .ToList();
            cands.RemoveAll(t => plains.Contains(t));

            LotteryPool<Tile> plainPool = new LotteryPool<Tile>().AddRange(plains);
            LotteryPool<Tile> otherPool = new LotteryPool<Tile>().AddRange(cands);

            try
            {
                LotteryPool<Tile> pool = plainPool.IsEmpty() ? otherPool : plainPool;
                Tile t = pool.Draw(player.GenerateRandomInt, false);
                t.SetMaterial(TileMaterialMistWood.DrawRandomWoodMaterial(player), player);
            }
            catch (Exception)
            {
            }
        }
    }
}