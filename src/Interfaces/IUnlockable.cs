namespace Aotenjo
{
    public interface IUnlockable : IRegisterable
    {
        public bool IsUnlocked(PlayerStats stats);
    }
}