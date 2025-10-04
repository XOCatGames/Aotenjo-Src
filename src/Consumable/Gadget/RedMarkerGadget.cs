using System;
using Aotenjo;

[Serializable]
public class RedMarkerGadget : ReusableGadget
{
    public RedMarkerGadget() : base("red_marker", 1, 2, 12)
    {
    }

    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (ShouldHighlightTile(tile, player))
        {
            tile.AddTransform(new TileTransformRedMarker(), player);
            EventManager.Instance.OnSoundEvent("RedMarker");
            return true;
        }

        return false;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        TileTransform tileTransform = tile.GetLastTransform();
        bool isRiced = tileTransform != null && tileTransform.GetNameKey() == new TileTransformRice().GetNameKey();
        bool isMarked = tileTransform != null &&
                        tileTransform.GetNameKey() == new TileTransformRedMarker().GetNameKey();

        Pair<Tile.Category, int> pair = tile.GetLastVisibleDisplay();

        bool isBird = pair.elem1 == Tile.Category.Suo && pair.elem2 == 1;
        if (isMarked && isBird) return false;
        return !isRiced
               && (tile.GetCategory() == Tile.Category.Suo || tile.GetCategory() == Tile.Category.Bing)
               && tile.GetOrder() < 9;
    }
}