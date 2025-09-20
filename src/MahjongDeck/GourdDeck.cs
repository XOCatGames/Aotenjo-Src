namespace Aotenjo
{
    public class GourdDeck : MahjongDeck
    {
        public GourdDeck() : base("gourd", "standard", "all", "intermediate", 8)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            Player player = new Player(Hand.PlainFullHand().tiles, seed, this, set, asensionLevel);
            player.ObtainArtifact(Artifacts.PurpleGourd);
            return player;
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            //TODO: 解锁条件
            return true;
        }
    }
}