using System.Collections.Generic;

namespace Aotenjo
{
    /// <summary>
    /// 一件宿主已经收集本体效果后的扩展点。调用方仍按库存顺序逐件发布，
    /// 处理器只能向本次效果列表追加结果，不能依赖不同宿主的事件订阅顺序。
    /// 预览与动画展开共用这些事件；需要正式结算的副作用应封装成 Effect。
    /// </summary>
    public abstract class ArtifactEffectEvent : PlayerEvent
    {
        public Artifact Artifact { get; }
        public Permutation Permutation { get; }

        protected ArtifactEffectEvent(Player player, Artifact artifact, Permutation permutation) : base(player)
        {
            Artifact = artifact;
            Permutation = permutation;
        }

        public sealed class Self : ArtifactEffectEvent
        {
            public List<Effect> Effects { get; }
            public Self(Player player, Artifact artifact, Permutation permutation, List<Effect> effects)
                : base(player, artifact, permutation) { Effects = effects; }
        }

        public sealed class Tile : ArtifactEffectEvent
        {
            public Aotenjo.Tile Target { get; }
            public List<Effect> Effects { get; }
            public Tile(Player player, Artifact artifact, Permutation permutation, Aotenjo.Tile target, List<Effect> effects)
                : base(player, artifact, permutation) { Target = target; Effects = effects; }
        }

        public sealed class Block : ArtifactEffectEvent
        {
            public Aotenjo.Block Target { get; }
            public List<Effect> Effects { get; }
            public Block(Player player, Artifact artifact, Permutation permutation, Aotenjo.Block target, List<Effect> effects)
                : base(player, artifact, permutation) { Target = target; Effects = effects; }
        }

        public sealed class RoundEnd : ArtifactEffectEvent
        {
            public List<IAnimationEffect> Effects { get; }
            public RoundEnd(Player player, Artifact artifact, Permutation permutation, List<IAnimationEffect> effects)
                : base(player, artifact, permutation) { Effects = effects; }
        }
    }
}
