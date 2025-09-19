using Aotenjo;

public class MiniBrushEvent
{
    public readonly Gadget gadgetPtr;
    public readonly Tile tile;

    public MiniBrushEvent(Gadget gadgetPtr, Tile tile)
    {
        this.gadgetPtr = gadgetPtr;
        this.tile = tile;
    }
}