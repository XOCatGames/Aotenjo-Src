using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 按已学习且属于指定番种类别的番种数量追加弃牌次数；数量为零时不生成空奖励。
    /// </summary>
    public sealed class PalmFanArtifact : Artifact
    {
        public PalmFanArtifact() : base("palm_fan", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            int amount = permutation.GetYakus(player).Count(y => player.GetSkillSet().GetLevel(y) > 0 && YakuTester.InfoMap[y].GetYakuCategories().Contains(1));
            if(amount != 0)
                effects.Add(new IncreDiscardEffect("palm_fan", this, amount));
        }
    }
}
