using System;
using System.Linq;
using System.Collections.Generic;
using Aotenjo;

public class StraightlessBoss : Boss
{
    private List<int> playedTile = new List<int>();

    public StraightlessBoss() : base("Straightless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent += CountTiles;
        EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        playedTile = new List<int>();
    }

    private void PostRoundEnd(PlayerEvent eventData)
    {
        playedTile.Clear();
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= CountTiles;
        EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
    }

    private void CountTiles(Permutation perm, Player player, List<OnTileAnimationEffect> lst)
    {
        if (perm == null) return;

        Tile p1 = perm.jiang.tile1;
        Tile p2 = perm.jiang.tile2;

        IEnumerable<Tile> ordered =
            player.GetSelectedTilesCopy()
                .OrderBy(t => (t == p1 || t == p2) ? 1 : 0);

        foreach (Tile t in ordered)
        {
            if (!t.IsNumbered() || playedTile.Contains(t.GetOrder())) continue;
            playedTile.Add(t.GetOrder());
            lst.Add(CreateFirstTimeNumberEffect(t));
        }
    }

    protected virtual OnTileAnimationEffect CreateFirstTimeNumberEffect(Tile tile)
        => new(tile, new DiffTileIncreScoreEffect());

    private class DiffTileIncreScoreEffect : Effect
    {
        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_fresh_number");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            player.levelTarget *= 1.1D;
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new StraightlessReverseArtifact();
    }
    public class StraightlessReverseArtifact : Artifact
    {
        private readonly HashSet<int> playedTile = new HashSet<int>();
        public StraightlessReverseArtifact() : base("Straightless_reversed", Rarity.COMMON)
        {
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if(player.Selecting(tile) && tile.IsNumbered() && !playedTile.Contains(tile.GetOrder()))
            {
                playedTile.Add(tile.GetOrder());
                effects.Add(new SimpleEffect("effect_straightl_reversed", this, (p) =>
                {
                    p.levelTarget *= 0.95D;
                    MessageManager.Instance.OnSetProgressBarLength(0.95f);
                }));
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new SilentEffect(() => playedTile.Clear()));
        }
    }

    public override Boss GetHarderBoss() => new StraightlessHarderBoss();
}


[Serializable]
[HarderBoss]
public class StraightlessHarderBoss : StraightlessBoss
{
    public StraightlessHarderBoss() : base() 
    {
    }

    protected override OnTileAnimationEffect CreateFirstTimeNumberEffect(Tile tile)
    {
        return new OnTileAnimationEffect(tile, new SuppressTileEffect(tile, null));
    }

    public override Boss GetHarderBoss() => this;
}