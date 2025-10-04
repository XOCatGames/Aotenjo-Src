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
            YakuPack yakuPack = new()
            {
                id = id,
                name = name,
                common = ParseStringYakus(common),
                rare = ParseStringYakus(rare),
                epic = ParseStringYakus(epic),
                legendary = ParseStringYakus(legendary),
                ancient = ParseStringYakus(ancient),
                weights = new List<int>(weights)
            };

            return yakuPack;
        }

        private static List<YakuType> ParseStringYakus(List<string> yakus)
        {
            return yakus == null ? new List<YakuType>() : yakus.Select(YakuType.FromString).ToList();
        }
    }
}