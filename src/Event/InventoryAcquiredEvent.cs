namespace Aotenjo
{
    /// <summary>Presentation-only receipt, published after inventory mutation and gameplay hooks.</summary>
    public sealed class InventoryAcquiredEvent : PlayerEvent
    {
        public readonly Gadget gadget;
        public readonly Gadget displayGadget;
        public readonly Artifact artifact;
        public readonly Artifact source;
        public readonly int amount;

        public InventoryAcquiredEvent(Player player, Gadget target, int amount, Artifact source = null) : base(player)
        {
            gadget = target;
            displayGadget = target.Copy();
            this.amount = amount;
            this.source = source;
        }

        public InventoryAcquiredEvent(Player player, Artifact artifact, Artifact source = null) : base(player)
        {
            this.artifact = artifact;
            this.source = source;
            amount = 1;
        }
    }
}
