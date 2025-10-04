using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class DaqianOrizuluArtifact : CraftableArtifact
    {
        public DaqianOrizuluArtifact() : base("daqian_orizulu", Rarity.RARE)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (permutation == null || !permutation.JiangFulfillAll(t => t.IsNumbered() && t.GetOrder() > 6)) return;
            effects.Add(new SimpleEffect("effect_daqian", this, p =>
            {
                List<Yaku> yakus = p.deck.GetAvailableYakus().Where(y => y.rarity == Rarity.COMMON).ToList();
                EventManager.Instance.OnSoundEvent("book");
                foreach (var yaku in yakus)
                {
                    p.GetSkillSet().AddLevel(yaku.yakuTypeID, 2);
                }
            }, "Book"));
        }
    }
}