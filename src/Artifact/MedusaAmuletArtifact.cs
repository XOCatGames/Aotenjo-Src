using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MedusaAmuletArtifact : Artifact
    {
        public MedusaAmuletArtifact() : base("medusa_amulet", Rarity.RARE)
        {
            SetHighlightRequirement((tile, p) => tile.IsHonor(p));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            // 美杜莎之目独立于复制字牌的条件发放，固定获得1次使用量，并沿用道具奖励的入库与容量规则。
            effects.Add(new SimpleEffect("gadget_snakes_eye_name", this,
                p => p.RequestGadgetReward(Gadgets.SnakeEye.SetUses(1), source: this), "ReceiveGadget"));

            if (permutation == null) return;
            if (permutation.ToTiles().Any(t => t.IsHonor(player)))
            {
                effects.Add(new MedusaDuplicateTileEffect(this));
            }
        }

        private class MedusaDuplicateTileEffect : ArtifactEffect
        {
            public MedusaDuplicateTileEffect(Artifact artifact) : base("copy", artifact)
            {
            }

            public override void Ingest(Player player)
            {
                Permutation permutation = player.GetAccumulatedPermutation();
                if (permutation == null) return;
                List<Tile> cands = permutation.ToTiles().Where(t => t.IsHonor(player)).ToList();
                if (cands.Count() > 0)
                {
                    LotteryPool<Tile> pool = new();
                    pool.AddRange(cands);
                    Tile tile = pool.Draw(player.GenerateRandomInt);
                    Tile newTile = tile.Copy();
                    newTile.properties = TileProperties.Plain();
                    player.AddNewTileToPool(newTile);
                }
            }
        }
    }
}
