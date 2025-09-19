using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class WastelandDestination : Destination
{
    public WastelandDestination(bool onSale, Player player) : base("wasteland", onSale, player)
    {
    }

    public List<Gadget> GetGadgets()
    {
        int gadgetPolls = 3; // 假设玩家可以从3个小工具中选择
        List<Gadget> gadgetDraws = player.GenerateGadgets(gadgetPolls, false, 1, 0)
            .Select(g => g.SetUses(onSale ? (g.uses == 1 ? 1 : g.uses - 1) : 1)).ToList();

        return gadgetDraws;
    }

    public List<Tile> GetTiles()
    {
        List<Tile> tiles = player.GenerateRandomTileWithEffects(4);

        if (onSale) return tiles;

        foreach (Tile tile in tiles)
        {
            if (player.GenerateRandomInt(onSale ? 7 : 3) == 0)
            {
                tile.properties.mask = player.GenerateRandomInt(3) == 0 ? TileMask.Fractured() : TileMask.Corrupted();
            }
        }

        return tiles;
    }

    public override Destination GetRandomRedEventVariant(Player player)
    {
        return new WastelandDestination(true, player);
    }

    public List<Artifact> GetBrokenArtifacts()
    {
        LotteryPool<Rarity> rarityPool = new();
        rarityPool.Add(Rarity.COMMON, 150);
        rarityPool.Add(Rarity.RARE, player.Level > 4 ? (onSale ? 20 : 10) : 5);
        if (onSale)
            rarityPool.Add(Rarity.EPIC, player.Level > 8 ? 2 : 0);

        List<Artifact> artifacts = player.DrawRandomArtifact(rarityPool.Draw(player.GenerateRandomInt), 2);
        artifacts.ForEach(a =>
        {
            if (!onSale && player.GenerateRandomInt(2) == 0) a.SetBroken(player);
        });
        return artifacts;
    }
}