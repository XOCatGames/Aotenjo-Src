using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 杠先翻倍单次符值，再按幺九条件追加独立效果；不能合并次数，否则会改变寄生体和动画触发。
    /// </summary>
    public sealed class AncientScriptureArtifact : Artifact
    {
        public AncientScriptureArtifact() : base("ancient_scripture", Rarity.RARE)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            bool isKong = player.GetCombinator().IsKong(block);
            if (!block.IsAAA() && !isKong) return;
            double amount = 20;
            if (isKong) amount *= 2;
            int triggerCount = block.All(t => t.IsYaoJiu(player)) ? 2 : 1;
            for (int trigger = 0; trigger < triggerCount; trigger++)
                effects.Add(ScoreEffect.AddFu(amount, this));
        }
    }
}
