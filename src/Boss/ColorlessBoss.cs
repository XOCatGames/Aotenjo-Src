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
        player.OnPostAddOnTileAnimationEffectEvent += Decolorize;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= Decolorize;
    }

    private void Decolorize(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        foreach (Tile t in player.GetSelectedTilesCopy())
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
        return Artifact.CreateOnTileEffectArtifact("Colorless_reversed", 
            Rarity.COMMON,
            (player, perm, tile, effects) =>
            {
                if (player.Selecting(tile) && tile.ContainsRed(player))
                {
                    effects.Add(new IncreaseTargetEffect(0.94D, "unred", baseArtifact));
                    EventManager.Instance.OnSetProgressBarLength(0.94f);
                }
            });
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