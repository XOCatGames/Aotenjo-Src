using System.Collections.Generic;
using static Aotenjo.Block;

namespace Aotenjo
{
    public abstract class BlockCombinator
    {
        public static BlockCombinator Default = new DefaultBlockCombinator();
        public static BlockCombinator Apollo = new ApolloBlockCombinator();
        public static BlockCombinator YingYang = new YingYangBlockCombinator();

        public static BlockCombinator Scarlet(Tile.Category unusedCategory) =>
            new ScarletBlockCombinator(unusedCategory);

        public abstract bool ASuccB(Tile tileSucc, Tile tilePred, bool categorySensitive = true, int step = 1);

        public virtual bool ASuccBFormingBlock(Tile tileSucc, Tile tilePred, bool categorySensitive = true,
            int step = 1)
        {
            return ASuccB(tileSucc, tilePred, categorySensitive, step);
        }

        public bool APredBFormingBlock(Tile tilePred, Tile tileSucc, bool categorySensitive = true, int step = 1)
        {
            return ASuccBFormingBlock(tileSucc, tilePred, categorySensitive, step);
        }

        public virtual bool ASuccB(Block blockSucc, Block blockPred, bool categorySensitive = true, int step = 1)
        {
            if (categorySensitive && blockSucc.GetCategory() != blockPred.GetCategory())
            {
                return false;
            }

            if ((blockSucc.IsABC() && blockPred.IsAAA()) || (blockSucc.IsAAA() && blockPred.IsABC()))
            {
                return false;
            }

            return ASuccB(blockSucc.tiles[0], blockPred.tiles[0], categorySensitive, step) &&
                   ASuccB(blockSucc.tiles[1], blockPred.tiles[1], categorySensitive, step);
        }

        public bool APredB(Tile tilePred, Tile tileSucc, bool categorySensitive = true, int step = 1)
        {
            return ASuccB(tileSucc, tilePred, categorySensitive, step);
        }

        public virtual bool IsIdentical(Tile tileSucc, Tile tilePred, bool categorySensitive = true)
        {
            if (categorySensitive)
            {
                return tileSucc.CompactWith(tilePred);
            }

            if (tileSucc.IsNumbered() && tilePred.IsNumbered())
            {
                return tileSucc.GetOrder() == tilePred.GetOrder();
            }

            return tileSucc.CompactWith(tilePred);
        }

        //判断block的identical
        public virtual bool IsIdentical(Block b1, Block b2, bool categorySensitive = true)
        {
            if (b1.IsABC() && b2.IsABC())
            {
                if (categorySensitive && b1.GetCategory() != b2.GetCategory())
                    return false;

                for (int i = 0; i < 3; i++)
                {
                    if (!IsIdentical(b1.tiles[i], b2.tiles[i], categorySensitive)) return false;
                }

                return true;
            }

            if (b1.IsAAA() && b2.IsAAA())
            {
                return IsIdentical(b1.tiles[0], b2.tiles[0], categorySensitive);
            }

            return false;
        }

        public virtual void CombineBlock(Tile t1, Tile t2, Tile t3, List<Tile> tiles, List<Permutation> perms,
            List<Block> formedBlocks, Jiang jiang, Player player)
        {
            if (player.GetArtifacts().Contains(Artifacts.Nunchaku))
            {
                if (t1.IsNumbered() && t1.GetOrder() == 4 && t2.IsNumbered() && t2.GetOrder() == 5 && t3.IsNumbered() &&
                    t3.GetOrder() == 6)
                {
                    return;
                }
            }

            List<Tile> clonedTiles = new List<Tile>(tiles);
            Block block = new Block(new[] { t1, t2, t3 });
            List<Block> formedBlocksCopy = new List<Block>(formedBlocks);
            List<Tile> tempTiles = new List<Tile>(clonedTiles);
            formedBlocksCopy.Add(block);
            tempTiles.Remove(t1);
            tempTiles.Remove(t2);
            tempTiles.Remove(t3);
            Hand.FindPermutation(tempTiles, perms, formedBlocksCopy, jiang, player);
        }

        public virtual void CombineKong(Tile t1, Tile t2, Tile t3, Tile t4, List<Tile> tiles, List<Permutation> perms,
            List<Block> formedBlocks, Jiang jiang, Player player)
        {
            List<Tile> clonedTiles = new List<Tile>(tiles);
            Block block = new Block(new[] { t1, t2, t3, t4 });
            List<Block> formedBlocksCopy = new List<Block>(formedBlocks);
            List<Tile> tempTiles = new List<Tile>(clonedTiles);
            formedBlocksCopy.Add(block);
            tempTiles.Remove(t1);
            tempTiles.Remove(t2);
            tempTiles.Remove(t3);
            tempTiles.Remove(t4);
            Hand.FindPermutation(tempTiles, perms, formedBlocksCopy, jiang, player);
        }

        public virtual bool IsIdenticalFormingBlock(Tile t, Tile cand)
        {
            return IsIdentical(t, cand);
        }

