using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aotenjo
{
    public abstract class BaseLockArtifact : LevelingArtifact
    {
        protected const int UPGRADE_LEVELS = 1;
        private const int LEVELS_PER_CIRCLE = 4;
        private YakuType[] pendingYakus;

        protected abstract double FanMultiplier { get; }
        protected virtual int TaskTarget => 0;
        protected virtual string NextLockId => null;
        protected abstract int ShopCircle { get; }

        protected BaseLockArtifact(string name, Rarity rarity) : base(name, rarity, 0)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            // Rebinding must not duplicate subscriptions or complete a settlement twice.
            UnsubscribeLockEvents(player);
            EventBus.Subscribe<PlayerEvents.RetrieveYakuMultiplierEvent>(player, PlayerOnRetrieveYakuMultiplierEvent);
            EventBus.Subscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, CaptureActivatedYakus);
            EventBus.Subscribe<PlayerEvents.PostSettlePermutationEvent>(player, CompleteSettlement);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            UnsubscribeLockEvents(player);
            pendingYakus = null;
        }

        private void UnsubscribeLockEvents(Player player)
        {
            EventBus.Unsubscribe<PlayerEvents.RetrieveYakuMultiplierEvent>(player, PlayerOnRetrieveYakuMultiplierEvent);
            EventBus.Unsubscribe<PlayerEvents.PreAppendSettleScoringEffectsEvent>(player, CaptureActivatedYakus);
            EventBus.Unsubscribe<PlayerEvents.PostSettlePermutationEvent>(player, CompleteSettlement);
        }

        private void PlayerOnRetrieveYakuMultiplierEvent(PlayerYakuEvent.RetrieveMultiplier yakuEvent)
        {
            Yaku yaku = yakuEvent.yakuType.GetYakuDefinition();
            if (yaku.rarity >= Rarity.RARE) yakuEvent.canceled = true;
        }

        public override bool CanObtainBy(Player player) =>
            base.CanObtainBy(player) && !player.GetArtifacts().Any(artifact => artifact is BaseLockArtifact);

        public override bool IsAvailableInShops(Player player)
        {
            if (player == null || !base.IsAvailableInShops(player) || !CanObtainBy(player)) return false;
            int circle = (Math.Max(1, player.Level) - 1) / LEVELS_PER_CIRCLE + 1;
            return ShopCircle == 3 ? circle >= ShopCircle : circle == ShopCircle;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(FanMultiplier, this));
        }

        private void CaptureActivatedYakus(PlayerEvents.PreAppendSettleScoringEffectsEvent evt)
        {
            // Capture the scored set before end-of-settlement unlocks or other relics modify the hand.
            pendingYakus = GetActivatedCommonYakus(evt.player, evt.permutation);
        }

        protected static YakuType[] GetActivatedCommonYakus(Player player, Permutation permutation) =>
            permutation?.GetYakus(player)
                .Where(type => type.GetYakuDefinition().rarity == Rarity.COMMON &&
                               player.GetSkillSet().GetLevel(type) > 0)
                .Distinct().ToArray() ?? Array.Empty<YakuType>();

        private void CompleteSettlement(PlayerEvents.PostSettlePermutationEvent evt)
        {
            YakuType[] activated = pendingYakus;
            pendingYakus = null;
            if (activated == null || activated.Length == 0) return;

            if (TaskTarget == 0) return;
            Level = Math.Min(TaskTarget, Level + activated.Length);
            if (Level >= TaskTarget) Evolve(evt.player);
        }

        protected void AddUpgradeEffect(List<IAnimationEffect> effects, IEnumerable<YakuType> yakus)
        {
            // Keep the round-end hand's candidates fixed while the queued animation plays.
            YakuType[] targets = yakus.ToArray();
            if (targets.Length == 0) return;
            effects.Add(new SimpleEffect("effect_lock_cultivate", this, player =>
            {
                foreach (YakuType yaku in targets) player.UpgradeYaku(yaku, UPGRADE_LEVELS);
            }));
        }

        private void Evolve(Player player)
        {
            int index = player.GetArtifacts().IndexOf(this);
            if (NextLockId == null || index < 0 || player.HasPendingArtifactReplacement(this)) return;
            Artifact next = player.GetAvailableArtifact(NextLockId);
            MessageManager.Instance.OnActivateArtifactEvent(this, new TextEffect("effect_lock_evolve", this));
            player.RequestArtifactReplacement(this, next);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player) =>
            TaskTarget > 0 ? ($"{{0}}/{TaskTarget}", Level) : base.GetAdditionalDisplayingInfo(player);

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            pendingYakus = null;
        }

        public override void Deserialize(string data)
        {
            if (!int.TryParse(data, NumberStyles.Integer, CultureInfo.InvariantCulture, out int progress) ||
                progress < 0 || (TaskTarget > 0 ? progress > TaskTarget : progress != 0))
                throw new FormatException("Invalid lock task progress.");
            Level = progress;
            pendingYakus = null;
        }
    }
}
