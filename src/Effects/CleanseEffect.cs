using System;
using Aotenjo;

public class CleanseEffect : Effect
{
    private Artifact artifact;
    private Tile tile;

    public CleanseEffect(Artifact artifact, Tile tile)
    {
        this.artifact = artifact;
        this.tile = tile;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func("effect_guzheng_cleanse_name");
    }

    public override string GetSoundEffectName()
    {
        return "Whisk";
    }

    public override Artifact GetEffectSource()
    {
        return artifact;
    }

    public override void Ingest(Player player)
    {
        if (tile.properties.mask.IsDebuff())
            tile.SetMask(TileMask.NONE, player);
        if (tile.properties.font.IsDebuff())
            tile.SetFont(TileFont.PLAIN, player);
    }
}