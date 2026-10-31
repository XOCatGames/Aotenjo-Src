using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class DesirelessBoss : Boss
{
    public DesirelessBoss() : base("Desireless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Vanish);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Vanish);
    }

    private void Vanish(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        List<Tile> tiles = new(player.GetSelectedTilesCopy());
        foreach (var tile in tiles)
        {
            if (ShouldBuff(tile))
                list.Add(new IncreaseTargetEffect(1.05D, "desireless").OnTile(tile));
        }
    }

    protected virtual bool ShouldBuff(Tile tile)
        => (tile.IsNumbered() && tile.GetOrder() < 5)
           || tile.GetCategory() == Tile.Category.Jian;

    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new DesirelessReversedArtifact(name);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class DesirelessReversedArtifact : Artifact
    {
        public DesirelessReversedArtifact(string bossName) : base($"{bossName}_reversed", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (player.Selecting(tile) && ((tile.IsNumbered() && tile.GetOrder() < 5)
                                           || tile.GetCategory() == Tile.Category.Jian))
            {
                effects.Add(new IncreaseTargetEffect(0.95D, "desireless"));
                MessageManager.Instance.OnSetProgressBarLength(0.95f);
            }
        }
    }

    public override Boss GetHarderBoss() => new DesirelessHarderBoss();

}

[Serializable]
[HarderBoss]
public class DesirelessHarderBoss : DesirelessBoss
{
    //TODO: 葫芦效果
    protected override bool ShouldBuff(Tile tile)
        => (tile.IsNumbered() && tile.GetOrder() < 6)
           || tile.GetCategory() == Tile.Category.Jian;

    public DesirelessHarderBoss() : base() { }

    public override Boss GetHarderBoss() => this;
}