using System;
using System.Collections.Generic;
using System.Text;

namespace Aotenjo
{
    public class ShredderArtifact : LevelingArtifact, IFanProvider
    {
        public ShredderArtifact() : base("shredder", Rarity.EPIC, 0)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFanFormat(Level);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            StringBuilder sb = new(string.Format(base.GetDescription(localizer), GetFan(player)));
            sb.AppendLine("");
            string format = localizer("artifact_shredder_limit");
            if (Level is >= 15 and < 30)
            {
                sb.Append(string.Format(format, localizer("rarity_common_name")));
            }
            else if (Level >= 30)
            {
                sb.Append(string.Format(format, localizer("rarity_rare_name")));
            }

            return sb.ToString();
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            if (Level > 0)
            {
                effects.Add(ScoreEffect.AddFan(GetFan(player), this));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.DeleteYakuEvent += OnDeleteYaku;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DeleteYakuEvent -= OnDeleteYaku;
        }

        private void OnDeleteYaku(PlayerYakuEvent.Delete eventData)
        {
            Player player = eventData.player;
            Yaku yaku = YakuTester.InfoMap[eventData.yakuType];

            if (yaku.rarity < Rarity.EPIC && Level > ((int)yaku.rarity + 1) * 15)
            {
                return;
            }

            float baseFan = yaku.rarity switch
            {
                Rarity.COMMON => 1f,
                Rarity.RARE => 3f,
                Rarity.EPIC => 6f,
                Rarity.LEGENDARY => 12f,
                Rarity.ANCIENT => 24f,
                _ => 0
            };
            int levelIncrease =
                (int)Math.Floor(baseFan * ((Math.Log(player.GetSkillSet().GetLevel(yaku)) / Math.Log(2)) + 1));
            Level += levelIncrease;
        }

        public double GetFan(Player player)
        {
            return Level;
        }
    }
}