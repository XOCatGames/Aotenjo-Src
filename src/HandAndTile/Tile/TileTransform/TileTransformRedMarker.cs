using System;
using Aotenjo;

[Serializable]
public class TileTransformRedMarker : TileTransformOrderShift
{
    public TileTransformRedMarker() : base(1, "red_marked")
    {
    }

    public override int GetDisplayID(Tile t, Player player, PlayerStats stats)
    {
        Pair<Tile.Category, int> pair = t.GetLastVisibleDisplay();

        bool isBird = pair.elem1 == Tile.Category.Suo && pair.elem2 == 1 &&
                      stats.GetWonNumberByDeck(player.deck.regName) > 0;
        if (isBird)
        {
            return 7 * 9 + player.deck.id;
        }

        int i = GetStackCount(t);
        int order = pair.elem2 - 1;
        int catShift = 9 * 8 * (Tile.CategoryToInteger(pair.elem1) - 2);
        int stackShift = ((i - 1) * 9);
        int v = 9 * 9 + 16 * 9 + stackShift + order + catShift;

        //如果有3D眼镜，往下平移16行
        if (player.GetArtifacts().Contains(Artifacts.ThreeDGlasses) && Artifacts.ThreeDGlasses.IsActive(player))
        {
            v += 16 * 9;
        }

        return v;
    }
}