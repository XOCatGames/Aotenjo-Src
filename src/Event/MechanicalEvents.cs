namespace Aotenjo
{
    public enum MechanicalPartChangeKind
    {
        Installed,
        Destroyed,
        Transformed,
        Transferred,
        TemporaryExpired
    }

    public sealed class MechanicalPartChangedEvent : PlayerTileEvent
    {
        public readonly MechanicalPartChangeKind kind;
        public readonly MechPartType from;
        public readonly MechPartType? to;
        public readonly int count;
        public readonly Tile target;

        public MechanicalPartChangedEvent(Player player, Tile tile, MechanicalPartChangeKind kind,
            MechPartType from, MechPartType? to, int count, Tile target = null) : base(player, tile)
        {
            this.kind = kind;
            this.from = from;
            this.to = to;
            this.count = count;
            this.target = target;
        }
    }

    public sealed class MechanicalTileGrownEvent : PlayerTileEvent
    {
        public readonly Artifact source;

        public MechanicalTileGrownEvent(Player player, Tile tile, Artifact source) : base(player, tile)
        {
            this.source = source;
        }
    }

    public sealed class PostKongTilesEvent : PlayerKongTileEvent
    {
        public PostKongTilesEvent(Player player, Permutation permutation, Block block)
            : base(player, block?.tiles?[0], permutation, block)
        {
        }
    }
}
