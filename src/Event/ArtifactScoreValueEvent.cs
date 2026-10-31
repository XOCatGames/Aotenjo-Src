namespace Aotenjo
{
    /// <summary>
    /// 遗物番符效果的最终数值查询。显示与执行均从原始数值重新发布，不累计修改效果本身。
    /// 监听者只修正本次 Value，不改变状态或消耗随机数；效果仍沿用原来的来源、顺序和动画。
    /// </summary>
    public sealed class ArtifactScoreValueEvent : PlayerArtifactEvent
    {
        public ScoreEffect.EffectType Type { get; }
        public double Value;

        public ArtifactScoreValueEvent(Player player, Artifact artifact, ScoreEffect.EffectType type, double value)
            : base(player, artifact)
        {
            Type = type;
            Value = value;
        }
    }
}
