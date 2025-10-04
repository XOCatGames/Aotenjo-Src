using System;
using System.Collections.Generic;
using Aotenjo;
using UnityEngine;

[Serializable]
public class DandelionSeedGadget : Gadget
{
    [SerializeField] private int order;

    public DandelionSeedGadget(int order) : base($"dandelion_seed_{order}", 21 + order, 5, 10)
    {
        this.order = order;
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        player.RemoveTileFromHand(tile, true);
        string representation = $"{order}{(char)('f' + player.GenerateRandomInt(4))}";
        Hand hand = new Hand(representation);
        List<Tile> tiles = hand.tiles;
        Tile toAdd = tiles[0];
        toAdd.SetMask(TileMask.Fractured(), player, true);
        player.AddTileToHand(toAdd);
        player.SortDeck();
        int index = player.GetHandDeckCopy().IndexOf(toAdd);
        EventManager.Instance.OnDrawTiles(new List<int> { index });
        return true;
    }

    public override Gadget Copy()
    {
        return new DandelionSeedGadget(order).SetUses(uses);
    }

    public override bool IsConsumable()
    {
        return true;
    }

    public override int GetStackLimit()
    {
        return 5;
    }
}