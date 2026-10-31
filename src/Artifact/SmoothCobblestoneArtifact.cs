using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 只对全部仍在手牌中的刻子追加弃牌次数奖励；保留牌对象的引用判断，避免旧面子重复触发。
    /// </summary>
    public sealed class SmoothCobblestoneArtifact : Artifact
    {
        public SmoothCobblestoneArtifact() : base("smooth_cobblestone", Rarity.COMMON)
        {
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if (block.IsAAA() && block.All(t => player.GetHandDeckCopy().Contains(t)))
            {
                effects.Add(new IncreDiscardEffect("cobblestone_shining", this, 3));
            }
        }
    }
}
