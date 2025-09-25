using System.Collections.Generic;
using Aotenjo;
using static UnstableBoss;
using System;

public class PowerlessBoss : Boss
{
    protected virtual int MaxSuppressCount => 4;

    public PowerlessBoss() : base("Powerless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent += OnPostAddScoringAnimationEffect;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent -= OnPostAddScoringAnimationEffect;
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new PowerlessBossReversedArtifact(baseArtifact);
    }

    public class PowerlessBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public PowerlessBossReversedArtifact(Artifact baseArtifact) : base("powerless_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnPrePostAddOnTileAnimationEffectEvent += PlayerOnOnPostAddOnTileAnimationEffectEvent;
        }
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnPrePostAddOnTileAnimationEffectEvent -= PlayerOnOnPostAddOnTileAnimationEffectEvent;
        }

        private void PlayerOnOnPostAddOnTileAnimationEffectEvent(Permutation arg1, Player arg2, List<IAnimationEffect> arg3)
        {
            arg3.Add(new TextEffect("effect_Powerless_reversed", baseArtifact));
            List<Tile> unusedTiles = arg2.GetUnusedTilesInHand();
            LotteryPool<Tile> pool = new LotteryPool<Tile>();
            pool.AddRange(unusedTiles);

            List<Tile> cands = new List<Tile>();
            int count = Math.Min(4, unusedTiles.Count);
            for (int i = 0; i < count; i++)
            {
                Tile t = pool.Draw(arg2.GenerateRandomInt);
                cands.Add(t);
            }
            cands.Sort();
            foreach (var t in cands)
            {
                arg3.Add(new TileScoringEffectAppendEffect(arg2, t, arg1, arg2.playHandEffectStack));
            }
        }
    }

    private void OnPostAddScoringAnimationEffect(Permutation permutation, Player player, List<IAnimationEffect> list)
    {
        List<Tile> unused = player.GetUnusedTilesInHand();

        foreach (Tile tile in ChooseTilesToSuppress(unused, player))
        {
            list.Add(new SuppressEffect(tile).OnTile(tile));
        }
    }

    protected virtual IEnumerable<Tile> ChooseTilesToSuppress(List<Tile> unused, Player player)
    {
        int count = Math.Min(MaxSuppressCount, unused.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = player.GenerateRandomInt(unused.Count);
            Tile t = unused[idx];
            unused.RemoveAt(idx);
            yield return t;
        }
    }

    public override Boss GetHarderBoss() => new PowerlessHarderBoss();
}

[Serializable]
[HarderBoss]
public class PowerlessHarderBoss : PowerlessBoss
{
    protected override IEnumerable<Tile> ChooseTilesToSuppress(
        List<Tile> unused, Player player) => unused;

    public PowerlessHarderBoss() : base() { }

    public override Boss GetHarderBoss() => this;
}