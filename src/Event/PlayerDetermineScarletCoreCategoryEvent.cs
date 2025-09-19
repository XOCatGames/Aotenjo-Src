namespace Aotenjo
{
    public enum ScarletCategoryType
    {
        Main,
        Sub,
        Unused
    }

    public class PlayerDetermineScarletCoreCategoryEvent : PlayerEvent
    {
        public readonly Tile.Category category;
        public readonly ScarletCategoryType type;
        public bool rawResult;

        public PlayerDetermineScarletCoreCategoryEvent(Player player, Tile.Category category, ScarletCategoryType type,
            bool rawResult)
            : base(player)
        {
            this.category = category;
            this.type = type;
            this.rawResult = rawResult;
        }
    }
}