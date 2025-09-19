using System.Collections.Generic;

namespace Aotenjo
{
    public class BronzePocketWatchArtifact : Artifact
    {
        private bool used = false;
        public BronzePocketWatchArtifact() : base("bronze_pocket_watch", Rarity.COMMON)
        {
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            used = false;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnPostAddRoundEndAnimationEffectEvent += OnPostAddRoundEndAnimationEffect;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnPostAddRoundEndAnimationEffectEvent -= OnPostAddRoundEndAnimationEffect;
        }

        public void OnPostAddRoundEndAnimationEffect(Permutation permutation, Player player,
            List<IAnimationEffect> list)
        {
            if (used) return;
            list.Add(new TextEffect("effect_bronze_pocket_watch", this));
            list.Add(new SilentEffect(() => used = true));
            list.Add(new RoundEndEffectAppendEffect(player.roundEndEffectsStack, player, permutation));
            list.Add(new SilentEffect(() => used = false));
        }
    }
}