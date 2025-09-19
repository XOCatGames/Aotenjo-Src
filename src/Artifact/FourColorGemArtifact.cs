using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class FourColorGemArtifact : Artifact, ICountable
    {
        public const int MUL = 3;

        protected List<int> triggeredCategories = new List<int>();

        public FourColorGemArtifact() : base("four_color_gem", Rarity.RARE)
        {
        }

        public override void Deserialize(string data)
        {
            base.Deserialize(data);
            triggeredCategories = new List<int>();
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            triggeredCategories = new List<int>();
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            triggeredCategories.Clear();
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL, GetCategoryStyledName(0, localizer),
                GetCategoryStyledName(1, localizer),
                GetCategoryStyledName(2, localizer), GetCategoryStyledName(3, localizer));
        }

        private string GetCategoryStyledName(int index, Func<string, string> localizer)
        {
            if (triggeredCategories.Contains(index))
            {
                return localizer($"yakucategory_{index}_name");
            }

            return localizer($"yakucategory_{index}_name_grey");
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            int id_added_predicted = 0;
            List<YakuType> yakuTypes =
                permutation.GetYakus(player).Where(y => player.GetSkillSet().GetLevel(y) > 0).ToList();
            List<YakuPack> yakuPacks = GameManager.Instance.yakuPacks;
            HashSet<int> idList = new HashSet<int>();

            foreach (YakuType type in yakuTypes)
            {
                foreach (YakuPack pack in yakuPacks)
                {
                    if (pack.ContainsYaku(YakuTester.InfoMap[type]))
                    {
                        idList.Add(pack.id);
                    }
                }
            }

            foreach (int id in idList)
            {
                if (!triggeredCategories.Contains(id))
                {
                    effects.Add(new FourColorGemEffect(this, id));
                    id_added_predicted++;
                }
            }

            if (id_added_predicted + triggeredCategories.Count() == 4)
            {
                effects.Add(ScoreEffect.MulFan(MUL, this));
            }
        }

        public int GetMaxCounter()
        {
            return 4;
        }

        public int GetCurrentCounter()
        {
            return 4 - triggeredCategories.Count();
        }

        private class FourColorGemEffect : Effect
        {
            private FourColorGemArtifact fourColorGem;
            private int id;

            public FourColorGemEffect(FourColorGemArtifact fourColorGem, int id)
            {
                this.fourColorGem = fourColorGem;
                this.id = id;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func($"effect_four_color_gem_{id}");
            }

            public override Artifact GetEffectSource()
            {
                return fourColorGem;
            }

            public override void Ingest(Player player)
            {
                fourColorGem.triggeredCategories.Add(id);
            }
        }
    }
}