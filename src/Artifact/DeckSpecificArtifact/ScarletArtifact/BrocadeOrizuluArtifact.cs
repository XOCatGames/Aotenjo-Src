using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BrocadeOrizuluArtifact : Artifact
    {
        private const int MAIN_AMOUNT = 3; //若雀头为主花色，获得金币量
        private const int NOT_MAIN_AMOUNT = -1; //若雀头不为主花色，获得金币量

        public BrocadeOrizuluArtifact() : base("brocade_orizulu", Rarity.COMMON)
        {
            
            SetHighlightRequirement((t, p) => ((ScarletPlayer)p).IsCompatibleWithMainCategory(t.GetCategory()));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation perm, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, perm, effects);

            if (player is not ScarletPlayer scarlet) return;

            bool isMainCategory = perm.JiangFulfillAll(t => scarlet.IsCompatibleWithMainCategory(t.GetCategory())
            );
            effects.Add(new EarnMoneyEffect(isMainCategory ? MAIN_AMOUNT : NOT_MAIN_AMOUNT, this));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MAIN_AMOUNT, NOT_MAIN_AMOUNT);
        }
    }
}