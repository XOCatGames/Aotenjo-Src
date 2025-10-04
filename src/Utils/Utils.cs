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
            int count = 0;

            List<Pair<Block, Block>> lst = new List<Pair<Block, Block>>();

            foreach (Block block1 in permutation.blocks)
            {
                foreach (Block block2 in permutation.blocks)
                {
                    if (block1 == block2 || lst.Any(p =>
                            p.elem1 == block1 && p.elem2 == block2 || p.elem1 == block2 && p.elem2 == block1))
                    {
                    }
                    else if (pred(block1, block2) || pred(block2, block1))
                    {
                        count++;
                        lst.Add(new(block1, block2));
                    }
                }
            }

            return count;
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
            YakuType typeID = yaku.yakuTypeID;
            return typeID == YakuType.QiDui || typeID == YakuType.LianQiDui || typeID == YakuType.DaCheLun
                   || typeID == YakuType.XiaoCheLun || typeID == YakuType.DaShuLin || typeID == YakuType.XiaoShuLin
                   || typeID == YakuType.DaZhuLin || typeID == YakuType.XiaoZhuLin || typeID == YakuType.QiXingDui
                   || typeID == YakuType.SanYuanDui || typeID == YakuType.SiXiDui || typeID == YakuType.QiTongDui
                   || typeID == YakuType.LongQiDui || typeID == YakuType.ShuangLongQiDui ||
                   typeID == YakuType.SanLongQiDui;
        }

        public static bool IsKongYaku(Yaku yaku)
        {
            YakuType typeID = yaku.yakuTypeID;
            return typeID == YakuType.Gang || typeID == YakuType.ShuangGang || typeID == YakuType.SanGang
                   || typeID == YakuType.SiGang || typeID == YakuType.TianDiChuangZao;
        }

        public static Vector2 WorldToCanvasPosition(Canvas canvas, RectTransform canvasRect, Camera camera,
            Vector3 position)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out Vector2 result);

            return canvas.transform.TransformPoint(result);
        }
    }
}