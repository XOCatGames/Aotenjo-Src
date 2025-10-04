using System.Collections.Generic;

namespace Aotenjo
{
    public class GoldenSeedArtifact : CountingArtifact, IPersistantAura
    {
        private const double MUL_PER_LEVEL = 0.5D;
        private const int MAX_PROC = 4;
        private bool affecting;
        private int level;

        public GoldenSeedArtifact() : base(3, "golden_seed", Rarity.EPIC)
        {
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(GetMul(), this));
        }

        private double GetMul()
        {
            return 1 + level * MUL_PER_LEVEL;
        }
        
        public override string GetDescription(Player player, System.Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), GetMul());
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            level = 0;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.EarnMoneyEvent += OnEarnMoney;
            player.PreRoundStartEvent += OnRoundStart;
            player.PostPreRoundEndEvent += OnRoundEnd;
            affecting = player.inRound;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.EarnMoneyEvent -= OnEarnMoney;
            player.PreRoundStartEvent -= OnRoundStart;
            player.PrePreRoundEndEvent -= OnRoundEnd;
        }

        private void OnRoundEnd(PlayerEvent playerEvent)
        {
            affecting = false;
            level = 0;
            currentCounter = 2;
        }

        private void OnRoundStart(PlayerEvent playerEvent)
        {
            affecting = true;
            level = 0;
            currentCounter = 2;
        }

        private void OnEarnMoney(PlayerMoneyEvent evt)
        {
            if (!affecting) return;
            int amount = evt.amount;
            if (amount > 0 && level < MAX_PROC)
            {
                currentCounter -= amount;
                GainArtifact(evt.player);

                if (level == MAX_PROC)
                {
                    currentCounter = 0;
                }
            }
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            affecting = player.inRound;
        }

        private void GainArtifact(Player player)
        {
            while (currentCounter < 0 && level < MAX_PROC)
            {
                level++;
                currentCounter += 3;
                List<Artifact> list = player.DrawRandomArtifact(Rarity.EPIC, 1);
                list.RemoveAll(a => a.IsUnique());
                if (list.Count == 0) return;
                Artifact tempArtifact = LotteryPool<Artifact>.DrawFromCollection(list, player.GenerateRandomInt);
                tempArtifact.IsTemporary = true;
                player.ObtainArtifact(tempArtifact);
                EventManager.Instance.OnSoundEvent("Agate");
            }
        }

        public bool IsAffecting(Player player)
        {
            return affecting;
        }

        public override int GetCurrentCounter()
        {
            if (level == MAX_PROC) return 0;
            return base.GetCurrentCounter() + 1;
        }
    }
}