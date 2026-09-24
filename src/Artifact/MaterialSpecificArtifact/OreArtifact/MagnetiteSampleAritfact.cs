using System.Linq;
using Aotenjo;

public class MagnetiteSampleAritfact : Artifact
{
    public MagnetiteSampleAritfact() : base("magnetite_sample", Rarity.COMMON)
    {
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        EventBus.Subscribe<PlayerEvents.PreKongTileEvent>(player, Gilde);
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        EventBus.Unsubscribe<PlayerEvents.PreKongTileEvent>(player, Gilde);
    }

    private void Gilde(PlayerKongTileEvent eventData)
    {
        Player player = eventData.player;
        Tile tile = eventData.tile;
        Tile sndTile = eventData.block.tiles.FirstOrDefault(t => t.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName());
        tile.SetMaterial(TileMaterial.Ore(), player);
        sndTile?.SetMaterial(TileMaterial.Ore(), player);
    }
}