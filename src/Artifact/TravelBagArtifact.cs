using System.Collections.Generic;

namespace Aotenjo
{
    public class TravelBagArtifact : Artifact
    {
        public TravelBagArtifact() : base("travel_bag", Rarity.RARE)
        {
        }

        public override bool IsUnique()
        {
            return true;
        }

        public override bool CanBeSellByPlayer()
        {
            return false;
        }

        public override bool CanBeBoughtWithoutSlotLimit(Player player)
        {
            return true;
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            player.SetArtifactLimit(player.GetArtifactLimit() + 2);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnAddSingleAnimationEffectEvent += PostAddScoringEffect;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnAddSingleAnimationEffectEvent -= PostAddScoringEffect;
        }

        private void PostAddScoringEffect(Player player, List<IAnimationEffect> lst, IAnimationEffect arg4)
        {
            lst.RemoveAll(effect =>
            {
                Effect IEffect = effect.GetEffect();
                if (IEffect is not ScoreEffect) return false;
                Artifact artifact = IEffect.GetEffectSource();
                if (artifact == null || artifact.GetNameID() == null || artifact.GetNameID() == "") return false;
                return (player.IsArtifactDebuffed(artifact));
            });
        }
    }
}