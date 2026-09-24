using System.Collections.Generic;
using System.Linq;
using Aotenjo.ClientSideEvent;

namespace Aotenjo
{
    public class RolyPolyToyArtifact : Artifact
    {
        public RolyPolyToyArtifact() : base("roly_poly_toy", Rarity.RARE)
        {
        }

        [SubscribeToEvent]
        public void OnEvent(PlayerDrawTileEvent.Pre eventData)
        {
            if (eventData.player.GenerateRandomInt(2) == 0) return;
            List<Tile> rolyPolyTiles = eventData.player.GetTilePool().Where(t => t.IsRotationalSymmetric()).ToList();
            if(rolyPolyTiles.Count == 0) return;
            Tile redrawnTile = rolyPolyTiles[eventData.player.GetRng("draw").Invoke(rolyPolyTiles.Count)];
            eventData.tile = redrawnTile;
            EventBus.Publish(new TileAnimationFXEvent(eventData.tile, TileAnimationFXType.LUCK_DRAW, eventData.player));
            MessageManager.Instance.OnSoundEvent("LuckDraw");
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            player.SetGadgetLimit(player.GetGadgetLimit() - 1);
        }

        public override void OnRemoved(Player player)
        {
            base.OnRemoved(player);
            player.SetGadgetLimit(player.GetGadgetLimit() + 1);
        }
    }
}