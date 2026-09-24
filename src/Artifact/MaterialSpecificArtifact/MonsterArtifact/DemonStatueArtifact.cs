using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class DemonStatueArtifact : Artifact
    {
        private const double MUL = 2;
        private const int CORRUPT_COUNT = 4;

        public DemonStatueArtifact() : base("demon_statue", Rarity.EPIC)
        {
            SetHighlightRequirement((tile, _) =>
                tile.properties.mask.GetRegName() == TileMask.Corrupted().GetRegName());
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), MUL, CORRUPT_COUNT);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.MulFan(MUL, this));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if (permutation == null) return;
            LotteryPool<Tile> pool = new();
            pool.AddRange(player.GetScoringTiles(permutation)
                .Where(t => t.properties.mask.GetRegName() != TileMask.Corrupted().GetRegName()));
            for (int i = 0; i < CORRUPT_COUNT; i++)
            {
                if (pool.IsEmpty()) break;
                Tile tile = pool.Draw(player.GenerateRandomInt, false);
                effects.Add(new CorruptEffect(tile).OnTile(tile));
            }
        }

        public override void OnRemoved(Player player)
        {
            base.OnRemoved(player);
            var yaojiuTiles = player.GetUniqueFullDeck().Where(t => t.IsYaoJiu(player)).ToList();
            if (yaojiuTiles.Count == 0) return;
            foreach (var tile in player.GetAllTiles()
                         .Where(t => t is not FlowerTile &&
                                     t.properties.mask.GetRegName() == TileMask.Corrupted().GetRegName())
                         .ToList())
            {
                Tile before = new(tile);
                Tile target = yaojiuTiles[player.GenerateRandomInt(yaojiuTiles.Count)];
                tile.ModifyCarvedDesign(target, player);

                if (before.GetBaseCategory() == tile.GetBaseCategory() &&
                    before.GetBaseOrder() == tile.GetBaseOrder())
                {
                    continue;
                }

                EventBus.Publish(new ClientSideEvent.TileChangeAnimationEvent(before, new Tile(tile), player));
            }
        }
    }
}
