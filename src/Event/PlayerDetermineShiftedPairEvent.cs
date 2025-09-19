namespace Aotenjo
{
    public class PlayerDetermineShiftedPairEvent : PlayerEvent
    {
        public readonly Block b1;
        public readonly Block b2;
        public bool res;
        public readonly int step;
        public bool catSensitive;

        public PlayerDetermineShiftedPairEvent(Player player, Block b1, Block b2, int step, bool categorySensitive,
            bool res) : base(player)
        {
            this.b1 = b1;
            this.b2 = b2;
            this.res = res;
            this.step = step;
            catSensitive = categorySensitive;
        }
    }
}