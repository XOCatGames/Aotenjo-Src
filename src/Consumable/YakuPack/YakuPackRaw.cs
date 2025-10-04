using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public class YakuPackRaw
    {
        public int id;
        public string name;

        public List<string> common;
        public List<string> rare;
        public List<string> epic;
        public List<string> legendary;
        public List<string> ancient;

        public List<int> weights = new List<int>();

        public YakuPack ToYakuPack()
        {
            YakuPack yakuPack = new();
            yakuPack.id = id;
            yakuPack.name = name;
            yakuPack.common = parseStringYakus(common);
            yakuPack.rare = parseStringYakus(rare);
            yakuPack.epic = parseStringYakus(epic);
            yakuPack.legendary = parseStringYakus(legendary);
            yakuPack.ancient = parseStringYakus(ancient);
            yakuPack.weights = new(weights);

            return yakuPack;
        }

        private static List<YakuType> parseStringYakus(List<string> yakus)
        {
            if (yakus == null)
            {
                return new List<YakuType>();
            }

            //Debug.Log(yakus.ToCommaSeparatedString());
            List<YakuType> yakuList = new List<YakuType>();
            foreach (string yaku in yakus)
            {
                if (!Enum.GetNames(typeof(YakuType)).Contains(yaku))
                {
                    Debug.LogError("Cannot find Yaku " + yaku);
                    continue;
                }

                yakuList.Add(Enum.Parse<YakuType>(yaku));
            }

            return yakuList;
        }
    }
}