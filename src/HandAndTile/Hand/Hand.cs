using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aotenjo
{
    public class Hand
    {
        public List<Tile> tiles;

        public Hand(List<Tile> tiles)
        {
            this.tiles = tiles;
            this.tiles.Sort();
        }

        public Hand(String representation)
        {
            tiles = new List<Tile>();
            if (representation == null)
            {
                tiles = new List<Tile>();
                return;
            }

            char[] arr = representation.ToCharArray();
            List<int> ordList = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if ('1' <= arr[i] && arr[i] <= '9')
                {
                    ordList.Add(arr[i] - '0');
                }
                else
                {
                    var query = ordList.Select(ord =>
                    {
                        Tile.Category category = Tile.GetCategoryFromChar(arr[i], ord);
                        if (category <= Tile.Category.Jian)
                            return new Tile(category, ord);
                        return FlowerTile.FromCategoryAndOrder(category, ord);
                    });
                    ordList = new List<int>();
                    tiles.AddRange(query);
                }
            }
        }

        public virtual List<Permutation> GetPerms(Player player, int mode, Block.Jiang jiang = null)
        {
            if (jiang == null && tiles.Count(t => t.CompatWith(tiles[0])) >= 5)
            {
                jiang = new Block.Jiang(tiles[0], tiles.First(t => t != tiles[0] && t.CompatWith(tiles[0])));
            }

            return GetPerms(new List<Block>(), player, mode, jiang);
        }

        public virtual List<Permutation> GetPerms(List<Block> confirmedBlocks, Player player, int mode,
            Block.Jiang jiang = null)
        {
            List<Permutation> resPerms = new();
            if (jiang == null && tiles.Count(t => t.CompatWith(tiles[0])) >= 5)
            {
                jiang = new Block.Jiang(tiles[0], tiles.First(t => t != tiles[0] && t.CompatWith(tiles[0])));
            }

            switch (mode)
            {
                case 0:
                    FindPermutation(new List<Tile>(
                            (jiang == null) ? tiles : tiles.Where(t => t != jiang.tile1 && t != jiang.tile2)),
                        resPerms,
                        confirmedBlocks,
                        jiang,
                        player
                    );
                    break;
                case 1:
                    resPerms.AddRange(GetSevenPairsPerms(jiang, player));
                    break;
                case 2:
                    resPerms.AddRange(GetThirteenOrphansPerms(player));
                    break;
            }

            return resPerms;
        }

        public virtual List<Permutation> GetSevenPairsPerms(Block.Jiang jiang, Player player)
        {
            List<Permutation> perms = new List<Permutation>();
            List<Tile> tiles = new List<Tile>(this.tiles);
            Block[] blocks = new Block[6];

            try
            {
                if (jiang != null)
                {
                    if (!player.GetCombinator().IsIdenticalFormingBlock(jiang.tile1, jiang.tile2)) return new List<Permutation>();
                    tiles.Remove(jiang.tile1);
                    tiles.Remove(jiang.tile2);
                    for (int i = 0; i < 6; i++)
                    {
                        Tile t = tiles[0];
                        Tile n = tiles.Find(x => x != t && player.GetCombinator().IsIdenticalFormingBlock(x, t));
                        blocks[i] = new PairBlock(new[] { t, n });
                        tiles.Remove(t);
                        tiles.Remove(n);
                    }

                    perms.Add(new SevenPairsPermutation(blocks, jiang));
                    return perms;
                }

                for (int i = 0; i < 6; i++)
                {
                    Tile t = tiles[0];
                    Tile n = tiles.Find(x => x != t && x.CompatWith(t));
                    blocks[i] = new PairBlock(new[] { t, n });
                    tiles.Remove(t);
                    tiles.Remove(n);
                }

                if (tiles[0].CompatWith(tiles[1]))
                    perms.Add(new SevenPairsPermutation(blocks, new Block.Jiang(tiles[0], tiles[1])));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return perms;
        }

        public virtual List<Permutation> GetThirteenOrphansPerms(Player player)
        {
            List<Permutation> perms = new List<Permutation>();
            List<Tile> tiles = new List<Tile>(this.tiles);
            Block[] blocks = new Block[1];

            List<Tile> cands = new();

            if (tiles.Any(t => !t.IsYaoJiu(player)))
            {
                return new List<Permutation>();
            }

            try
            {
                Dictionary<string, int> tileNames = new Dictionary<string, int>();

                foreach (Tile tile in tiles)
                {
                    if (tileNames.ContainsKey(tile.ToString()))
                    {
                        tileNames[tile.ToString()]++;
                    }
                    else
                    {
                        tileNames[tile.ToString()] = 1;
                    }
                }

                bool foundPair = false;
                Tile jiang1 = null;
                Tile jiang2 = null;
                foreach (string name in tileNames.Keys)
                {
                    if (tileNames[name] == 2)
                    {
                        if (foundPair)
                        {
                            return new List<Permutation>();
                        }

                        foundPair = true;
                        jiang1 = tiles.First(t => name == t.ToString());
                        tiles.Remove(jiang1);
                        jiang2 = tiles.First(t => name == t.ToString());
                    }
                    else if (tileNames[name] == 1)
                    {
                        cands.Add(tiles.First(t => name == t.ToString()));
                    }
                    else
                    {
                        return new List<Permutation>();
                    }
                }

                if (!foundPair) return new List<Permutation>();

                blocks[0] = new ThirteenOrphansBlock(cands.ToArray());
                perms.Add(new ThirteenOrphansPermutation(blocks, new Block.Jiang(jiang1, jiang2)));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return perms;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Hand{");
            sb.Append("tiles=").Append(string.Join(",", tiles));
            sb.Append('}');
            return sb.ToString();
        }

        public virtual int GetSize()
        {
            return tiles.Count;
        }

        public virtual Permutation GetHighestScoredPerm(Player player)
        {
            return GetPerms(player, player.playMode).First();
        }

        public virtual Permutation GetHighestScoredPerm(List<Block> confirmedBlock, Player player)
        {
            return GetPerms(confirmedBlock, player, player.playMode).First();
        }

        public virtual Permutation GetHighestScoredPerm(Player player, Tile p1, Tile p2)
        {
            return GetPerms(player, player.playMode, new Block.Jiang(p1, p2))
                .First();
        }

        public virtual Permutation GetHighestScoredPerm(List<Block> confirmedBlock, Player player, Tile p1, Tile p2)
        {
            return GetPerms(confirmedBlock, player, player.playMode, new Block.Jiang(p1, p2))
                .First();
        }


        /// <summary>
        /// 核心常规型麻将组牌算法，从Tile的列表中获取所有可能从这个列表形成的牌型
        /// </summary>
        /// <param name="tiles">所有组成手牌的Tile</param>
        /// <param name="perms">需要往内添加牌型的列表</param>
        /// <param name="formedBlocks">递归中已经组好的面子</param>
        /// <param name="jiang">已经组好的雀头</param>
        public static void FindPermutation(List<Tile> tiles, List<Permutation> perms, List<Block> formedBlocks,
            Block.Jiang jiang, Player player)
        {
            BlockCombinator combinator = player.GetCombinator();
            if (tiles.Count == 0)
            {
                if (jiang == null) return;
                perms.Add(new Permutation(formedBlocks.ToArray(), new Block.Jiang(jiang.tile1, jiang.tile2)));
                return;
            }

            List<Tile> clonedTiles = new List<Tile>(tiles);

            Tile cand = clonedTiles[0];
            clonedTiles.RemoveAt(0);

            // Find XX
            if (jiang == null)
            {
                List<Tile> potentialTwins =
                    clonedTiles.Where(t => combinator.IsIdenticalFormingBlock(t, cand)).ToList();
                if (potentialTwins.Count > 0)
                {
                    foreach (Tile twin in potentialTwins)
                    {
                        Block.Jiang potJiang = new Block.Jiang(cand, twin);
                        List<Tile> tempTiles = new List<Tile>(clonedTiles);
                        tempTiles.Remove(twin);
                        FindPermutation(tempTiles, perms, formedBlocks, potJiang, player);
                    }
                }
            }

            // Find XXXX
            foreach (Tile another in clonedTiles.Where(t => combinator.IsIdentical(t, cand)))
            {
                foreach (Tile yetAnother in clonedTiles.Where(a =>
                             combinator.IsIdenticalFormingBlock(a, cand) && a != another && another != cand))
                {
                    foreach (Tile lastAnother in clonedTiles.Where(a =>
                                 combinator.IsIdenticalFormingBlock(a, cand) && a != yetAnother && a != another && a != cand))
                    {
                        BuildKong(cand, another, yetAnother, lastAnother, tiles, perms, formedBlocks, jiang, player);
                    }
                }
            }

            // Find XBC and AXB
            foreach (Tile potSucc in clonedTiles.Where(t => combinator.ASuccBFormingBlock(t, cand)))
            {
                // Find XBC
                foreach (Tile potWSucc in clonedTiles.Where(a =>
                             combinator.ASuccBFormingBlock(a, potSucc) && a != cand && cand != potSucc))
                {
                    BuildBlock(cand, potSucc, potWSucc, tiles, perms, formedBlocks, jiang, player);
                }

                // Find AXB
                foreach (Tile potPred in clonedTiles.Where(a =>
                             combinator.APredBFormingBlock(a, cand) && a != cand && cand != potSucc))
                {
                    BuildBlock(cand, potSucc, potPred, tiles, perms, formedBlocks, jiang, player);
                }
            }

            // Find ABX
            foreach (Tile potPred in clonedTiles.Where(a => combinator.APredBFormingBlock(a, cand)))
            {
                foreach (Tile potWPred in clonedTiles.Where(a =>
                             combinator.APredBFormingBlock(a, potPred) && a != cand && cand != potPred))
                {
                    BuildBlock(cand, potPred, potWPred, tiles, perms, formedBlocks, jiang, player);
                }
            }

            // Find XXX
            foreach (Tile another in clonedTiles.Where(t => combinator.IsIdentical(t, cand)))
            {
                foreach (Tile yetAnother in clonedTiles.Where(a =>
                             combinator.IsIdenticalFormingBlock(a, cand) && a != another && another != cand))
                {
                    BuildBlock(cand, another, yetAnother, tiles, perms, formedBlocks, jiang, player);
                }
            }

            if (tiles.Count == 0)
            {
                if (jiang != null && formedBlocks.Count > 0)
                {
                    perms.Add(new Permutation(formedBlocks.ToArray(), jiang));
                }
            }
        }

        public static void BuildBlock(Tile t1, Tile t2, Tile t3, List<Tile> tiles, List<Permutation> perms,
            List<Block> formedBlocks, Block.Jiang jiang, Player player)
        {
            player.GetCombinator().CombineBlock(t1, t2, t3, tiles, perms, formedBlocks, jiang, player);
        }

        public static void BuildKong(Tile t1, Tile t2, Tile t3, Tile t4, List<Tile> tiles, List<Permutation> perms,
            List<Block> formedBlocks, Block.Jiang jiang, Player player)
        {
            player.GetCombinator().CombineKong(t1, t2, t3, t4, tiles, perms, formedBlocks, jiang, player);
        }

        public static Hand PlainFullHand()
        {
            return new("111122223333444455556666777788889999m" +
                       "111122223333444455556666777788889999s" +
                       "111122223333444455556666777788889999p" +
                       "1111222233334444555566667777z");
        }

        public static Hand NumberedFullHand()
        {
            return new("111122223333444455556666777788889999m" +
                       "111122223333444455556666777788889999s" +
                       "111122223333444455556666777788889999p");
        }

        public static Hand PlainUniqueHand()
        {
            return new("123456789m" +
                       "123456789s" +
                       "123456789p" +
                       "1234567z");
        }

        public static Hand NumberedUniqueHand()
        {
            return new("123456789m" +
                       "123456789s" +
                       "123456789p");
        }
    }
}