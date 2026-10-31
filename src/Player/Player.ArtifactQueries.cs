using System;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        [NonSerialized] private bool preparingRoundStart;
        [SerializeField] private int lastPreparedRound;

        internal ArtifactQueryEvent.Capacity QueryArtifactCapacity(Artifact artifact, ArtifactAffixState candidate = null) =>
            artifact.Query(new ArtifactQueryEvent.Capacity(this, artifact), candidate);

        private int ArtifactCapacityBonus => GetArtifacts().Sum(a => QueryArtifactCapacity(a).ArtifactSlots);

        public int[] QueryRarityWeights(int[] weights, bool inShop)
        {
            var evt = new RarityWeightsEvent(this, weights, inShop);
            EventBus.PublishRequired(evt);
            return evt.Resolve();
        }

        /// <summary>
        /// 开局准备先按现有订阅顺序收集动作，再执行库存变化，最后按最终库存重新绑定。
        /// 在正式开局广播前完成；保存阶段标记以防续局或获取回调重入后重复消耗随机数。
        /// </summary>
        public void PrepareRoundStart()
        {
            RequireArtifactRuntime();
            if (lastPreparedRound == Level) return;
            lastPreparedRound = Level;
            var evt = new PlayerRoundEvent.Start.Prepare(this);
            preparingRoundStart = true;
            try
            {
                EventBus.PublishRequired(evt);
                evt.Apply();
            }
            finally
            {
                preparingRoundStart = false;
                if (evt.HasChanges) OnArtifactOrderChanged();
            }
        }
    }
}
