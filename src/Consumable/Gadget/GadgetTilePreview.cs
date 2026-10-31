using Aotenjo;

/// <summary>A detached visual prediction; Target preserves the real tile identity for the UI.</summary>
public readonly struct GadgetTilePreview
{
    public Tile Target { get; }
    public Tile Display { get; }

    public GadgetTilePreview(Tile target, Tile display)
    {
        Target = target;
        Display = display;
    }

    public static GadgetTilePreview WithTransform(Tile target, TileTransform transform)
    {
        Tile display = target.Copy();
        display.AddTransformForced(transform);
        return new GadgetTilePreview(target, display);
    }
}
