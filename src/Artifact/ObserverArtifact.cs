using System;

namespace Aotenjo
{
    public class ObserverArtifact : LevelingArtifact, IFuProvider
    {
        public double lastRoundTriggers;
        public const double FU = 5;
        
        public ObserverArtifact() : base("observer", Rarity.EPIC, 0)
        {
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            lastRoundTriggers = 0;
        }

        public override string Serialize()
        {
            return base.Serialize() + "," + lastRoundTriggers;
        }

        public override void Deserialize(string data)
        {
            var parts = data.Split(new[] { ',' });
            base.Deserialize(parts[0]);
            lastRoundTriggers = int.Parse(parts[1]);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFuFormat(GetFu(player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), lastRoundTriggers, FU);
        }

        [SubscribeToEvent]
        private void OnPostAddScoringAnimationEffect(PlayerEvents.OnPostAddScoringAnimationEffectEvent eventData)
        {
            Player player = eventData.player;
            eventData.effects.Add(ScoreEffect.AddFu(() =>
            {
                lastRoundTriggers = GetFu(player);
                return GetFu(player);
            }, this));
        }

        public double GetFu(Player player)
        {
            return FU * Level;
        }

        [SubscribeToEvent]
        private void OnPreAppendSettleScoringEffects(PlayerEvents.PreAppendSettleScoringEffectsEvent eventData)
        {
            Level = 0;
        }

        [SubscribeToEvent]
        private void OnPostIngest(PlayerEvents.PostIngestEffectEvent eventData)
        {
            Level++;
        }

        [SubscribeToEvent]
        private void PostRoundEnd(PlayerRoundEvent.End.Post eventData)
        {
            Level = 0;
        }
    }
}