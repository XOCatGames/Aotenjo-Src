namespace Aotenjo
{
    public interface IJade : IRegisterable
    {
        int GetLevel(Player player);
    }
    
    public static class IJadeHelper
    {
        public static int GetEffectiveJadeStack(this IJade jade, Player player)
        {
            return player.GetEffectiveJadeStack(jade);
        }
    }
}