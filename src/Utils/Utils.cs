using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Aotenjo
{
    public static class Utils
    {
        public static double SmoothStep(double from, double to, double t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        public static int GetAotenjoBonus(double mul)
        {
            double extraMul = Math.Floor(mul - 1);
            int bonus = 0;
            if (extraMul <= 4D)
            {
                bonus += (int)(extraMul * 1);
                return bonus;
            }

            bonus += 4 * 1;
            extraMul -= 4D;
            extraMul /= 4D;

            double v = Math.Log(3D * extraMul + 1D);
            double d = v / Math.Log(4D);
            bonus += (int)Math.Floor(d);
            return bonus;
        }
        
        public static Yaku GetYakuDefinition(this YakuType type)
        {
            return YakuTester.InfoMap[type];
        }

        public static int CountPairs(Permutation permutation, Func<Block, Block, bool> pred)
        {
            return FindPairs(permutation, pred).Count;
        }
        
        public static List<(Block, Block)> FindPairs(Permutation permutation, Func<Block, Block, bool> pred)
        {
            List<(Block, Block)> lst = new List<(Block, Block)>();

            foreach (Block block1 in permutation.blocks)
            {
                foreach (Block block2 in permutation.blocks)
                {
                    if (block1 == block2 || lst.Any(p =>
                            p.Item1 == block1 && p.Item2 == block2 || p.Item1 == block2 && p.Item2 == block1))
                    {
                    }
                    else if (pred(block1, block2) || pred(block2, block1))
                    {
                        lst.Add((block1, block2));
                    }
                }
            }
            
            lst.Sort((b1, b2) => -(b1.Item1.tiles.Union(b1.Item2.tiles).Min()
                .CompareTo(b2.Item1.tiles.Union(b2.Item2.tiles).Min())));

            return lst;
        }
        
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    // 如果不是首字符，前面加下划线
                    if (i > 0)
                        result.Append('_');
                    result.Append(char.ToLower(c));
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static Yaku ToYaku(this YakuType type)
        {
            return YakuTester.InfoMap[type];
        }


        //public static string NumberToChineseFormat(double num)
        //{
        //    if (num < Math.Pow(10, 8)) return NumberToFormat(num);

        //    Dictionary<int, string> exponentRange = new Dictionary<int, string>()
        //    {
        //        {8, "亿"},
        //        {12, "兆"},
        //        {16, "京"},
        //        {20, "垓"},
        //        {24, "秭"},
        //        {28, "穰"},
        //        {32, "沟"},
        //        {36, "涧"},
        //        {40, "正"},
        //        {44, "载"},
        //        {48, "极"},
        //        {52, "恒河沙"},
        //        {56, "阿僧祇"},
        //        {60, "那由他"},
        //        {64, "不可思议"},
        //        {68, "无量大数"}
        //    };
        //}

        public static string NumberToFormat(double num, int digitsToScientificNotation = 8)
        {
            return (num > Math.Pow(10, digitsToScientificNotation) ? num.ToString("0.0E0") : num.ToShortString());
        }

        public static string NumberToFormat(long num)
        {
            return (num > 100000000D ? num.ToString("0.0E0") : num.ToShortString());
        }

        public static string NumberToFormat(float num)
        {
            return (num > 100000000D ? num.ToString("0.0E0") : num.ToShortString());
        }

        public static Color WithA(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }

        public static Vector3 IncreY(this Vector3 c, float a)
        {
            return new(c.x, a, c.z);
        }

        public static int TextLength(string richText)
        {
            int len = 0;
            bool inTag = false;

            foreach (var ch in richText)
            {
                if (ch == '<')
                {
                    inTag = true;
                }
                else if (ch == '>')
                {
                    inTag = false;
                }
                else if (!inTag)
                {
                    len++;
                }
            }

            return len;
        }

        public static bool IsSevenPairYaku(Yaku yaku)
        {
            YakuType typeID = yaku.GetYakuType();
            return typeID == FixedYakuType.QiDui || typeID == FixedYakuType.LianQiDui || typeID == FixedYakuType.DaCheLun
                   || typeID == FixedYakuType.XiaoCheLun || typeID == FixedYakuType.DaShuLin || typeID == FixedYakuType.XiaoShuLin
                   || typeID == FixedYakuType.DaZhuLin || typeID == FixedYakuType.XiaoZhuLin || typeID == FixedYakuType.QiXingDui
                   || typeID == FixedYakuType.SanYuanDui || typeID == FixedYakuType.SiXiDui || typeID == FixedYakuType.QiTongDui
                   || typeID == FixedYakuType.LongQiDui || typeID == FixedYakuType.ShuangLongQiDui ||
                   typeID == FixedYakuType.SanLongQiDui;
        }

        public static bool IsKongYaku(Yaku yaku)
        {
            YakuType typeID = yaku.GetYakuType();
            return typeID == FixedYakuType.Gang || typeID == FixedYakuType.ShuangGang || typeID == FixedYakuType.SanGang
                   || typeID == FixedYakuType.SiGang || typeID == FixedYakuType.TianDiChuangZao || typeID == FixedYakuType.WuGang;
        }
    }
}