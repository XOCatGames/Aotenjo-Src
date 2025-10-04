using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BambooBookArtifact : Artifact, IBook
    {
        public BambooBookArtifact() : base("bamboo_book", Rarity.RARE)
        {
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if(player is not OraclePlayer) return;
            effects.Add(new SimpleEffect("effect_bamboo_book", this, this.UpgradeYaku));
        }

        public List<Yaku> GetYakuPool(Player player)
        {
            var perm = player.GetAccumulatedPermutation();
            if (perm is null) return new List<Yaku>();
            
            var allYakus = perm.GetYakus(player).ToList();
            return allYakus.Where(y =>
            {
                YakuTester.TestYaku(perm, y, player, out var relatedTiles);
                return ((OraclePlayer)player).oracleBlock.Any(t => relatedTiles.Contains(t));
            }).Select(type => type.GetYakuDefinition()).ToList();
        }

        public List<Yaku> DrawYakusToUpgrade(Player player)
        {
            if (player is not OraclePlayer oraclePlayer) return new List<Yaku>();
            var yakuPool = GetYakuPool(player);
            if (!yakuPool.Any()) return new List<Yaku>();
            var drawnYakus = new List<Yaku>();
            int drawCount = Math.Min(3, yakuPool.Count);
            return LotteryPool<Yaku>.DrawFromCollection(yakuPool, player.GenerateRandomInt, drawCount);
        }
    }
}