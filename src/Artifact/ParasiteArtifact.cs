namespace Aotenjo
{
    public class ParasiteArtifact : Artifact
    {
        public const double Multiplier = 1.1D;

        public ParasiteArtifact() : base("parasite", Rarity.EPIC)
        {
        }

        [SubscribeToEvent]
        private void OnEffectTriggered(PlayerEvents.PostIngestEffectEvent eventData)
        {
            Player player = eventData.player;
            var artifacts = eventData.artifactsAtTrigger;
            int index = artifacts.IndexOf(this);
            if (index <= 0 || !player.GetArtifacts().Contains(this) || player.IsArtifactDebuffed(this)) return;

            // Preserve the left neighbor even if its own effect consumes/removes it.
            if (eventData.effect.GetEffectSource() == artifacts[index - 1])
                eventData.followingEffects.Add(ScoreEffect.MulFan(Multiplier, this));
        }
    }
}
