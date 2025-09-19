using System.Collections.Generic;
using Aotenjo;
using System;
using System.Linq;

public class HeartlessBoss : Boss
{
    public HeartlessBoss() : base("Heartless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent += Corrupt;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= Corrupt;
    }

    private void Corrupt(Permutation perm, Player player, List<OnTileAnimationEffect> list)
    {
        if (perm == null) return;

        if (!NeedCorrupt(perm, player)) return;

        foreach (Tile t in player.GetSelectedTilesCopy().OrderBy(t => player.TileSettlingOrder(t, player.GetAccumulatedPermutation())))
            list.Add(new CorruptEffect(t).OnTile(t));
    }

    protected virtual bool NeedCorrupt(Permutation perm, Player player)
        => !perm.JiangFulfillAll(t => t.IsYaoJiu(player));

    public override Boss GetHarderBoss() => new HeartlessHarderBoss();
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnTileEffectArtifact($"{name}_reversed", 
            Rarity.COMMON,
            (player, perm, tile, effects) =>
            {
                if (!perm.JiangFulfillAll(t => !t.IsYaoJiu(player))) return;
                if (!tile.properties.IsDebuffed()) return;
                effects.Add(new CleanseEffect(baseArtifact, tile));
            });
    }
}

[Serializable]
[HarderBoss]
public class HeartlessHarderBoss : HeartlessBoss
{
    protected override bool NeedCorrupt(Permutation perm, Player player)
    {
        return perm.JiangFulfillAll(t =>
                   t.IsHonor(player)
                || (t.IsNumbered() && t.GetOrder() % 2 == 1));
    }

    public override Boss GetHarderBoss() => this;
}