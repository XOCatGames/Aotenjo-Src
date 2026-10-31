using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public partial class Player
    {
        [NonSerialized] private bool runRuntimeDeactivated;

        // 冷启动弃局也会恢复 Boss、牌和牌体集监听，因此必须与场景退出共用完整清理边界。
        // 保留关卡和库存数据供历史记录／保存读取；不调用移除效果，也不清空全局事件总线。
        public void DeactivateRunRuntime() => DeactivateRunRuntime(
            (HandDeck ?? new List<Tile>()).Concat(Discarded ?? new List<Tile>()).Concat(TilePool ?? new List<Tile>()), true);

        internal void DeactivateRunRuntime(IEnumerable<Tile> boundTiles, bool materialSetBound, bool achievementsBound = true)
        {
            var failures = new List<Exception>();
            void Clean(Action cleanup)
            {
                try { cleanup(); }
                catch (Exception error) { failures.Add(error); }
            }
            // 旧成就包含无 Player 参数的全局订阅，必须调用自身的解绑入口，不能只按作用域删除。
            // 菜单结束流程可能补订成就，所以重复清理也需要执行此项；未订成就的恢复失败不触碰它们。
            if (achievementsBound)
                foreach (var achievement in Achievements.AchievementList)
                    Clean(() => achievement.UnsubscribeFromPlayer(this));
            if (runRuntimeDeactivated)
            {
                EventBus.ClearForPlayer(this);
                if (failures.Count > 0) throw new AggregateException("成就监听清理失败。", failures);
                return;
            }
            runRuntimeDeactivated = true;
            Clean(DeactivateArtifactRuntime);
            Clean(() => currentLevel?.Exit(this));
            foreach (var tile in boundTiles.Where(t => t != null).Distinct())
                Clean(() => tile.UnsubscribeFromPlayer(this));
            if (materialSetBound) Clean(() => materialSet?.UnsubscribeToPlayerEvents(this));
            // 成就和匿名回调也可能持有本局引用；移除本局订阅，保留跨场景的全局服务。
            EventBus.ClearForPlayer(this);
            if (failures.Count > 0) throw new AggregateException("对局监听清理失败。", failures);
        }
    }
}
