using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PorcelainScissorArtifact : Artifact
    {
        public PorcelainScissorArtifact() : base("porcelain_scissor", Rarity.RARE)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostSkipRoundEndEvent += Bone;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostSkipRoundEndEvent -= Bone;
        }

        private void Bone(PlayerEvent eventData)
        {
            Player player = eventData.player;
            List<Tile> cands = player.GetHandDeckCopy().Where(t => t.CompactWithMaterial(TileMaterial.PLAIN, player))
                .ToList();
            if (cands.Count == 0) return;
            Tile tile1 = cands[player.GenerateRandomInt(cands.Count())];
            tile1.SetMaterial(TileMaterial.BonePorcelain(), player);
            cands.Remove(tile1);
            if (cands.Count == 0) return;
            Tile tile2 = cands[player.GenerateRandomInt(cands.Count())];
            tile2.SetMaterial(TileMaterial.BonePorcelain(), player);
        }
    }
}