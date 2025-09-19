using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Aotenjo
{
    public class Tips
    {
        public static readonly Tips[] TIPS_LIST =
        {
            Create("pair"),
            Create("end_game"),
            Create("play_quad"),
            Create("final_hand"),
            Create("dayuwu_xiaoyuwu"),
            Create("epic_material"),
            Create("tax"),
            Create("change_pair"),
            Create("double_category"),
            Create("taichiscroll"),
            Create("gadget_combination"),
            Create("red_marker"),
            Create("neon"),
            Create("seven_pairs"),
            Create("common_and_rare_pattern"),
            Create("boss_level"),
            Create("upgrade_pattern"),
            Create("interest"),
            Create("price"),
            Create("gadget"),
            Create("sequence"),
            Create("variety"),
            Create("pivot"),
            Create("explore")
        };

        private string key;
        private Predicate<Player> conditions;

        private Tips(string key)
        {
            this.key = key;
            conditions = _ => true;
        }

        public Tips SetConditions(Predicate<Player> conditions)
        {
            this.conditions = conditions;
            return this;
        }

        public string GetText(Func<string, string> loc)
        {
            return loc($"tips_{key}");
        }

        public bool IsAvailable(Player player)
        {
            return conditions(player);
        }

        public static Tips Create(string key)
        {
            return new Tips(key);
        }

        public static string GetRandomTips(Player player, Func<string, string> loc)
        {
            Tips tips = TIPS_LIST.Where(t => t.IsAvailable(player)).OrderBy(_ => Random.value).FirstOrDefault();
            if (tips == null)
            {
                return "";
            }

            return tips.GetText(loc);
        }
    }
}