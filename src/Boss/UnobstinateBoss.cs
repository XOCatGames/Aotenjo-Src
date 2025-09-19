using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using UnityEngine;

[Serializable]
public class UnobstinateBoss : Boss
{
    protected virtual double Multiplier => 1.04D;

    public UnobstinateBoss() : base("Unobstinate")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent += Persist;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= Persist;
    }

    private void Persist(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        if (permutation == null) return;
        List<Tile> tiles = new(player.GetSelectedTilesCopy());
        foreach (var tile in tiles)
        {
            if (permutation.ToTiles().Any(a => a != tile && a.CompactWith(tile)))
                list.Add(new PersistEffect(tile, Multiplier).OnTile(tile));
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnTileEffectArtifact($"{name}_reversed", 
            Rarity.COMMON,
            (_, perm, tile, effects) =>
            {
                int count = perm.ToTiles().Count(a => a != tile && a.CompactWith(tile));
                if (count > 0)
                    effects.Add(ScoreEffect.AddFu(count * 5, baseArtifact));
            });
    }

    public override Boss GetHarderBoss() => new UnobstinateHarderBoss();

    [Serializable]
    [HarderBoss]
    private class PersistEffect : Effect
    {
        [SerializeField] private Tile tile;
        private readonly double mul;

        public PersistEffect(Tile tile, double mul)
        {
            this.tile = tile;
            this.mul = mul;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_persist");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            player.levelTarget = player.GetLevelTarget() * mul;
        }
    }
}

[Serializable]
[HarderBoss]
public class UnobstinateHarderBoss : UnobstinateBoss
{
    protected override double Multiplier => 1.08D;

    public UnobstinateHarderBoss() : base() { }

    public override Boss GetHarderBoss() => this;
}