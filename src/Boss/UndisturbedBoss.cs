using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class UndisturbedBoss : Boss
{
    public UndisturbedBoss() : base("Undisturbed")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerRoundEvent.Start.Pre>(Suppress);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerRoundEvent.Start.Pre>(Suppress);
    }

    private void Suppress(PlayerEvent eventData)
    {
        Player player = eventData.player;
        const int ratio = 2;
        List<Tile> yaojiuTiles = player.GetTilePool().Where(t => t.IsYaoJiu(player)).ToList();
        int suppressCount = yaojiuTiles.Count / ratio;

        for (int i = 0; i < suppressCount; i++)
        {
            Tile tile = yaojiuTiles[player.GenerateRandomInt(yaojiuTiles.Count)];
            yaojiuTiles.Remove(tile);
            tile.Suppress(player);
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UndisturbedBossArtifact();
    }
    
    public class UndisturbedBossArtifact : Artifact
    {
        
        public UndisturbedBossArtifact() : base("Undisturbed_reversed", Rarity.COMMON)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerTileEvent.RetrieveBaseFu>(OnRetrieveBaseFu);
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerTileEvent.RetrieveBaseFu>(OnRetrieveBaseFu);
        }

        private void OnRetrieveBaseFu(PlayerTileEvent.RetrieveBaseFu evt)
        {
            if (evt.tile.IsYaoJiu(evt.player))
            {
                evt.baseFu += 6;
            }
        }
    }
}
