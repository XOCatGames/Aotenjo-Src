using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class YakuTester
    {
        public static Dictionary<YakuType, Yaku> InfoMap;

        private static readonly Dictionary<int, Tile.Category> CateDict = new()
        {
            { 0, Tile.Category.Wan },
            { 1, Tile.Category.Suo },
            { 2, Tile.Category.Bing }
        };

        public static readonly Dictionary<YakuType, Func<Permutation, Player, bool>> YAKUS_PREDICATE_MAP =
            new Dictionary<YakuType, Func<Permutation, Player, bool>>
            {
                { FixedYakuType.PingHu, VerifyPingHu },
                { FixedYakuType.YiBanGao, VerifyYiBanGao },

                { FixedYakuType.JianKe, VerifyJianKe },
                { FixedYakuType.QuanFengKe, VerifyQuanFengKe },
                { FixedYakuType.MenFengKe, VerifyMenFengKe },
                { FixedYakuType.SanFengKe, VerifySanFengKe },
                { FixedYakuType.ShuangJianKe, VerifyShuangJianKe },
                { FixedYakuType.HunYaoJiu, VerifyHunYaoJiu },
                { FixedYakuType.XiaoSanYuan, VerifyXiaoSanYuan },
                { FixedYakuType.XiaoSiXi, VerifyXiaoSiXi },
                { FixedYakuType.ShuangTongZiKe, VerifyShuangTongZiKe },
                { FixedYakuType.DaSanYuan, VerifyDaSanYuan },
                { FixedYakuType.DaSiXi, VerifyDaSiXi },
                { FixedYakuType.QingYaoJiu, VerifyQingYaoJiu },
                { FixedYakuType.SanTongZiKe, VerifySanTongZiKe },
                { FixedYakuType.SiTongZiKe, VerifySiTongZiKe },

                { FixedYakuType.HuaLong, VerifyHuaLong },
                { FixedYakuType.QingLong, VerifyQingLong },
                { FixedYakuType.QuanDaiYao, VerifyQuanDaiYao },

                //2024.5.28新加番种
                { FixedYakuType.LianLiu, VerifyLianLiu },
                { FixedYakuType.XiXiangFeng, VerifyXiXiangFeng },
                { FixedYakuType.LaoShaoFu, VerifyLaoShaoFu },
                { FixedYakuType.SanSeSanBuGao, VerifySanSeSanBuGao },
                { FixedYakuType.SanSeShuangLongHui, VerifySanSeShuangLongHui },
                { FixedYakuType.QuanDa, VerifyQuanDa },
                { FixedYakuType.QuanZhong, VerifyQuanZhong },
                { FixedYakuType.QuanXiao, VerifyQuanXiao },
                { FixedYakuType.LyuYiSe, VerifyLyuYiSe },
                { FixedYakuType.JiuLianBaoDeng, VerifyJiuLianBaoDeng },

                //大于/小于/全带五
                { FixedYakuType.DaYuWu, VerifyDaYuWu },
                { FixedYakuType.XiaoYuWu, VerifyXiaoYuWu },
                { FixedYakuType.QuanDaiWu, VerifyQuanDaiWu },

                //2024.5.28新加番种2
                { FixedYakuType.YiSeSiTongShun, VerifyYiSeSiTongShun },
                { FixedYakuType.YiSeSanBuGao, VerifyYiSeSanBuGao },
                { FixedYakuType.YiSeShuangLongHui, VerifyYiSeShuangLongHui },
                { FixedYakuType.LianQiDui, VerifyLianQiDui },
                { FixedYakuType.TuiBuDao, VerifyTuiBuDao },
                { FixedYakuType.YiSeSanJieGao, VerifyYiSeSanJieGao },
                { FixedYakuType.YiSeSiJieGao, VerifyYiSeSiJieGao },
                { FixedYakuType.WuZi, VerifyWuZi },

                //2024.5.29新加番种
                { FixedYakuType.SiGuiYi, VerifySiGuiYi },
                { FixedYakuType.WuGuiYi, VerifyWuGuiYi },
                { FixedYakuType.DuoGuiYi, VerifyDuoGuiYi },
                { FixedYakuType.ShuangSeShuangTongKe, VerifyShuangSeShuangTongKe },
                { FixedYakuType.SanSeSanTongKe, VerifySanSeSanTongKe },
                { FixedYakuType.QuanShuangKe, VerifyQuanShuangKe },
                { FixedYakuType.YiSeShuangTongKe, VerifyYiSeShuangTongKe },
                { FixedYakuType.YiSeSanTongKe, VerifyYiSeSanTongKe },
                { FixedYakuType.YiSeSiTongKe, VerifyYiSeSiTongKe },

                //2024.6.3新加番种
                { FixedYakuType.YiSeSiBuGao, VerifyYiSeSiBuGao },
                { FixedYakuType.JingTongShun, VerifyJingTongShun },
                { FixedYakuType.JingTongKe, VerifyJingTongKe },
                { FixedYakuType.BaiWanShi, VerifyBaiWanShi },


                //2024.6.15新加番种
                { FixedYakuType.Gang, VerifyGang },
                { FixedYakuType.ShuangGang, VerifyShuangGang },
                { FixedYakuType.SanGang, VerifySanGang },
                { FixedYakuType.SiGang, VerifySiGang },
                { FixedYakuType.LiangBanGao, VerifyLiangBanGao },

                { FixedYakuType.QiDui, VerifyQiDui },
                { FixedYakuType.ShiSanYao, VerifyThirteenOrphan },

                { FixedYakuType.SanSeSanJieGao, VerifySanSeSanJieGao },
                { FixedYakuType.DuanHong, VerifyDuanHong },

                //2024.9.8新加番种
                { FixedYakuType.JinMenQiao, VerifyJinMenQiao },
                { FixedYakuType.QiXingDui, VerifyQiXingDui },
                { FixedYakuType.SanYuanDui, VerifySanYuanDui },
                { FixedYakuType.SiXiDui, VerifySiXiDui },
                { FixedYakuType.TiaoPaiKe, VerifyTiaoPaiKe },
                { FixedYakuType.JinPaiKe, VerifyJinPaiKe },
                { FixedYakuType.DinSanKe, VerifyDinSanKe },
                { FixedYakuType.JiangDui, VerifyJiangDui },
                { FixedYakuType.SanSeTiaoPaiKe, VerifySanSeTiaoPaiKe },
                { FixedYakuType.SanSeJinPaiKe, VerifySanSeJinPaiKe },
                { FixedYakuType.SanSeDinSanKe, VerifySanSeDinSanKe },
                { FixedYakuType.SiTiaoPaiKe, VerifySiTiaoPaiKe },
                { FixedYakuType.QingDongMen, VerifyQingDongMen },
                { FixedYakuType.DaCheLun, VerifyDaCheLun },
                { FixedYakuType.XiaoCheLun, VerifyXiaoCheLun },
                { FixedYakuType.DaZhuLin, VerifyDaZhuLin },
                { FixedYakuType.XiaoZhuLin, VerifyXiaoZhuLin },
                { FixedYakuType.DaShuLin, VerifyDaShuLin },
                { FixedYakuType.XiaoShuLin, VerifyXiaoShuLin },
                { FixedYakuType.ChunQuanDaiYao, VerifyChunQuanDaiYao },
                { FixedYakuType.HongKongQue, VerifyHongKongQue },

                { FixedYakuType.TianDiChuangZao, VerifyTianDiChuangZao },

                { FixedYakuType.ShuangTongZiShun, VerifyShuangTongZiShun },
                { FixedYakuType.SanTongZiShun, VerifySanTongZiShun },
                { FixedYakuType.SiTongZiShun, VerifySiTongZiShun },
                { FixedYakuType.SiFengShun, VerifySiXiShun },
                { FixedYakuType.ZhengHua, VerifyZhengHua },
                { FixedYakuType.YiTaiHua, VerifyYiTaiHua },
                { FixedYakuType.BaXianGuoHai, VerifyBaXianGuoHai },

                { FixedYakuType.HunYiSe, VerifyHunYiSe },
                { FixedYakuType.WuMenQi, VerifyWuMenQi },
                { FixedYakuType.QingYiSe, VerifyQingYiSe },
                { FixedYakuType.ShuangKe, VerifyShuangAnKe },
                { FixedYakuType.SanKe, VerifySanAnKe },
                { FixedYakuType.SiKe, VerifySiAnKe },
                { FixedYakuType.SanSeSanTongShun, VerifySanSeTongShun },
                { FixedYakuType.YiSeSanTongShun, VerifyYiSeSanTongShun },
                { FixedYakuType.DuanYao, VerifyDuanYaoJiu },
                { FixedYakuType.QueYiMen, VerifyQueYiMen },
                { FixedYakuType.ZiYiSe, VerifyZiYiSe },

                { FixedYakuType.QiTongDui, VerifyQiTongDui },
                { FixedYakuType.WuFanHu, VerifyWuFanHu },
                { FixedYakuType.QuanDan, VerifyQuanDan },

                { FixedYakuType.YinYangShun, VerifyYinYangShun },
                { FixedYakuType.YinKou, VerifyYinKou },
                { FixedYakuType.YinYangLong, VerifyYinYangLong },
                { FixedYakuType.YinYangSanBuGao, VerifyYinYangSanBuGao },
                { FixedYakuType.YinYangLiangBanGao, VerifyYinYangLiangBanGao },
                { FixedYakuType.NaiHeQiao, VerifyNaiHeQiao },
                { FixedYakuType.LiangJieQiao, VerifyLiangJieQiao },

                { FixedYakuType.LongQiDui, VerifyLongQiDui },
                { FixedYakuType.ShuangLongQiDui, VerifyShuangLongQiDui },
                { FixedYakuType.SanLongQiDui, VerifySanLongQiDui },

                { FixedYakuType.Base, ((_, _) => true) }
            };

        private static bool VerifyLongQiDui(Permutation permutation, Player player)
        {
            if (permutation.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> blocks = permutation.blocks.ToList().Append(permutation.jiang.ToPairBlock()).ToList();

            (Block, Block)? pair = FindIdenticalBlockPair(blocks);
            if (pair != null)
            {
                tilesToHighlight = pair?.Item1.tiles.Union(pair?.Item2.tiles).ToList();
                return true; 
            }
            return false;
        }

        private static bool VerifyShuangLongQiDui(Permutation permutation, Player player)
        {
            if (permutation.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> blocks = permutation.blocks.ToList().Append(permutation.jiang.ToPairBlock()).ToList();

            (Block, Block)? pair = FindIdenticalBlockPair(blocks);
            if (pair == null) return false;
            blocks.Remove(pair.Value.Item1);
            blocks.Remove(pair.Value.Item2);

            (Block, Block)? pair2 = FindIdenticalBlockPair(blocks);
            if (pair2 != null)
            {
                tilesToHighlight = pair?.Item1.tiles.Union(pair?.Item2.tiles).Union(pair2?.Item1.tiles).Union(pair2?.Item2.tiles).ToList();
                return true; 
            }
            return false;
        }

        private static bool VerifySanLongQiDui(Permutation permutation, Player player)
        {
            if (permutation.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> blocks = permutation.blocks.ToList().Append(permutation.jiang.ToPairBlock()).ToList();

            (Block, Block)? pair = FindIdenticalBlockPair(blocks);
            if (pair == null) return false;
            blocks.Remove(pair.Value.Item1);
            blocks.Remove(pair.Value.Item2);

            var pair2 = FindIdenticalBlockPair(blocks);
            if (pair2 == null) return false;
            blocks.Remove(pair2.Value.Item1);
            blocks.Remove(pair2.Value.Item2);

            var pair3 = FindIdenticalBlockPair(blocks);

            if (pair3 != null)
            {
                tilesToHighlight = pair?.Item1.tiles
                    .Union(pair?.Item2.tiles)
                    .Union(pair2?.Item1.tiles)
                    .Union(pair2?.Item2.tiles)
                    .Union(pair3?.Item1.tiles)
                    .Union(pair3?.Item2.tiles)
                    .ToList();
                return true; 
            }
            return false;
        }

        private static (Block, Block)? FindIdenticalBlockPair(IEnumerable<Block> blocks)
        {
            foreach (var b in blocks)
            {
                Block copy = blocks.FirstOrDefault(b2 => b2 != b && b.CompactWith(b2));
                if (copy != null)
                {
                    return (b, copy);
                }
            }

            return null;
        }

        private static bool VerifyYinYangShun(Permutation permutation, Player player)
        {
            return VerifyForTwoBlocks(permutation, player,
                a => a.IsABC() && a.IsYinSeq(),
                (a, b) => b.IsABC() && b.OfSameCategory(a) && !b.IsYinSeq() && b.tiles[1].CompactWith(a.tiles[1])
            );
        }

        private static bool VerifyYinKou(Permutation permutation, Player player)
        {
            return VerifyForTwoBlocks(permutation, player,
                a => a.IsABC() && a.IsYinSeq(),
                (a, b) => player.DetermineShiftedPair(a, b, 1, true)
            );
        }

        private static bool VerifyYinYangLong(Permutation permutation, Player player)
        {
            return VerifyForThreeBlocks(permutation, player,
                a => a.CompactWithNumbers("123"),
                (a, b) => b.CompactWithNumbers("789") && b.OfSameCategory(a),
                (a, _, c) => c.CompactWithNumbers("357") && c.OfSameCategory(a)
            );
        }

        private static bool VerifyYinYangSanBuGao(Permutation permutation, Player player)
        {
            return VerifyForThreeBlocks(permutation, player,
                a => a.IsABC() && a.IsYinSeq(),
                (a, b) => b.OfSameCategory(a) && b.IsABC() && !b.IsYinSeq() &&
                          b.tiles[0].GetOrder() > a.tiles[0].GetOrder(),
                (a, b, c) => c.OfSameCategory(a) && c.IsABC() && c.IsYinSeq() &&
                             player.DetermineShiftedPair(a, c, 2, true) &&
                             b.tiles[0].GetOrder() <= c.tiles[0].GetOrder() + 1
            );
        }

        private static bool VerifyYinYangLiangBanGao(Permutation permutation, Player player)
        {
            return VerifyForFourBlocks(permutation, player,
                a => a.IsABC() && a.IsYinSeq(),
                (a, b) => b.IsABC() && !b.IsYinSeq() && b.tiles[1].CompactWith(a.tiles[1]),
                (_, _, c) => c.IsABC() && c.IsYinSeq(),
                (_, _, c, d) => d.IsABC() && !d.IsYinSeq() && d.tiles[1].CompactWith(c.tiles[1])
            );
        }

        private static bool VerifyNaiHeQiao(Permutation permutation, Player player)
        {
            return VerifyForFourBlocks(permutation, player,
                a => a.CompactWithNumbers("123"),
                (a, b) => b.CompactWithNumbers("789") && b.OfSameCategory(a),
                (a, _, c) => c.CompactWithNumbers("246") && c.OfSameCategory(a),
                (a, _, _, d) => d.CompactWithNumbers("468") && d.OfSameCategory(a)
            );
        }

        private static bool VerifyLiangJieQiao(Permutation permutation, Player player)
        {
            return VerifyForFourBlocks(permutation, player,
                a => a.CompactWithNumbers("123"),
                (a, b) => b.CompactWithNumbers("789") && b.OfSameCategory(a),
                (a, _, c) => c.CompactWithNumbers("246") && !c.OfSameCategory(a),
                (_, _, c, d) => d.CompactWithNumbers("468") && d.OfSameCategory(c)
            );
        }

        private static bool VerifyQuanDan(Permutation permutation, Player player)
        {
            return VerifyForAllTiles(permutation, player, t => t.IsNumbered() && t.GetOrder() % 2 == 1);
        }

        private static bool VerifyWuFanHu(Permutation permutation, Player player)
        {
            return permutation.IsFullHand() &&
                   !permutation.GetYakus(player, y => !Equals(y, (YakuType)FixedYakuType.WuFanHu) && !Equals(y, (YakuType)FixedYakuType.Base)).Any();
        }

        private static bool VerifyQiTongDui(Permutation permutation, Player player)
        {
            return permutation.GetPermType() == PermutationType.SEVEN_PAIRS &&
                   permutation.TilesFulfullAll(t => t.CompactWith(permutation.ToTiles()[0]));
        }

        private static bool VerifySiXiShun(Permutation permutation, Player player)
        {
            return VerifyForFourBlocks(permutation, player, block => block.ToFormat() == "123z",
                (a, b) => b.ToFormat() == "234z" && b.OfSameCategory(a),
                (_, _, c) => c.ToFormat() == "341z",
                (_, _, _, d) => d.ToFormat() == "412z");
        }

        private static bool VerifyBaXianGuoHai(Permutation permutation, Player player)
        {
            if (player is RainbowDeck.RainbowPlayer rplayer)
            {
                Tile.Category[] cats =
                    { Tile.Category.SiJi, Tile.Category.JunZi, Tile.Category.SiYi, Tile.Category.SiYe };
                if (cats.Count(c => new[] { 1, 2, 3, 4 }.All(o =>
                        rplayer.PlayedFlowerTiles.Any(t => t.GetOrder() == o && t.GetCategory() == c))) == 2)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyYiTaiHua(Permutation permutation, Player player)
        {
            if (player is RainbowDeck.RainbowPlayer rplayer)
            {
                Tile.Category[] cats =
                    { Tile.Category.SiJi, Tile.Category.JunZi, Tile.Category.SiYi, Tile.Category.SiYe };
                if (cats.Any(c => new[] { 1, 2, 3, 4 }.All(o =>
                        rplayer.PlayedFlowerTiles.Any(t => t.GetOrder() == o && t.GetCategory() == c))))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyZhengHua(Permutation permutation, Player player)
        {
            if (player is RainbowDeck.RainbowPlayer rplayer)
            {
                if (rplayer.PlayedFlowerTiles.Any(f => player.IsPlayerWind(f.GetOrder())))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifySiTongZiShun(Permutation permutation, Player player)
        {
            
            //和三同字顺类似
            
            return VerifyForFourBlocks(permutation, player,
                a => a.IsHonor(player) && a.IsABC(),
                (a, b) => b.CompactWith(a),
                (a, _, c) => c.CompactWith(a),
                (a, _, _, d) => d.CompactWith(a)
            );
        }

        private static bool VerifySanTongZiShun(Permutation permutation, Player player)
        {
            return VerifyForThreeBlocks(permutation, player,
                a => a.IsHonor(player) && a.IsABC(),
                (a, b) => b.CompactWith(a),
                (a, _, c) => c.CompactWith(a)
            );
        }

        private static bool VerifyShuangTongZiShun(Permutation permutation, Player player)
        {
            return VerifyForTwoBlocks(permutation, player, a => a.IsHonor(player) && a.IsABC(),
                (a, b) => a.CompactWith(b));
        }

        private static bool VerifyTianXiaGuiZhong(Permutation perm, Player player)
        {
            return VerifySiGang(perm, player) && perm.ToTiles().All(t => t.CompactWith(new Tile("7z")));
        }

        private static bool VerifyWanWuShengZhang(Permutation perm, Player player)
        {
            return VerifySiGang(perm, player) && perm.ToTiles().All(t => t.CompactWith(new Tile("6z")));
        }

        private static bool VerifyTianDiChuangZao(Permutation perm, Player player)
        {
            return VerifySiGang(perm, player) && VerifyForAllTiles(perm, player, t => t.CompactWith(new Tile("5z")));
        }

        private static bool VerifyDaShuLin(Permutation perm, Player player)
        {
            return VerifyLianQiDui(perm, player) &&
                   VerifyForAllTiles(perm, player, t => !t.IsYaoJiu(player) && t.GetCategory() == Tile.Category.Wan);
        }

        private static bool VerifyXiaoShuLin(Permutation perm, Player player)
        {
            List<Tile> tiles = perm.ToTiles();
            return VerifyLianQiDui(perm, player) && tiles.Any(t => t.IsYaoJiu(player)) &&
                   VerifyForAllTiles(perm, player, t => t.GetCategory() == Tile.Category.Wan);
        }

        private static bool VerifyDaZhuLin(Permutation perm, Player player)
        {
            return VerifyLianQiDui(perm, player) &&
                   VerifyForAllTiles(perm, player, t => !t.IsYaoJiu(player) && t.GetCategory() == Tile.Category.Suo);
        }

        private static bool VerifyXiaoZhuLin(Permutation perm, Player player)
        {
            List<Tile> tiles = perm.ToTiles();
            return VerifyLianQiDui(perm, player) && tiles.Any(t => t.IsYaoJiu(player)) &&
                   VerifyForAllTiles(perm, player, t => t.GetCategory() == Tile.Category.Suo);
        }

        private static bool VerifyDaCheLun(Permutation perm, Player player)
        {
            return VerifyLianQiDui(perm, player) &&
                   VerifyForAllTiles(perm, player, t => !t.IsYaoJiu(player) && t.GetCategory() == Tile.Category.Bing);
        }

        private static bool VerifyXiaoCheLun(Permutation perm, Player player)
        {
            List<Tile> tiles = perm.ToTiles();
            return VerifyLianQiDui(perm, player) && tiles.Any(t => t.IsYaoJiu(player)) &&
                   VerifyForAllTiles(perm, player, t => t.GetCategory() == Tile.Category.Bing);
        }

        private static bool VerifySiTiaoPaiKe(Permutation perm, Player status)
        {
            bool Succ(Player player, Block b2, Block b1, int step)
            {
                return player.GetCombinator().ASuccB(b2.tiles[0], b1.tiles[0], true, step);
            }

            for (int i = 0; i < 3; i++)
            {
                if (VerifyForFourBlocks(perm, status,
                        a => a.IsAAA() && a.OfCategory(CateDict[i]),
                        (a, b) => b.IsAAA() && Succ(status, b, a, 2) && b.OfCategory(CateDict[i]),
                        (_, b, c) => c.IsAAA() && Succ(status, c, b, 2) && c.OfCategory(CateDict[i]),
                        (_, _, c, d) => d.IsAAA() && Succ(status, d, c, 2) && d.OfCategory(CateDict[i])
                    ))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifySanSeDinSanKe(Permutation perm, Player player)
        {
            Func<Player, Block, Block, int, bool> Succ = (player, b2, b1, step) =>
            {
                return player.GetCombinator().ASuccB(b2.tiles[0], b1.tiles[0], false, step);
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        if (perm.blocks.Any(a => a.IsAAA() && a.OfCategory(CateDict[i])
                                                           && perm.blocks.Any(b => b != a && b.IsAAA() &&
                                                               Succ(player, b, a, 4) && b.OfCategory(CateDict[j])
                                                               && perm.blocks.Any(c =>
                                                                   c != b && c != a && c.IsAAA() &&
                                                                   Succ(player, c, b, 4) &&
                                                                   c.OfCategory(CateDict[k])))))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool VerifySanSeJinPaiKe(Permutation perm, Player player)
        {
            Func<Player, Block, Block, int, bool> Succ = (player, b2, b1, step) =>
            {
                return player.GetCombinator().ASuccB(b2.tiles[0], b1.tiles[0], false, step);
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        if (perm.blocks.Any(a => a.IsAAA() && a.OfCategory(CateDict[i])
                                                           && perm.blocks.Any(b => b != a && b.IsAAA() &&
                                                               Succ(player, b, a, 3) && b.OfCategory(CateDict[j])
                                                               && perm.blocks.Any(c =>
                                                                   c != b && c != a && c.IsAAA() &&
                                                                   Succ(player, c, b, 3) &&
                                                                   c.OfCategory(CateDict[k])))))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool VerifySanSeTiaoPaiKe(Permutation perm, Player player)
        {
            Func<Player, Block, Block, int, bool> Succ = (player, b2, b1, step) =>
            {
                return player.GetCombinator().ASuccB(b2.tiles[0], b1.tiles[0], false, step);
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        if (perm.blocks.Any(a => a.IsAAA() && a.OfCategory(CateDict[i])
                                                           && perm.blocks.Any(b => b != a && b.IsAAA() &&
                                                               Succ(player, b, a, 2) && b.OfCategory(CateDict[j])
                                                               && perm.blocks.Any(c =>
                                                                   c != b && c != a && c.IsAAA() &&
                                                                   Succ(player, c, b, 2) &&
                                                                   c.OfCategory(CateDict[k])))))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool VerifyTiaoPaiKe(Permutation perm, Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (perm.blocks.Any(b => b.IsAAA() && b.OfCategory(CateDict[i]) && perm.blocks.Any(c =>
                        c.IsAAA() && player.GetCombinator().ASuccB(c.tiles[0], b.tiles[0], true, 2) &&
                        c.OfCategory(CateDict[i]) && perm.blocks.Any(d =>
                            d.IsAAA() && player.GetCombinator().ASuccB(d.tiles[0], c.tiles[0], true, 2) &&
                            d.OfCategory(CateDict[i])))))
                    return true;
            }

            return false;
        }

        private static bool VerifyJinPaiKe(Permutation perm, Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (perm.blocks.Any(b => b.IsAAA() && b.OfCategory(CateDict[i]) && perm.blocks.Any(c =>
                        c.IsAAA() && player.GetCombinator().ASuccB(c.tiles[0], b.tiles[0], true, 3) &&
                        c.OfCategory(CateDict[i]) && perm.blocks.Any(d =>
                            d.IsAAA() && player.GetCombinator().ASuccB(d.tiles[0], c.tiles[0], true, 3) &&
                            d.OfCategory(CateDict[i])))))
                    return true;
            }

            return false;
        }

        private static bool VerifyDinSanKe(Permutation perm, Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (perm.blocks.Any(b => b.IsAAA() && b.OfCategory(CateDict[i]) && perm.blocks.Any(c =>
                        c.IsAAA() && player.GetCombinator().ASuccB(c.tiles[0], b.tiles[0], true, 4) &&
                        c.OfCategory(CateDict[i]) && perm.blocks.Any(d =>
                            d.IsAAA() && player.GetCombinator().ASuccB(d.tiles[0], c.tiles[0], true, 4) &&
                            d.OfCategory(CateDict[i])))))
                    return true;
            }

            return false;
        }

        private static bool VerifyQingDongMen(Permutation perm, Player player)
        {
            List<Tile> tiles = new Hand("248p12346z").tiles;
            List<Tile> candTiles = perm.ToTiles();
            if (!candTiles.Any(t => t.CompactWith(new Tile("6z")))) return false;
            if (candTiles.All(t => t.CompactWith(new Tile("6z")))) return false;
            return VerifyForAllTiles(perm, player, t => tiles.Any(t.CompactWith));
        }

        private static bool VerifyHeiYiSe(Permutation perm, Player player)
        {
            List<Tile> tiles = new Hand("248p1234z").tiles;
            List<Tile> candTiles = perm.ToTiles();
            return candTiles.All(t => tiles.Any(t2 => t.CompactWith(t2)));
        }

        private static bool VerifyHongKongQue(Permutation perm, Player player)
        {
            List<Tile> tiles = new Hand("1579s7z").tiles;
            List<Tile> candTiles = perm.ToTiles();
            return tiles.All(t => candTiles.Any(t2 => t.CompactWith(t2))) &&
                   VerifyForAllTiles(perm, player, t => tiles.Any(t2 => t.CompactWith(t2)));
        }

        private static bool VerifyChunQuanDaiYao(Permutation perm, Player player)
        {
            return VerifyQuanDaiYao(perm, player) && VerifyWuZi(perm, player);
        }

        private static bool VerifyJiangDui(Permutation perm, Player player)
        {
            return VerifyForAllTiles(perm, player, t => t.IsNumbered(2) || t.IsNumbered(5) || t.IsNumbered(8));
        }

        private static bool VerifySiXiDui(Permutation permutation, Player player)
        {
            if (permutation.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> blocks = permutation.blocks.ToList();
            blocks.Add(new PairBlock(new[] { permutation.jiang.tile1, permutation.jiang.tile2 }));
            for (int i = 1; i < 5; i++)
            {
                Tile tile = new Tile(i + "z");
                if (blocks.All(b => !b.All(t => t.CompactWith(tile)))) return false;
            }

            return true;
        }

        private static bool VerifyQiXingDui(Permutation permutation, Player player)
        {
            if (permutation.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> blocks = permutation.blocks.ToList();
            blocks.Add(new PairBlock(new[] { permutation.jiang.tile1, permutation.jiang.tile2 }));
            for (int i = 1; i < 8; i++)
            {
                Tile tile = new Tile(i + "z");
                if (blocks.All(b => !b.All(t => t.CompactWith(tile)))) return false;
            }

            return true;
        }

        private static bool VerifySanYuanDui(Permutation permutation, Player player)
        {
            if (permutation.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> blocks = permutation.blocks.ToList();
            blocks.Add(new PairBlock(new[] { permutation.jiang.tile1, permutation.jiang.tile2 }));
            for (int i = 5; i < 8; i++)
            {
                Tile tile = new Tile(i + "z");
                if (blocks.All(b => !b.All(t => t.CompactWith(tile)))) return false;
            }

            return true;
        }

        private static bool VerifyJinMenQiao(Permutation perm, Player player)
        {
            int step = 2;
            for (int i = 0; i < 3; i++)
            {
                if (VerifyForFourBlocks(perm, player,
                        a => a.IsABC() && a.OfCategory(CateDict[i]),
                        (a, b) => b.IsABC() && player.GetCombinator().ASuccB(b.tiles[0], a.tiles[0], true, step) &&
                                  b.OfCategory(CateDict[i]),
                        (_, b, c) => c.IsABC() && player.GetCombinator().ASuccB(c.tiles[0], b.tiles[0], true, step) &&
                                     c.OfCategory(CateDict[i]),
                        (_, _, c, d) =>
                            d.IsABC() && player.GetCombinator().ASuccB(d.tiles[0], c.tiles[0], true, step) &&
                            d.OfCategory(CateDict[i])
                    ))
                {
                    return true;
                }
            }

            return false;
        }

        private static List<Tile> tilesToHighlight; 

        public static bool TestYaku(Permutation permutation, YakuType yaku, Player status, out List<Tile> relatedTiles)
        {
            tilesToHighlight = permutation.ToTiles();
            Func<Permutation, Player, bool> Predicate;
            YAKUS_PREDICATE_MAP.TryGetValue(yaku, out Predicate);
            if (Predicate != null)
            {
                bool testYaku = Predicate.Invoke(permutation, status);
                relatedTiles = new (tilesToHighlight);
                return testYaku;
            }
            relatedTiles = new (tilesToHighlight);
            return false;
        }

        public static bool IncludeYaku(YakuType a, YakuType b)
        {
            return GetYakuChilds(a).Contains(b);
        }

        public static double GetFan(YakuType yaku, int blockCount, SkillSet skillSet, int extraFan = 0)
        {
            return (skillSet.CalculateFan(yaku, blockCount, extraFan));
        }

        private static HashSet<YakuType> GetYakuChilds(YakuType yaku)
        {
            HashSet<YakuType> yakus = new() { yaku };
            foreach (YakuType child in InfoMap[yaku].includedYakus)
            {
                yakus.UnionWith(GetYakuChilds(child));
            }

            return yakus;
        }

        public static HashSet<YakuType> GetYakuChildsExcludeSelf(YakuType yaku)
        {
            HashSet<YakuType> yakus = GetYakuChilds(yaku);
            yakus.Remove(yaku);
            return yakus;
        }

        public static string GetColoredYakuName(YakuType yaku, Rarity rarity)
        {
            string color = rarity switch
            {
                Rarity.COMMON => "#FFFFFF",
                Rarity.RARE => "#0000FF",
                Rarity.EPIC => "#FF00FF",
                Rarity.LEGENDARY => "#FFA500",
                Rarity.ANCIENT => "#FF0000",
                _ => throw new ArgumentOutOfRangeException("Invalid rarity")
            };
            return $"<color={color}>{YakuLocalizationManager.GetYakuName(InfoMap[yaku])}</color>";
        }


        //番种验证函数
        private static bool VerifyPingHu(Permutation permutation, Player player)
        {
            if (player.deck.regName.Equals(MahjongDeck.BambooDeck.regName))
            {
                if (permutation.jiang.All(t => t.CompactWith(new(player.GetPrevalentWind() + "z")))) return false;
                if (permutation.jiang.All(t => t.CompactWith(new(player.GetPlayerWind() + "z")))) return false;
                if (permutation.jiang.All(t => t.CompactWithCategory(Tile.Category.Jian))) return false;
                return permutation.BlocksFulfillAll(block => block.IsABC());
            }

            return permutation.BlocksFulfillAll(block => block.IsABC()) &&
                   permutation.TilesFulfullAll(t => t.IsNumbered());
        }

        private static bool VerifySanSeTongShun(Permutation permutation, Player status)
        {
            return VerifyForThreeBlocks(permutation, status, a => a.IsABC() && a.OfCategory(Tile.Category.Wan),
                (a, b) => b.IsABC() && b.OfCategory(Tile.Category.Suo) && b.OfSameOrder(a),
                (_, b, c) => c.IsABC() && c.OfCategory(Tile.Category.Bing) && c.OfSameOrder(b));
        }


        private static bool VerifyYiSeSanTongShun(Permutation permutation, Player status)
        {
            return permutation.blocks.Any(b => 
                b.IsNumbered() && 
                b.IsABC() && 
                VerifyForBlockCount(permutation, status, b2 => status.DetermineShiftedPair(b, b2, 0, true), 3));
        }

        //新的YiSeSiTongShun的verify
        private static bool VerifyYiSeSiTongShun(Permutation permutation, Player status)
        {
            return permutation.blocks.Any(b => 
                b.IsNumbered() && 
                b.IsABC() && 
                VerifyForBlockCount(permutation, status, b2 => status.DetermineShiftedPair(b, b2, 0, true), 4));
        }

        private static bool VerifyQuanDaiWu(Permutation permutation, Player status)
        {
            //TODO: 高亮关联牌
            return permutation.BlocksFulfillAll(b => b.IsNumbered() && b.Any(t => t.GetOrder() == 5))
                   && permutation.JiangFulfillAll(t => t.IsNumbered() && t.GetOrder() == 5);
        }

        private static bool VerifyDaYuWu(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, t => t.IsNumbered() && t.GetOrder() > 5);
        }

        private static bool VerifyXiaoYuWu(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status,t => t.IsNumbered() && t.GetOrder() < 5);
        }

        private static bool VerifyQuanDa(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status,a => a.IsNumbered() && a.GetOrder() <= 9 && a.GetOrder() >= 7);
        }

        private static bool VerifyQuanZhong(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status,a => a.IsNumbered() && a.GetOrder() <= 6 && a.GetOrder() >= 4);
        }

        private static bool VerifyQuanXiao(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status,a => a.IsNumbered() && a.GetOrder() <= 3 && a.GetOrder() >= 1);
        }

        private static bool VerifyYiBanGao(Permutation permutation, Player status)
        {
            return VerifyForTwoBlocks(permutation, status, a => a.IsNumbered() && a.IsABC(),
                (a, b) => status.DetermineShiftedPair(b, a, 0, true));
        }

        private static bool VerifyLiangBanGao(Permutation permutation, Player status)
        {
            return VerifyForFourBlocks(permutation, status,
                a => a.IsABC(),
                (a, b) => a.CompactWith(b),
                (_, _, c) => c.IsABC(),
                (_, _, c, d) => d.CompactWith(c));
        }

        private static bool VerifyQingYiSe(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status,tile =>
                       tile.CompactWithCategory(Tile.Category.Suo)) ||
                   VerifyForAllTiles(permutation, status,tile =>
                       tile.CompactWithCategory(Tile.Category.Bing)) ||
                   VerifyForAllTiles(permutation, status,tile =>
                       tile.CompactWithCategory(Tile.Category.Wan));
        }

        private static bool VerifyHunYiSe(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status,tile =>
                       tile.IsHonor(status) ||
                       tile.CompactWithCategory(Tile.Category.Suo)) ||
                   VerifyForAllTiles(permutation, status,tile =>
                       tile.IsHonor(status) ||
                       tile.CompactWithCategory(Tile.Category.Bing)) ||
                   VerifyForAllTiles(permutation, status,tile =>
                       tile.IsHonor(status) ||
                       tile.CompactWithCategory(Tile.Category.Wan));
        }

        private static bool VerifyWuMenQi(Permutation permutation, Player status)
        {
            Tile.Category[] categories =
                { Tile.Category.Wan, Tile.Category.Suo, Tile.Category.Bing, Tile.Category.Feng, Tile.Category.Jian };
            return categories.All(c =>
                permutation.blocks.Any(b => b.GetCategory() == c) ||
                permutation.JiangFulfillAll(j => j.GetCategory() == c));
        }

        private static bool VerifyShuangAnKe(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAA(), 2);
        }

        private static bool VerifySanAnKe(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAA(), 3);
        }

        private static bool VerifySiAnKe(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAA(), 4);
        }

        private static bool VerifyGang(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAAA(), 1);
        }

        private static bool VerifyShuangGang(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAAA(), 2);
        }

        private static bool VerifySanGang(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAAA(), 3);
        }

        private static bool VerifySiGang(Permutation permutation, Player status)
        {
            return VerifyForBlockCount(permutation, status, b => b.IsAAAA(), 4);
        }

        private static bool VerifyDuanYaoJiu(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, a => !a.IsYaoJiu(status));
        }

        private static bool VerifyLianLiu(Permutation permutation, Player status)
        {
            return VerifyForTwoBlocks(permutation, status,
                a => a.IsABC(), (a, b) => b.IsABC() && status.GetCombinator().ASuccB(a.tiles[0], b.tiles[2])
            );
        }

        private static bool VerifyXiXiangFeng(Permutation permutation, Player status)
        {
            return VerifyForTwoBlocks(permutation, status, a => a.IsABC() && a.IsNumbered(),
                (a, b) => b.IsNumbered() && !a.OfSameCategory(b) && a.OfSameOrder(b));
        }

        private static bool VerifyWuZi(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, t => !t.IsHonor(status));
        }

        private static bool VerifyQueYiMen(Permutation permutation, Player status)
        {
            return (VerifyForAllTiles(permutation, status, a => a.GetCategory() != Tile.Category.Wan))
                   || (VerifyForAllTiles(permutation, status, a => a.GetCategory() != Tile.Category.Suo))
                   || (VerifyForAllTiles(permutation, status, a => a.GetCategory() != Tile.Category.Bing));
        }

        private static bool VerifyZiYiSe(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, a => a.IsHonor(status));
        }

        private static bool VerifyJianKe(Permutation permutation, Player status)
        {
            return VerifyBlock(permutation, status, a => a.IsAAA() && a.OfCategory(Tile.Category.Jian));
        }

        private static bool VerifyMenFengKe(Permutation permutation, Player status)
        {
            return VerifyBlock(permutation, status, a => a.IsAAA() && a.All(t => t.IsPlayerWind(status)));
        }

        private static bool VerifyQuanFengKe(Permutation permutation, Player status)
        {
            return VerifyBlock(permutation, status, a => a.IsAAA() && a.All(t => t.IsPrevalentWind(status)));
        }

        private static bool VerifySanFengKe(Permutation permutation, Player status)
        {
            for (int i = 1; i < 5; i++)
            {
                int j = (i) % 4 + 1;
                int k = (j) % 4 + 1;
                if (VerifyForThreeBlocks(
                        permutation, 
                        status, 
                        b => b.CompactWith(new Block($"{i}{i}{i}z")),
                        ((_, b1) => b1.CompactWith(new Block($"{j}{j}{j}z"))),
                        ((_, _, b2) => b2.CompactWith(new Block($"{k}{k}{k}z")))
                        ))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyShuangJianKe(Permutation permutation, Player status)
        {
            //TODO
            for (int i = 5; i < 8; i++)
            {
                int j = ((i - 5 + 1) % 3) + 5;
                if (permutation.blocks.Any(a => a.CompactWith(new(i.ToString() + i + i + "z"))
                                                && permutation.blocks.Any(b => b != a &&
                                                                               b.CompactWith(new(j.ToString() + j + j +
                                                                                   "z"))
                                                )
                    )
                   )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyHunYaoJiu(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, a => a.IsYaoJiu(status));
        }

        private static bool VerifyXiaoSanYuan(Permutation permutation, Player status)
        {
            //TODO
            for (int i = 5; i < 8; i++)
            {
                int j = ((i - 4) % 3) + 5;
                int k = ((j - 4) % 3) + 5;
                if (permutation.blocks.Any(a => a.CompactWith(new(i.ToString() + i + i + "z"))
                                                && permutation.blocks.Any(b => b != a &&
                                                                               b.CompactWith(new(j.ToString() + j + j +
                                                                                   "z"))
                                                )
                    ) && permutation.jiang.All(a => a.CompactWith(new Tile(k + "z")))
                   )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyXiaoSiXi(Permutation permutation, Player status)
        {
            //TODO
            for (int i = 1; i < 5; i++)
            {
                int j = (i) % 4 + 1;
                int k = (j) % 4 + 1;
                int l = (k) % 4 + 1;
                if (permutation.blocks.Any(a => a.CompactWith(new(i.ToString() + i + i + "z"))
                                                && permutation.blocks.Any(b => b != a &&
                                                                               b.CompactWith(new(j.ToString() + j + j +
                                                                                   "z"))
                                                                               && permutation.blocks.Any(c => c != a &&
                                                                                   c != b &&
                                                                                   c.CompactWith(new(k.ToString() + k +
                                                                                       k + "z"))
                                                                               )
                                                )
                    ) && permutation.jiang.All(a => a.CompactWith(new(l + "z")))
                   )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyShuangTongZiKe(Permutation permutation, Player status)
        {
            return VerifyForTwoBlocks(permutation, status, a => a.IsHonor(status) && a.IsAAA(),
                (a, b) => b.IsHonor(status) && b.IsAAA() && b.CompactWith(a));
        }

        private static bool VerifyDaSanYuan(Permutation permutation, Player status)
        {
            //TODO
            return permutation.blocks.Any(a => a.CompactWith(new("555z"))
                                               && permutation.blocks.Any(b => a != b && b.CompactWith(new Block("666z"))
                                                   && permutation.blocks.Any(c =>
                                                       c != b && c != a && c.CompactWith(new Block("777z")))));
        }

        private static bool VerifyDaSiXi(Permutation permutation, Player status)
        {
            return VerifyForFourBlocks(permutation, status, (a => a.IsAAA() && a.All(t => t.CompactWith(new("1z")))),
                (_, b) => b.IsAAA() && b.All(t => t.CompactWith(new("2z"))),
                (_, _, c) => c.IsAAA() && c.All(t => t.CompactWith(new("3z"))),
                (_, _, _, d) => d.IsAAA() && d.All(t => t.CompactWith(new("4z"))));
        }

        private static bool VerifyQingYaoJiu(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, a => !a.IsHonor(status) && (a.IsYaoJiu(status)));
        }

        private static bool VerifySanTongZiKe(Permutation permutation, Player status)
        {
            return VerifyForThreeBlocks(permutation, status,
                a => a.IsHonor(status) && a.IsAAA(),
                (a, b) => b.IsHonor(status) && b.IsAAA() && b.CompactWith(a),
                (a, _, c) => c.IsHonor(status) && c.IsAAA() && c.CompactWith(a));
        }

        private static bool VerifySiTongZiKe(Permutation permutation, Player status)
        {
            return VerifyForFourBlocks(permutation, status,
                a => a.IsHonor(status) && a.IsAAA(),
                (a, b) => b.IsHonor(status) && b.IsAAA() && b.CompactWith(a),
                (a, _, c) => c.IsHonor(status) && c.IsAAA() && c.CompactWith(a),
                (a, _, _, d) => d.IsHonor(status) && d.IsAAA() && d.CompactWith(a));
        }

        private static bool VerifyHuaLong(Permutation permutation, Player status)
        {
            Dictionary<int, Tile.Category> dict = new()
            {
                { 0, Tile.Category.Wan },
                { 1, Tile.Category.Suo },
                { 2, Tile.Category.Bing }
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        if (VerifyForThreeBlocks(permutation, status, a => a.IsABC() && a.OfCategory(dict[i]),
                                (a, b) => b.IsABC() && b.OfCategory(dict[j]) &&
                                          status.GetCombinator().ASuccB(b.tiles[0], a.tiles[2], false),
                                (_, b, c) =>
                                    c.IsABC() && c.OfCategory(dict[k]) &&
                                    status.GetCombinator().ASuccB(c.tiles[0], b.tiles[2], false))
                           )
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool VerifyQingLong(Permutation permutation, Player status)
        {
            Dictionary<int, Tile.Category> dict = new()
            {
                { 0, Tile.Category.Wan },
                { 1, Tile.Category.Suo },
                { 2, Tile.Category.Bing }
            };

            for (int i = 0; i < 3; i++)
            {
                Tile.Category cat = dict.GetValueOrDefault(i);
                if (VerifyForThreeBlocks(permutation, status, a => a.IsABC() && a.OfCategory(cat),
                        (a, b) => b.IsABC() && b.OfCategory(cat) &&
                                  status.GetCombinator().ASuccB(b.tiles[0], a.tiles[2]),
                        (_, b, c) => c.IsABC() && c.OfCategory(cat) &&
                                     status.GetCombinator().ASuccB(c.tiles[0], b.tiles[2]))
                   )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyQuanDaiYao(Permutation permutation, Player player)
        {
            return permutation.blocks.All(b => b.Any(t => t.IsYaoJiu(player))) &&
                   permutation.jiang.All(t => t.IsYaoJiu(player));
        }

        private static bool VerifyLyuYiSe(Permutation permutation, Player status)
        {
            return VerifyForAllTiles(permutation, status, t => t.FulfillAllGreen(status));
        }

        private static bool VerifyJiuLianBaoDeng(Permutation permutation, Player status)
        {
            if (permutation.blocks.Any(b => b.IsAAAA())) return false;
            if (VerifyQingYiSe(permutation, status))
            {
                Dictionary<int, int> dict = new();
                for (int i = 1; i < 10; i++)
                {
                    dict.Add(i, 0);
                }

                permutation.ToTiles().ForEach(t => dict[t.GetOrder()]++);
                if (dict[1] >= 3 && dict[2] >= 1 && dict[3] >= 1 && dict[4] >= 1 && dict[5] >= 1 && dict[6] >= 1
                    && dict[7] >= 1 && dict[8] >= 1 && dict[9] >= 3)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifySanSeSanBuGao(Permutation perm, Player status)
        {
            return VerifyForThreeBlocks(perm, status,
                a => a.IsNumbered() && a.IsABC(),
                (a, b) => b.IsNumbered() && b.IsABC() && status.DetermineShiftedPair(a, b, 1, false),
                (a, b, c) => c.IsNumbered() && c.IsABC()
                                            && new[] { a.GetCategory(), b.GetCategory(), c.GetCategory() }.Distinct()
                                                .Count() == 3
                                            && status.DetermineShiftedPair(b, c, 1, false));
        }

        private static bool VerifySanSeSanJieGao(Permutation perm, Player status)
        {
            return VerifyForThreeBlocks(perm, status,
                a => a.IsNumbered() && a.IsAAA(),
                (a, b) => b.IsNumbered() && b.IsAAA() && status.DetermineShiftedPair(a, b, 1, false),
                (a, b, c) => c.IsNumbered() && c.IsAAA()
                                            && new[] { a.GetCategory(), b.GetCategory(), c.GetCategory() }.Distinct()
                                                .Count() == 3
                                            && status.DetermineShiftedPair(b, c, 1, false));
        }

        private static bool VerifyDuanHong(Permutation perm, Player player)
        {
            return VerifyForAllTiles(perm, player, t => !t.ContainsRed(player));
        }

        private static bool VerifyYiSeSanBuGao(Permutation perm, Player status)
        {
            for (int step = 1; step <= 2; step++)
            {
                if (VerifyForThreeBlocks(perm, status,
                        a => a.IsNumbered() && a.IsABC(),
                        (a, b) => status.DetermineShiftedPair(a, b, step, true),
                        (_, b, c) => status.DetermineShiftedPair(b, c, step, true)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyLaoShaoFu(Permutation permutation, Player status)
        {
            for (int i = 0; i < 3; i++)
            {
                if (VerifyForTwoBlocks(permutation,
                        status,
                        block => block.CompactWithNumbers("123"),
                        (b1, b2) => b2.CompactWithNumbers("789") && b2.OfSameCategory(b1)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasLaoShaoFu(Tile.Category category, Permutation perm)
        {
            return perm.blocks.Any(a => a.IsABC() && a.OfCategory(category) && a.tiles[0].GetOrder() == 1 &&
                                        perm.blocks.Any(b =>
                                            a != b && b.IsABC() && b.OfCategory(category) &&
                                            b.tiles[0].GetOrder() == 7));
        }

        private static bool VerifySanSeShuangLongHui(Permutation permutation, Player status)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        if (HasLaoShaoFu(CateDict[i], permutation)
                            && HasLaoShaoFu(CateDict[j], permutation)
                            && permutation.JiangFulfillAll(t =>
                                t.CompactWithCategory(CateDict[k]) && t.GetOrder() == 5))
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool VerifyYiSeShuangLongHui(Permutation perm, Player status)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!HasLaoShaoFu(CateDict[i], perm)) continue;
                if (perm.blocks
                        .Count(b => b.IsABC() && b.OfCategory(CateDict[i]) && b.tiles[0].GetOrder() == 1) == 2
                    && perm.blocks
                        .Count(b => b.IsABC() && b.OfCategory(CateDict[i]) && b.tiles[0].GetOrder() == 7) == 2
                    && perm.JiangFulfillAll(t => t.CompactWithCategory(CateDict[i]) && t.GetOrder() == 5))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyLianQiDui(Permutation perm, Player status)
        {
            if (perm.GetPermType() != PermutationType.SEVEN_PAIRS) return false;
            List<Block> visitedBlocks = new List<Block>();
            List<Block> allBlocks = perm.blocks.ToList();
            allBlocks.Add(new PairBlock(new[] { perm.jiang.tile1, perm.jiang.tile2 }));
            Block startBlock = allBlocks.FirstOrDefault(b =>
                allBlocks.All(b2 => !status.GetCombinator().APredB(b2.tiles[0], b.tiles[0])));

            if (startBlock == null) return false;

            visitedBlocks.Add(startBlock);

            Block nextBlock = allBlocks.FirstOrDefault(b =>
                !visitedBlocks.Contains(b) && status.GetCombinator().APredB(startBlock.tiles[0], b.tiles[0]));
            while (nextBlock != null)
            {
                visitedBlocks.Add(nextBlock);
                nextBlock = allBlocks.FirstOrDefault(b =>
                    !visitedBlocks.Contains(b) && status.GetCombinator().APredB(nextBlock.tiles[0], b.tiles[0]));
            }

            return visitedBlocks.Count == 7;
        }

        private static bool VerifyTuiBuDao(Permutation perm, Player status)
        {
            return VerifyForAllTiles(perm, status, t => t.IsRotationalSymmetric());
        }

        private static bool VerifyYiSeSanJieGao(Permutation perm, Player status)
        {
            if (VerifyForThreeBlocks(perm, status,
                    a => a.IsNumbered() && a.IsAAA(),
                    (a, b) => status.DetermineShiftedPair(a, b, 1, true),
                    (_, b, c) => status.DetermineShiftedPair(b, c, 1, true)))
            {
                return true;
            }

            return false;
        }

        private static bool VerifyYiSeSiJieGao(Permutation perm, Player status)
        {
            if (VerifyForFourBlocks(perm, status,
                    a => a.IsNumbered() && a.IsAAA(),
                    (a, b) => status.DetermineShiftedPair(a, b, 1, true),
                    (_, b, c) => status.DetermineShiftedPair(b, c, 1, true),
                    (_, _, c, d) => status.DetermineShiftedPair(c, d, 1, true)))
            {
                return true;
            }

            return false;
        }

        private static bool VerifySiGuiYi(Permutation perm, Player status)
        {
            return VerifyMultipleTileCopies(perm, 4);
        }

        private static bool VerifyWuGuiYi(Permutation perm, Player status)
        {
            return VerifyMultipleTileCopies(perm, 5);
        }

        private static bool VerifyDuoGuiYi(Permutation perm, Player status)
        {
            return VerifyMultipleTileCopies(perm, 6);
        }

        private static bool VerifyMultipleTileCopies(Permutation perm, int count)
        {
            return perm.ToTiles().Any(t =>
            {
                var sameTiles = perm.ToTiles().Where(t.CompactWith).ToList();
                bool res = sameTiles.Count >= count;
                if(res) tilesToHighlight = sameTiles;
                return res;
            });
        }

        private static bool VerifyShuangSeShuangTongKe(Permutation perm, Player status)
        {
            return VerifyForTwoBlocks(perm, status, b1 => b1.IsNumbered() && b1.IsAAA(),
                (b1, b2) => b2.IsMirageOf(b1, status) && !b1.OfSameCategory(b2));
        }

        private static bool VerifySanSeSanTongKe(Permutation perm, Player status)
        {
            return VerifyForThreeBlocks(perm, status, b1 => b1.IsNumbered() && b1.IsAAA(),
                (b1, b2) => b2.IsMirageOf(b1, status) && !b1.OfSameCategory(b2),
                (b1, b2, b3) => b3.IsMirageOf(b1, status) && !b1.OfSameCategory(b3) && !b2.OfSameCategory(b3));
        }

        private static bool VerifyQuanShuangKe(Permutation perm, Player status)
        {
            return VerifyForAllTiles(perm, status, t => t.IsNumbered() && t.GetOrder() % 2 == 0);
        }

        private static bool VerifyYiSeShuangTongKe(Permutation perm, Player status)
        {
            return perm.blocks.Any(b => 
                b.IsNumbered() && 
                b.IsAAA() && 
                VerifyForBlockCount(perm, status, b2 => b.IsMirageOf(b2, status, true), 2));
        }

        private static bool VerifyYiSeSanTongKe(Permutation perm, Player status)
        {
            return perm.blocks.Any(b => 
                b.IsNumbered() && 
                b.IsAAA() && 
                VerifyForBlockCount(perm, status, b2 => b.IsMirageOf(b2, status, true), 3));
        }

        private static bool VerifyYiSeSiTongKe(Permutation perm, Player status)
        {
            return perm.blocks.Any(b => 
                b.IsNumbered() && 
                b.IsAAA() && 
                VerifyForBlockCount(perm, status, b2 => b.IsMirageOf(b2, status, true), 4));
        }

        private static bool VerifyYiSeSiBuGao(Permutation perm, Player status)
        {
            for (int step = 1; step <= 2; step++)
            {
                if (VerifyForFourBlocks(perm, status,
                        a => a.IsABC(),
                        (a, b) => status.DetermineShiftedPair(a, b, step, true),
                        (_, b, c) => status.DetermineShiftedPair(b, c, step, true),
                        (_, _, c, d) => status.DetermineShiftedPair(c, d, step, true))
                   )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VerifyJingTongShun(Permutation perm, Player status)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (VerifyForFourBlocks(perm, status,
                            a => a.IsABC() && a.OfCategory(CateDict[i]),
                            (a, b) => b.IsABC() && b.OfCategory(CateDict[j]) && b.OfSameOrder(a),
                            (_, _, c) => c.IsABC() && c.OfCategory(CateDict[i]),
                            (_, _, c, d) => d.IsABC() && d.OfCategory(CateDict[j]) && c.OfSameOrder(d)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool VerifyJingTongKe(Permutation perm, Player status)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (VerifyForFourBlocks(perm, status,
                            a => a.IsAAA() && a.OfCategory(CateDict[i]),
                            (a, b) => b.IsAAA() && b.OfCategory(CateDict[j]) && b.OfSameOrder(a),
                            (_, _, c) => c.IsAAA() && c.OfCategory(CateDict[i]),
                            (_, _, c, d) => d.IsAAA() && d.OfCategory(CateDict[j]) && c.OfSameOrder(d)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool VerifyBaiWanShi(Permutation perm, Player status)
        {
            return perm.ToTiles().All(a => a.CompactWithCategory(Tile.Category.Wan)) &&
                   perm.ToTiles().Sum(t => t.GetOrder()) >= 100;
        }

        private static bool VerifyQiDui(Permutation perm, Player _)
        {
            PermutationType permutationType = perm.GetPermType();
            return permutationType == PermutationType.SEVEN_PAIRS;
        }

        private static bool VerifyThirteenOrphan(Permutation perm, Player _)
        {
            return perm.GetPermType() == PermutationType.THIRTEEN_ORPHANS;
        }

        private static bool VerifyForAllTiles(Permutation perm, Player status, Func<Tile, bool> pred)
        {
            var permTiles = perm.ToTiles();
            tilesToHighlight = permTiles.Union(status.GetHandDeckCopy()).Where(pred).ToList();
            return permTiles.All(t => tilesToHighlight.Contains(t));
        }

        private static bool VerifyForBlockCount(Permutation perm, Player status, Func<Block, bool> pred, int num)
        {
            var blocks = perm.blocks.Where(pred).ToArray();
            if (blocks.Length < num) return false;
            tilesToHighlight = blocks.SelectMany(b => b.tiles).ToList();
            return true;
        }
        
        private static bool VerifyBlock(Permutation perm, Player status, Func<Block, bool> pred)
        {
            return VerifyForBlockCount(perm, status, pred, 1);
        }

        private static bool VerifyForFourBlocks(Permutation perm, Player status, Func<Block, bool> pred1,
            Func<Block, Block, bool> pred2, Func<Block, Block, Block, bool> pred3,
            Func<Block, Block, Block, Block, bool> pred4)
        {
            return perm.blocks.Any(a => pred1(a)
                                        && perm.blocks.Any(b => b != a && pred2(a, b)
                                                                       && perm.blocks.Any(c => c != a && c != b &&
                                                                           pred3(a, b, c)
                                                                           && perm.blocks.Any(d =>
                                                                               {
                                                                                   if (d != a && d != b && d != c &&
                                                                                        pred4(a, b, c, d))
                                                                                   {
                                                                                       tilesToHighlight =
                                                                                           a.tiles.Union(b.tiles)
                                                                                               .Union(c.tiles)
                                                                                               .Union(d.tiles)
                                                                                               .ToList();
                                                                                       return true;
                                                                                   }
                                                                                   return false;
                                                                               }
                                                                               ))));
        }

        private static bool VerifyForThreeBlocks(Permutation perm, Player status, Func<Block, bool> pred1,
            Func<Block, Block, bool> pred2, Func<Block, Block, Block, bool> pred3)
        {
            return perm.blocks.Any(a => pred1(a)
                                        && perm.blocks.Any(b => b != a && pred2(a, b)
                                                                       && perm.blocks.Any(c =>
                                                                           {
                                                                               if (c != a && c != b && pred3(a, b, c))
                                                                               {
                                                                                   tilesToHighlight =
                                                                                       a.tiles.Union(b.tiles)
                                                                                           .Union(c.tiles).ToList();
                                                                                      return true;
                                                                               }
                                                                               return false;
                                                                           }
                                                                           )));
        }

        private static bool VerifyForTwoBlocks(Permutation perm, Player status, Func<Block, bool> pred1,
            Func<Block, Block, bool> pred2)
        {
            return perm.blocks.Any(a => pred1(a)
                                        && perm.blocks.Any(b =>
                                        {
                                            if (b != a && pred2(a, b))
                                            {
                                                tilesToHighlight = a.tiles.Union(b.tiles).ToList();
                                                return true;
                                            }
                                            return false;
                                        }));
        }

        private static bool VerifyForOneBlock(Permutation perm, Player status, Func<Block, bool> pred1)
        {
            return perm.blocks.Any(a =>
            {
                bool b = pred1(a);
                if(b)
                    tilesToHighlight = a.tiles.ToList();
                return b;
            });
        }

        public static int GetUnlockRequirement(YakuType yaku)
        {
            return Math.Abs(InfoMap[yaku].growthFactor - 1) < 0.01f ? 2 : 4;
        }
    }
}