namespace Aotenjo
{
    public interface ITileHighlighter
    {
        /// <summary>
        /// 是否在悬停时高亮显示牌，通常用于遗物、番种或小道具的触发条件判断
        /// </summary>
        /// <param name="tile">需要判断的麻将牌</param>
        /// <param name="player">玩家实例</param>
        /// <returns>高亮结果</returns>
        bool ShouldHighlightTile(Tile tile, Player player);
    }
}