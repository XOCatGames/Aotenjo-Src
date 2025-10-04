using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TeleportingOrizuluArtifact : SneakyArtifact, IActivable
    {
        public List<Tile> prevPair;
        public bool first;

        public TeleportingOrizuluArtifact() : base("teleporting_orizulu", Rarity.RARE)
        {
            prevPair = new List<Tile>();
            first = true;
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            prevPair = new List<Tile>();
            first = true;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostRoundStartEvent += PostRoundStart;
            player.PreAppendSettleScoringEffectsEvent += PreAppendSettleScoringEffects;
        }


        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostRoundStartEvent -= PostRoundStart;
            player.PreAppendSettleScoringEffectsEvent -= PreAppendSettleScoringEffects;
        }

        private void PreAppendSettleScoringEffects(PlayerPermutationEvent permutationEvent)
        {
            var prevPerm = permutationEvent.player.GetAccumulatedPermutation();
            if (prevPerm == null) return;
            prevPair = new List<Tile>
            {
                prevPerm.jiang.tile1,
                prevPerm.jiang.tile2
            };
        }
        
        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            if (permutation.GetPermType() == PermutationType.NORMAL && first && permutation.blocks.Count() == 2)
                effects.Add(new TeleportingEffect(this));
        }

        private void PostRoundStart(PlayerEvent playerEvent)
        {
            prevPair = new List<Tile>();
            first = true;
        }

        public bool IsActivating()
        {
            return first;
        }

        private class TeleportingEffect : Effect
        {
            private readonly TeleportingOrizuluArtifact orizulu;

            public TeleportingEffect(TeleportingOrizuluArtifact teleportingOrizuluArtifact)
            {
                orizulu = teleportingOrizuluArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_teleporting_orizulu");
            }

            public override Artifact GetEffectSource()
            {
                return orizulu;
            }

            public override void Ingest(Player player)
            {
                if (!orizulu.first) return;
                if (orizulu.prevPair.Count != 2) return;
                foreach (var item in orizulu.prevPair)
                {
                    ((SneakyPlayer)player).SneakTile(item);
                    player.RemoveTileFromDiscarded(item);
                }
                
                orizulu.prevPair.Clear();

                orizulu.first = false;
            }
        }
    }
}