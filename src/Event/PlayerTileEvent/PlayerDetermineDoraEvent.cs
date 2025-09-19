using static Aotenjo.BambooDeckPlayer;

namespace Aotenjo
{
    public class PlayerDetermineDoraEvent : PlayerEvent
    {
        public IndicatorTile indicator;
        public Tile cand;
        public bool yes;

        public PlayerDetermineDoraEvent(BambooDeckPlayer player, Tile tile, IndicatorTile indicator) : base(player)
        {
            cand = tile;
            this.indicator = indicator;
        }
    }
}