using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Profiling;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class Permutation
    {
        public Block[] blocks;

        public Block.Jiang jiang;

        public Permutation(string representation)
        {
            string[] splitted1 = representation.Split('/');
            string[] splitted = splitted1[0].Split(' ');
            blocks = splitted.Select(b => new Block(b)).ToArray();
            jiang = new Block.Jiang(splitted1[1]);
        }

        public Permutation(Block[] array, Block.Jiang jiang)
        {
            blocks = array.OrderBy(b => b.tiles[0]).ToArray();
            this.jiang = jiang;
        }

        public virtual double GetFu()
        {
            return ToTiles().Select(t => t.GetBaseFu()).Sum();
        }

        public virtual double GetFan(Player player)
        {
            return GetFan(player, GetYakus(player));
        }
        
        public virtual double GetFan(Player player, List<YakuType> yakus)
        {
            return yakus
                .Sum(y => player.GetFanForYaku(y, this));
        }

        public virtual double GetNumericalScore(Player player)
        {
            return GetFu() * GetFan(player);
        }

        public virtual Score GetScore(Player player)
        {
            return new Score(this, player);
        }

        public virtual PermutationType GetPermType()
        {
            return PermutationType.NORMAL;
        }

        public virtual Block GetLastBlock()
        {
            return blocks[^1];
        }

        public virtual List<YakuType> GetYakus(Player player, bool allContainedYakus = false)
        {
            return GetYakus(player, _ => true, allContainedYakus);
        }

        private static ProfilerMarker marker = new ProfilerMarker("Permutation.GetYakus");

        public virtual List<YakuType> GetYakus(Player player, Predicate<YakuType> pred, bool allContainedYakus = false)
        {
            var yakus = new List<YakuType>();
            
            
            foreach (YakuType yaku in YakuTester.InfoMap.Keys)
            {
                if (!pred(yaku)) continue;
                if (YakuTester.TestYaku(this, yaku, player, out _) && player.deck.HasYakuType(yaku))
                {
                    yakus.Add(yaku);
                }
            }
            
            
            marker.End();

            if (allContainedYakus)
            {
                return yakus;
            }

            var toRemove = new HashSet<YakuType>();
            foreach (YakuType yaku in yakus)
            {
                YakuType[] childYakus = YakuTester.GetYakuChildsExcludeSelf(yaku).ToArray();
                foreach (var item in childYakus)
                {
                    toRemove.Add(item);
                }
            }

            foreach (var item in toRemove)
            {
                yakus.Remove(item);
            }

            return yakus;
        }


        public Tile.Category[] GetCategoriesIncluded()
        {
            HashSet<Tile.Category> set = new();
            foreach (var b in blocks)
            {
                set.Add(b.GetCategory());
            }

            set.Add(jiang.GetCategory());
            return set.ToArray();
        }

        public bool ContainsYaku(YakuType yaku, Player player)
        {
            return GetYakus(player).Contains(yaku);
        }

        public bool ContainsYakuRecursive(YakuType yaku, Player player)
        {
            return GetYakus(player).Any(a => YakuTester.IncludeYaku(a, yaku));
        }

        public virtual bool TilesFulfullAll(Predicate<Tile> tilePred)
        {
            return ToTiles().All(a => tilePred.Invoke(a));
        }

        public virtual bool BlocksFulfillAll(Predicate<Block> predicate)
        {
            return blocks.All(x => predicate.Invoke(x));
        }

        public virtual bool JiangFulfillAll(Predicate<Tile> predicate)
        {
            return jiang.All(predicate);
        }

        public virtual bool JiangFulfillAny(Predicate<Tile> predicate)
        {
            return jiang.Any(predicate);
        }

        public Tile.Category GetMostlyPlayedCategory()
        {
            return ToTiles()
                .Where(t => t.IsNumbered())
                .GroupBy(b => b.GetCategory())
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        public override String ToString()
        {
            StringBuilder sb = new("Permutation{");
            sb.Append("blocks=").Append(blocks == null ? "null" : blocks.ToString());
            sb.Append(", jiang=").Append(jiang);
            sb.Append('}');
            return sb.ToString();
        }

        public virtual Hand ToHand()
        {
            var list = new List<Tile>();
            foreach (var b in blocks)
            {
                list.AddRange(b.tiles);
            }

            list.Add(jiang.tile1);
            list.Add(jiang.tile2);
            return new Hand(list);
        }

        public virtual List<Tile> ToTiles()
        {
            var list = new List<Tile>();
            foreach (var b in blocks)
            {
                list.AddRange(b.tiles);
            }

            list.Add(jiang.tile1);
            list.Add(jiang.tile2);
            return list;
        }

        public virtual Permutation DeepClone()
        {
            Block[] newBlocks = new Block[blocks.Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                newBlocks[i] = new Block(blocks[i].tiles.Select(t => new Tile(t)).ToArray());
            }

            Block.Jiang newJiang = new Block.Jiang(new Tile(jiang.tile1), new Tile(jiang.tile2));
            return new Permutation(newBlocks, newJiang);
        }

        public virtual string ToFormat()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < blocks.Length; i++)
            {
                sb.Append(blocks[i].ToFormat());
                if (i != blocks.Length - 1)
                {
                    sb.Append(" ");
                }
            }

            sb.Append("/");
            sb.Append(jiang.ToFormat());
            return sb.ToString();
        }

        public virtual bool IsFullHand(Player player)
        {
            return blocks != null && blocks.Count() == player.GetMaxPlayingStage();
        }

        public Permutation Sort()
        {
            blocks = new Permutation(blocks, jiang).blocks;
            return this;
        }
    }

    [Serializable]
    public enum PermutationType
    {
        NORMAL,
        SEVEN_PAIRS,
        THIRTEEN_ORPHANS,
        LIGULIGU,
        EXTENDED_THIRTEEN_ORPHANS
    }
}