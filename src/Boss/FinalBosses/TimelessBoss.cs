using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class TimelessBoss : Boss
{
    public bool firstTime = true;

    public TimelessBoss() : base("Timeless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerRoundEvent.End.PrePre>(Timeless);
        firstTime = true;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerRoundEvent.End.PrePre>(Timeless);
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new TimelessReversedArtifact(baseArtifact);
    }

    public class TimelessReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
        private bool triggered = false;

        public TimelessReversedArtifact(Artifact baseArtifact) : base("Timeless_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            bool lastHand = player.CurrentPlayingStage == player.GetMaxPlayingStage() - 1 || 
                            permutation.GetPermType() == PermutationType.SEVEN_PAIRS || 
                            permutation.GetPermType() == PermutationType.THIRTEEN_ORPHANS;
            if (triggered || !lastHand) return;
            effects.Add(new SimpleEffect("effect_timeless_reversed", baseArtifact, (p) =>
            {
                if(p.GetGadgets().TrueForAll(g => !g.regName.Equals(new TimeShardGadget().regName)))
                {
                    p.AddGadget(new TimeShardGadget());
                    triggered = true;
                }
            }, "Agate"));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new SilentEffect(() => triggered = false));
        }
    }

    private void Timeless(PlayerEvent playerEvent)
    {
        Player player = playerEvent.player;
        bool passes = Math.Floor(player.CurrentAccumulatedScore) >= Math.Floor(player.GetLevelTarget());
        if (!firstTime || !passes)
        {
            return;
        }

        OnFirstSuccessKeepPlaying(player);

        firstTime = false;
        player.SetCurrentAccumulatedBlock(null);
        player.CurrentAccumulatedScore = 0f;
        player.CurrentPlayingStage = 0;
        playerEvent.canceled = true;
    }

    protected virtual void OnFirstSuccessKeepPlaying(Player player) { }

    public override Boss GetHarderBoss() => new TimelessHarderBoss();
}

public class TimeShardGadget : Gadget
{
    public TimeShardGadget() : base("time_shard", 36, 1, 999)
    {
    }
    
    public override bool UseOnTile(Player player, Tile tile)
    {
        if (uses <= 0) return false;
        if (!ShouldHighlightTile(tile, player)) return false;
        Permutation perm = player.GetAccumulatedPermutation();
        Block block = perm.blocks.First(b => b.tiles.Contains(tile));
        player.EraseBlock(block);

        return true;
    }

    public override Rarity GetRarity()
    {
        return Rarity.RARE;
    }

    public override bool IsConsumable()
    {
        return true;
    }

    public override bool CanObtainBy(Player player, bool inShop)
    {
        return false;
    }

    public override bool CanUseOnSettledTiles()
    {
        return true;
    }

    public override bool ShouldHighlightTile(Tile tile, Player player)
    {
        List<Tile> tiles = player.GetSettledTiles();
        if (tiles.Count == 0) return false;
        return tiles.Contains(tile) && player.GetAccumulatedPermutation().JiangFulfillAll((t => t != tile));
    }
}

[Serializable]
[HarderBoss]
public class TimelessHarderBoss : TimelessBoss
{
    public TimelessHarderBoss() : base() { }

    protected override void OnFirstSuccessKeepPlaying(Player player)
    {
        player.levelTarget *= 2.0;
    }

    public override Boss GetHarderBoss() => this;
}