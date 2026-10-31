using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class ColorlessBoss : Boss
{
    public ColorlessBoss() : base("Colorless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Decolorize);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnTileAnimationEffectEvent>(player, Decolorize);
    }

    private void Decolorize(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        foreach (Tile t in player.GetPlayingTiles())
        {
            if (!t.ContainsRed(player)) continue;

            AddRedTileEffects(t, list);
        }
    }

    protected virtual void AddRedTileEffects(Tile tile, List<OnTileAnimationEffect> list)
    {
        list.Add(new IncreaseTargetEffect(1.06D, "unred").OnTile(tile));
    }

    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new ColorlessReversedArtifact(baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class ColorlessReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public ColorlessReversedArtifact(Artifact baseArtifact) : base("Colorless_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (player.IsPlayingTile(tile) && tile.ContainsRed(player))
            {
                effects.Add(new IncreaseTargetEffect(0.94D, "unred", baseArtifact));
                MessageManager.Instance.OnSetProgressBarLength(0.94f);
            }
        }
    }

    public override Boss GetHarderBoss() => new ColorlessHarderBoss();
}

[Serializable]
[HarderBoss]
public class ColorlessHarderBoss : ColorlessBoss
{
    protected override void AddRedTileEffects(Tile tile, List<OnTileAnimationEffect> list)
    {
        list.Add(new DecolorizeEffect(tile).OnTile(tile));
    }

    private class DecolorizeEffect : TextEffect
    {
        private readonly Tile tile;
        public DecolorizeEffect(Tile tile): base("effect_decolorize") { this.tile = tile; }

        public override void Ingest(Player player)
            => tile.SetFont(TileFont.COLORLESS, player);
    }

}