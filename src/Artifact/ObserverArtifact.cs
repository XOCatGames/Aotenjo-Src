using System;
using System.Collections.Generic;

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

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerEvents.PostIngestEffectEvent>(player, OnPostIngest);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, OnPreAppendSettleScoringEffects);
            EventBus.Subscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, OnPostAddScoringAnimationEffect);
        }

        private void OnPostAddScoringAnimationEffect(Permutation permutation, Player player,
            List<IAnimationEffect> list)
        {
            list.Add(ScoreEffect.AddFu(() =>
            {
                lastRoundTriggers = GetFu(player);
                return GetFu(player);
            }, this));
        }

        public double GetFu(Player player)
        {
            return FU * Level;
        }

        private void OnPreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            Level = 0;
        }

        private void OnPostIngest(Permutation permutation, Player player, Effect effect)
        {
            Level++;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerEvents.PostIngestEffectEvent>(player, OnPostIngest);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, OnPreAppendSettleScoringEffects);
            EventBus.Unsubscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, OnPostAddScoringAnimationEffect);
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            Level = 0;
        }
    }
}