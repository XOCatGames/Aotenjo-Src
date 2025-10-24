using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GoldenScissorArtifact : Artifact
    {
        public GoldenScissorArtifact() : base("golden_scissor", Rarity.COMMON)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new GoldenEffect(this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostSkipRoundEvent += Golden;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostSkipRoundEvent -= Golden;
        }

        private void Golden(PlayerEvent eventData)
        {
            Player player = eventData.player;
            List<Tile> cands = player.GetHandDeckCopy().Where(t =>
                (t.IsNumbered() || t.IsHonor(player)) && t.CompatWithMaterial(TileMaterial.PLAIN, player)).ToList();
            if (cands.Count == 0) return;
            Tile tile = cands[player.GenerateRandomInt(cands.Count())];
            tile.SetMaterial(TileMaterial.GOLDEN, player);
        }


        private class GoldenEffect : TextEffect
        {
            public GoldenEffect(Artifact source) : base("effect_golden", source, "Agate")
            {
            }

            public override void Ingest(Player player)
            {
                List<Tile> targets = player.GetHandDeckCopy().Where(t =>
                    (t.IsNumbered() || t.IsHonor(player)) &&
                    t.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName()).ToList();
                if (targets.Count == 0) return;
                LotteryPool<Tile> pool = new();
                pool.AddRange(targets);
                pool.Draw(player.GenerateRandomInt).SetMaterial(TileMaterial.GOLDEN, player);
            }
        }
    }
}