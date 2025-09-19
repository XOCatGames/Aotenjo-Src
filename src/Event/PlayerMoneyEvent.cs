namespace Aotenjo
{
    public class PlayerMoneyEvent : PlayerEvent
    {
        public int amount;

        public PlayerMoneyEvent(Player player, int amount) : base(player)
        {
            this.amount = amount;
        }
    }
}