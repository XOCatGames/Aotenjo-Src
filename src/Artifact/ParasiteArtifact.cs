namespace Aotenjo
{
    public class ParasiteArtifact : Artifact
    {
        public const double Multiplier = 1.1D;
        public const double BlockMultiplier = 1.3D;

        public ParasiteArtifact() : base("parasite", Rarity.EPIC)
        {
        }

        // “吮”的故事暂不配置；移除占位键后保持故事区域为空，避免显示缺失的本地化键。
        public override string GetStory(System.Func<string, string> loc) => string.Empty;

        [SubscribeToEvent]
        private void OnEffectTriggered(PlayerEvents.PostIngestEffectEvent eventData)
        {
            Player player = eventData.player;
            var artifacts = eventData.artifactsAtTrigger;
            int index = artifacts.IndexOf(this);
            if (index <= 0 || !player.GetArtifacts().Contains(this) || player.IsArtifactDebuffed(this)) return;

            // Preserve the left neighbor even if its own effect consumes/removes it.
            if (eventData.effect.GetEffectSource() == artifacts[index - 1])
            {
                double multiplier = eventData.effect.HasTag(EffectTag.BlockScoring) ? BlockMultiplier : Multiplier;
                eventData.followingEffects.Add(ScoreEffect.MulFan(multiplier, this));
            }
        }
    }
}
