namespace Aotenjo
{
    public class PlayerPermutationEvent : PlayerEvent
    {
        public Permutation permutation;

        public PlayerPermutationEvent(Player player, Permutation permutation) : base(player)
        {
            this.permutation = permutation;
        }
    }
}