using System;
using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 针对单件遗物的同步规则查询。查询只修改返回值，不改变库存、随机流或订阅。
    /// 候选商品、合成产物和未激活的存档也使用相同事件，避免预检依赖实局监听是否已绑定。
    /// </summary>
    public abstract class ArtifactQueryEvent : PlayerArtifactEvent
    {
        protected ArtifactQueryEvent(Player player, Artifact artifact) : base(player, artifact) { }

        public sealed class Capacity : ArtifactQueryEvent
        {
            public int ArtifactSlots;
            public Capacity(Player player, Artifact artifact) : base(player, artifact) { }
        }

        public sealed class Sale : ArtifactQueryEvent
        {
            public bool Allowed = true;
            public Sale(Player player, Artifact artifact) : base(player, artifact) { }
        }

        public sealed class ApplyAffix : ArtifactQueryEvent
        {
            public bool Allowed = true;
            public ApplyAffix(Player player, Artifact artifact) : base(player, artifact) { }
        }

        public sealed class Transfer : ArtifactQueryEvent
        {
            public ArtifactAffixState Affix = new();
            public Transfer(Player player, Artifact artifact) : base(player, artifact) { }
        }
    }

    /// <summary>
    /// 抽池完成阶段修正后、消耗随机数之前查询权重。调用方只提供来源与基础权重。
    /// 同类修正先汇总再执行一次，由提供者定义公式，避免多件词条被逐次取整或改变叠加规则。
    /// </summary>
    public sealed class RarityWeightsEvent : PlayerEvent
    {
        public bool InShop { get; }
        private readonly int[] weights;
        private readonly Dictionary<string, (int count, Func<int[], int, int[]> apply)> modifiers = new();

        public RarityWeightsEvent(Player player, int[] weights, bool inShop) : base(player)
        {
            this.weights = (int[])weights.Clone();
            InShop = inShop;
        }

        public void AddModifier(string id, Func<int[], int, int[]> apply)
        {
            modifiers.TryGetValue(id, out var previous);
            modifiers[id] = (previous.count + 1, apply);
        }

        public int[] Resolve()
        {
            var result = (int[])weights.Clone();
            foreach (var modifier in modifiers.Values) result = modifier.apply(result, modifier.count);
            return result;
        }
    }
}
