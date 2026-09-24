namespace Aotenjo.ClientSideEvent
{
    public sealed class TileChangeAnimationEvent : PlayerEvent
    {
        public Tile Before { get; }
        public Tile After { get; }

        public TileChangeAnimationEvent(Tile before, Tile after, Player player) : base(player)
        {
            Before = before;
            After = after;
        }
    }

    public class TileAnimationFXEvent : PlayerEvent
    {
        public Tile Tile { get; }
        public TileAnimationFXType FXType { get; }

        public TileAnimationFXEvent(Tile tile, TileAnimationFXType fxType, Player player)  : base(player)
        {
            Tile = tile;
            FXType = fxType;
        }
    }

    public class DelayedTileAnimationFXEvent : TileAnimationFXEvent
    {
        public bool shouldProc = false;
        public DelayedTileAnimationFXEvent(Tile tile, TileAnimationFXType fxType, Player player) : base(tile, fxType, player)
        {
        }
    }

    public enum TileAnimationFXType
    {
        LUCK_DRAW
    }
}
