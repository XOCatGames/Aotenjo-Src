using System;
using System.Linq;

namespace Aotenjo
{
    public class FishingBasketArtifact : Artifact
    {
        private const int V = 5;

        public FishingBasketArtifact() : base("fishing_basket", Rarity.RARE)
        {
            SetHighlightRequirement((t, _) => t.properties.material is TileMaterialWood);
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), V, GetChanceMultiplier(player));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostDiscardTileEvent += Player_PostDiscardTileEvent;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostDiscardTileEvent -= Player_PostDiscardTileEvent;
        }

        private void Player_PostDiscardTileEvent(PlayerDiscardTileEvent.Post discardTileEvent)
        {
            Player player = discardTileEvent.player;
            if (player.GenerateRandomDeterminationResult(V) &&
                player.GetRiverTiles().Any(t => t.properties.material is TileMaterialWood))
            {
                Tile cand = LotteryPool<Tile>.DrawFromCollection(
                    player.GetRiverTiles().Where(t => t.properties.material is TileMaterialWood),
                    player.GenerateRandomInt);
                player.DiscardLeft++;
                player.RemoveTileFromDiscarded(cand);
                player.AddTileToPool(cand);
                player.priortizedDrawingList.Add(cand);
                EventManager.Instance.OnSoundEvent("WaterSplash");
            }
        }
    }
}