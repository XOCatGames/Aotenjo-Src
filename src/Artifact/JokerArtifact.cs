using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 仅全部选中的刻子且不是杠才产生金钱；雀头同类时金额翻倍，保持单次收益而非重复效果。
    /// </summary>
    public sealed class JokerArtifact : Artifact
    {
        public JokerArtifact() : base("joker", Rarity.COMMON)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.Any(a => !player.Selecting(a))) return;
            if (!block.IsAAA() || player.GetCombinator().IsKong(block)) return;
            int amount = permutation.JiangFulfillAll(t => t.IsSameCategory(block.tiles[0]))? 2 : 1;
            effects.Add(new EarnMoneyEffect(amount, this));
        }
    }
}
