using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 比较当前选中面子与结算雀头的数字牌类别；只需一个不同类别面子就触发一次倍率。
    /// </summary>
    public sealed class BicolorOrizuluArtifact : Artifact
    {
        public BicolorOrizuluArtifact() : base("bicolor_orizulu", Rarity.RARE)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (player.GetCurrentSelectedBlocks().Any(b =>
                    b.All(t => t.IsNumbered()) && permutation.jiang.All(t => t.IsNumbered())
                        && b.GetCategory() != permutation.jiang.GetCategory()))

                effects.Add(ScoreEffect.MulFan(2, this));
        }
    }
}
