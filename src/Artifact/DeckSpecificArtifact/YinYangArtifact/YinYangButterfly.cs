using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class YinYangButterflyArtifact : Artifact
    {
        private bool usedThisRound;

        public YinYangButterflyArtifact() : base("yin_yang_butterfly", Rarity.EPIC)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreRoundStartEvent += OnPreRoundStart;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreRoundStartEvent -= OnPreRoundStart;
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            usedThisRound = false;
        }

        private void OnPreRoundStart(PlayerEvent _)
        {
            usedThisRound = false;
        }

        public override void AddOnBlockEffects(Player player, Permutation perm, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, perm, block, effects);

            if (!block.IsYinSeq() || !block.Any(player.Selecting)) return;

            if (usedThisRound) return;

            var cat = block.GetCategory();
            int midOrder = block.tiles[1].GetOrder();

            var targets = player.GetAccumulatedPermutation()?
                              .blocks
                              .Where(b => b.IsYangSeq() &&
                                          b.GetCategory() == cat &&
                                          b.tiles[1].GetOrder() == midOrder)
                              .ToList()
                          ?? new List<Block>();

            if (targets.Count == 0) return;

            effects.Add(new EraseYangSeqEffect(this, targets));
            usedThisRound = true;
        }

        private sealed class EraseYangSeqEffect : Effect
        {
            private readonly YinYangButterflyArtifact artifact;
            private readonly List<Block> targets;

            public EraseYangSeqEffect(YinYangButterflyArtifact art, List<Block> targets)
            {
                artifact = art;
                this.targets = targets;
            }

            public override void Ingest(Player player)
            {
                Permutation perm = player.GetAccumulatedPermutation();
                if (perm == null) return;

                var comb = player.GetCombinator();

                perm.blocks = perm.blocks
                    .Where(b => !targets.Any(t => comb.IsIdentical(b, t)))
                    .ToArray();

                if (perm.blocks.Length == 0 ||
                    perm.GetPermType() == PermutationType.SEVEN_PAIRS)
                {
                    player.SetCurrentAccumulatedBlock(null);
                }

                bool needUnfreeze = player.CurrentPlayingStage == 4;
                player.CurrentPlayingStage = Math.Max(0, player.CurrentPlayingStage - targets.Count);
                if (needUnfreeze)
                    EventManager.Instance.OnUnfreezeEvent(player);
            }

            public override string GetEffectDisplay(Func<string, string> f)
                => f("effect_yin_yang_butterfly");

            public override Artifact GetEffectSource() => artifact;
        }
    }
}