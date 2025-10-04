using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class XiaoqianOrizuluArtifact : CraftableArtifact
    {
        public XiaoqianOrizuluArtifact() : base("xiaoqian_orizulu", Rarity.RARE)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (permutation == null || !permutation.JiangFulfillAll(t => t.IsNumbered() && t.GetOrder() < 4)) return;
            effects.Add(new SimpleEffect("effect_xiaoqian", this, p =>
            {
                List<Yaku> yakus = p.deck.GetAvailableYakus().Where(y => y.rarity == Rarity.RARE).ToList();
                MessageManager.Instance.OnSoundEvent("book");
                p.GetSkillSet().AddLevel(yakus.OrderByDescending(y => p.stats.GetYakuCount(y)).First().GetYakuType(),
                    1);
            }, "Book"));
        }
    }
}