        /// <summary>
        /// 默认的BlockCombinator实现
        /// </summary>
        private class DefaultBlockCombinator : BlockCombinator
        {
            public override bool ASuccB(Tile tileSucc, Tile tilePred, bool categorySensitive = true, int step = 1)
            {
                if (categorySensitive && !tileSucc.IsSameCategory(tilePred)) return false;
                return tileSucc.IsNumbered() && tilePred.IsNumbered() &&
                       tileSucc.GetOrder() == tilePred.GetOrder() + step;
            }
        }

        /// <summary>
        /// 银河麻将的阿波罗规则BlockCombinator实现，字牌可以按照特定的顺序连接，数字牌按照顺序连接，且可以跨19连接
        /// </summary>
        private class ApolloBlockCombinator : BlockCombinator
        {
            public override bool ASuccB(Tile tileSucc, Tile tilePred, bool categorySensitive = true, int step = 1)
            {
                //风牌连接
                if (tileSucc.CompactWithCategory(Tile.Category.Feng) &&
                    tilePred.CompactWithCategory(Tile.Category.Feng))
                {
                    return tileSucc.GetOrder() == tilePred.GetOrder() % 4 + step;
                }

                //箭牌连接
                if (tileSucc.CompactWithCategory(Tile.Category.Jian) &&
                    tilePred.CompactWithCategory(Tile.Category.Jian))
                {
                    return tileSucc.GetOrder() == ((tilePred.GetOrder() - 4) % 3 + step) + 4;
                }

                //数字牌连接
                if (tileSucc.IsNumbered() && tilePred.IsNumbered())
                {
                    return (!categorySensitive || tileSucc.IsSameCategory(tilePred)) &&
                           tileSucc.GetOrder() == ((tilePred.GetOrder() - 1 + step) % 9) + 1;
                }

                return false;
            }

            public override bool ASuccB(Block blockSucc, Block blockPred, bool categorySensitive = true, int step = 1)
            {
                if (blockSucc.IsABC() && blockPred.IsABC() && blockSucc.GetCategory() == Tile.Category.Jian &&
                    blockPred.GetCategory() == Tile.Category.Jian) return true;
                return base.ASuccB(blockSucc, blockPred, categorySensitive, step);
            }
        }

        private class YingYangBlockCombinator : DefaultBlockCombinator
        {
            public override bool ASuccBFormingBlock(Tile tileSucc, Tile tilePred, bool categorySensitive = true,
                int step = 1)
            {
                if (tileSucc.IsNumbered() && tilePred.IsNumbered())
                {
                    if (tileSucc.GetOrder() == tilePred.GetOrder() + 2) return true;
                }

                return base.ASuccBFormingBlock(tileSucc, tilePred, categorySensitive, step);
            }

            public override void CombineBlock(Tile t1, Tile t2, Tile t3, List<Tile> tiles, List<Permutation> perms,
                List<Block> formedBlocks, Jiang jiang, Player player)
            {
                if (t1.IsNumbered() && t2.IsNumbered() && t3.IsNumbered())
                {
                    if (t1.GetOrder() + 2 == t2.GetOrder() && t2.GetOrder() + 1 == t3.GetOrder())
                    {
                        return;
                    }

                    if (t1.GetOrder() + 1 == t2.GetOrder() && t2.GetOrder() + 2 == t3.GetOrder())
                    {
                        return;
                    }
                }

                base.CombineBlock(t1, t2, t3, tiles, perms, formedBlocks, jiang, player);
            }
        }

        private class ScarletBlockCombinator : DefaultBlockCombinator
        {
            private Tile.Category unusedCategory;

            public ScarletBlockCombinator(Tile.Category unusedCategory)
            {
                this.unusedCategory = unusedCategory;
            }

            public override bool IsIdenticalFormingBlock(Tile t, Tile cand)
            {
                if (t.GetCategory() == unusedCategory || cand.GetCategory() == unusedCategory) return false;
                return base.IsIdenticalFormingBlock(t, cand);
            }

            public override void CombineBlock(Tile t1, Tile t2, Tile t3, List<Tile> tiles, List<Permutation> perms,
                List<Block> formedBlocks, Jiang jiang, Player player)
            {
                if (t1.GetCategory() == unusedCategory || t2.GetCategory() == unusedCategory ||
                    t3.GetCategory() == unusedCategory ||
                    (jiang != null && jiang.Any(t => t.GetCategory() == unusedCategory)))
                {
                    return; //如果有未使用的类别，则不允许形成Block
                }

                base.CombineBlock(t1, t2, t3, tiles, perms, formedBlocks, jiang, player);
            }
            
            //杠同理
            public override void CombineKong(Tile t1, Tile t2, Tile t3, Tile t4, List<Tile> tiles,
                List<Permutation> perms, List<Block> formedBlocks, Jiang jiang, Player player)
            {
                if (t1.GetCategory() == unusedCategory || t2.GetCategory() == unusedCategory ||
                    t3.GetCategory() == unusedCategory || t4.GetCategory() == unusedCategory ||
                    (jiang != null && jiang.Any(t => t.GetCategory() == unusedCategory)))
                {
                    return; //如果有未使用的类别，则不允许形成Kong
                }

                base.CombineKong(t1, t2, t3, t4, tiles, perms, formedBlocks, jiang, player);
            }
        }
    }
}