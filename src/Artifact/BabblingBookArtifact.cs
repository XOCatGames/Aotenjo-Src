using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class BabblingBookArtifact : Artifact, IBook
    {
        public BabblingBookArtifact() : base("babbling_book", Rarity.COMMON)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new UpgradeRandomYakuEffect(this));
        }

        private class UpgradeRandomYakuEffect : Effect
        {
            private readonly BabblingBookArtifact artifact;

            public UpgradeRandomYakuEffect(BabblingBookArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_babbling_book");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override bool NoDefaultSound()
            {
                return true;
            }

            public override void Ingest(Player player)
            {
                artifact.UpgradeYaku(player);
                MessageManager.Instance.OnSoundEvent("book");
            }
        }


        public List<Yaku> GetYakuPool(Player player)
        {
            return player.deck.GetAvailableYakus();
        }

        public List<Yaku> DrawYakusToUpgrade(Player player)
        {
            List<Yaku> yakus = GetYakuPool(player);
            Yaku yaku = yakus[player.GenerateRandomInt(yakus.Count)];
            return new List<Yaku> {yaku};
        }
    }
}