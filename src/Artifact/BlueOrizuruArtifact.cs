using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 仅雀头原始类别全部为饼子时追加金钱效果；收益在效果结算时发放。
    /// </summary>
    public sealed class BlueOrizuruArtifact : Artifact
    {
        public BlueOrizuruArtifact() : base("blue_orizulu", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (permutation.JiangFulfillAll(t => t.GetCategory() == Tile.Category.Bing))
                effects.Add(new EarnMoneyEffect(1, this));
        }
    }
}
