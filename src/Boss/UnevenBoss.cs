using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class UnevenBoss : Boss
{
    public UnevenBoss() : base("Uneven")
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
        Tile jiangTile = perm.jiang.tile1;
        Tile.Category jiangCat = jiangTile.GetCategory();
        int jiangNum = jiangTile.IsNumbered()? jiangTile.GetOrder() : -1;
        IEnumerable<Tile> handRemainder = player.GetHandDeckCopy().Except(player.GetSelectedTilesCopy());

        foreach (Tile t in handRemainder)
        {
            if (ShouldCorrupt(t, jiangCat, jiangNum))
            {
                list.Add(new CorruptEffect(t).OnTile(t));
            }
        }
    }

    protected virtual bool ShouldCorrupt(Tile t, Tile.Category jc, int jNum)
        => t.GetCategory() == jc;

    #region 葫芦效果

    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnevenBossReversedArtifact(baseArtifact);
    }

    public class UnevenBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UnevenBossReversedArtifact(Artifact baseArtifact) : base("Uneven_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            if (tile.properties.IsDebuffed() && tile.GetCategory() == perm.jiang.GetCategory())
            {
                effects.Add(new CleanseEffect(baseArtifact, tile));
            }
        }
    }
    

    #endregion

    public override Boss GetHarderBoss() => new UnevenHarderBoss();
}

[Serializable]
[HarderBoss]
public class UnevenHarderBoss : UnevenBoss
{
    protected override bool ShouldCorrupt(Tile t, Tile.Category jc, int jNum)
        => t.GetCategory() == jc
            || (t.IsNumbered() && t.GetOrder() == jNum);

    public override Boss GetHarderBoss() => this;
}