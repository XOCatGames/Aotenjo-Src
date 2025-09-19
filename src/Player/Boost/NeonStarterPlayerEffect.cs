using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class NeonStarterPlayerEffect : StarterBoostEffect
{
    public NeonStarterPlayerEffect() : base("starter_neon")
    {
    }

    public override void Boost(Player player)
    {
        List<Tile> cands = player.GetTilePool().Where(t => t.IsNumbered()).ToList();
        int neonCount = cands.Count / 2;

        for (int i = 0; i < neonCount; i++)
        {
            Tile t = cands[player.GenerateRandomInt(cands.Count)];
            cands.Remove(t);

            t.SetFont(TileFont.Neon(), player);
        }
    }

    public class Lite : StarterBoostEffect
    {
        public Lite() : base("starter_neon_lite")
        {
        }

        public override void Boost(Player player)
        {
            List<Tile> cands = player.GetTilePool().Where(t => t.IsNumbered()).ToList();
            int neonCount = cands.Count / 3;
            for (int i = 0; i < neonCount; i++)
            {
                Tile t = cands[player.GenerateRandomInt(cands.Count)];
                cands.Remove(t);
                t.SetFont(TileFont.Neon(), player);
            }
        }
    }
}