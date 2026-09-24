using System.Collections.Generic;
using System.Linq;
using Aotenjo.ClientSideEvent;

namespace Aotenjo
{
    public class PairMagnetArtifact : Artifact
    {
        public PairMagnetArtifact() : base("pair_magnet", Rarity.RARE)
        {
        }

        [SubscribeToEvent]
        public void OnEvent(PlayerDrawTileEvent.Pre eventData)
        {
            Player player = eventData.player;
            List<Tile> handTiles = player.GetHandDeckCopy().ToList();
            List<Tile> tilePool = player.GetTilePool().ToList();

            // 优先使用不会形成刻子的牌池
            List<Tile> legalTiles = tilePool
                .Where(tile => !WillCompleteTriplet(tile, handTiles))
                .ToList();

            // 如果所有牌都会形成刻子，则退回完整牌池
            if (legalTiles.Count == 0)
                legalTiles = tilePool;

            // 当前抽牌不合法时，强制替换
            if (!legalTiles.Any(tile => tile.CompatWith(eventData.tile)))
            {
                eventData.tile = legalTiles[player.GenerateRandomInt(legalTiles.Count, "draw")];
            }

            // 1/2 概率抽取能形成对子的合法牌
            if (player.GenerateRandomInt(2) == 0)
                return;

            List<Tile> pairTiles = legalTiles
                .Where(tile => WillCompletePair(tile, handTiles))
                .ToList();

            if (pairTiles.Count > 0)
            {
                eventData.tile = pairTiles[player.GenerateRandomInt(pairTiles.Count, "draw")];
                EventBus.Publish(new TileAnimationFXEvent(eventData.tile, TileAnimationFXType.LUCK_DRAW, player));
                MessageManager.Instance.OnSoundEvent("LuckDraw");
            }
        }
        
        
        
        private static int GetCompatibleTileCount(
            Tile candidate,
            IEnumerable<Tile> handTiles)
        {
            return handTiles.Count(tile => tile.CompatWith(candidate));
        }

        private static bool WillCompletePair(
            Tile candidate,
            IEnumerable<Tile> handTiles)
        {
            return GetCompatibleTileCount(candidate, handTiles) == 1;
        }

        private static bool WillCompleteTriplet(
            Tile candidate,
            IEnumerable<Tile> handTiles)
        {
            return GetCompatibleTileCount(candidate, handTiles) == 2;
        }
    }
}