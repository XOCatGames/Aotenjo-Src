namespace Aotenjo
{
    public class RoundStatus
    {
        public readonly int RoundWind;
        public readonly int PlayerWind;

        public RoundStatus(int round)
        {
            RoundWind = (((round - 1) % 16) / 4) + 1;
            PlayerWind = ((round - 1) % 4) + 1;
        }

        public RoundStatus(Player player) : this(player.Level)
        {
        }

        public string GetRoundWindKey()
        {
            return string.Format("wind_{0}_name", intToWind(RoundWind));
        }

        public string GetPlayerWindKey()
        {
            return string.Format("wind_{0}_name", intToWind(PlayerWind));
        }

        private string intToWind(int n)
        {
            if (n <= 0) return n.ToString();
            switch (n)
            {
                case 1:
                    return "east";
                case 2:
                    return "south";
                case 3:
                    return "west";
                case 4:
                    return "north";
                default:
                    return $"{intToWind(n - 4)}";
            }
        }
    }
}