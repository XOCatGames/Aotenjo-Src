using System;

namespace Aotenjo
{
    public class ExtractionForcepsArtifact : Artifact, IPersistantAura
    {
        public double removedTargets;

        public ExtractionForcepsArtifact() : base("extraction_forceps", Rarity.EPIC)
        {
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            removedTargets = 0;
        }

        public override string GetDescription(Func<string, string> func)
        {
            return string.Format(base.GetDescription(func), Utils.NumberToFormat(removedTargets));
        }

        public override string Serialize()
        {
            return removedTargets.ToString();
        }

        public override void Deserialize(string s)
        {
            removedTargets = long.Parse(s);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostRoundStartEvent += DecreaseTarget;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostRoundStartEvent -= DecreaseTarget;
        }

        private void DecreaseTarget(PlayerEvent player)
        {
            if (player.player.Level % 4 == 0)
            {
                removedTargets += (player.player.levelTarget * 0.5d);
                player.player.levelTarget *= 0.5d;
                EventManager.Instance.OnSetProgressBarLength(0.5f);
            }
        }

        public bool IsAffecting(Player player)
        {
            return player.Level % 4 == 0;
        }
    }
}