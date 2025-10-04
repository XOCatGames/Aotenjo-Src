using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

[Serializable]
public class UndeterminedBoss : Boss
{
    public UndeterminedBoss() : base("Undetermined")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent += ChangeSuit;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnTileAnimationEffectEvent -= ChangeSuit;
    }

    private void ChangeSuit(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        if (permutation == null) return;
        List<Tile> tiles = new(player.GetUnusedTilesInHand());
        foreach (var tile in tiles)
        {
            if (tile.IsNumbered())
            {
                list.Add(new ChangeSuitEffect(tile, (Tile.Category)(player.GenerateRandomInt(3))).OnTile(tile));
            }
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UndeterminedBossReversedArtifact(baseArtifact);
    }

    public class UndeterminedBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UndeterminedBossReversedArtifact(Artifact baseArtifact):base("Undetermined_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.OnPostAddOnTileAnimationEffectEvent += PlayerOnOnPostAddOnTileAnimationEffectEvent;
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.OnPostAddOnTileAnimationEffectEvent -= PlayerOnOnPostAddOnTileAnimationEffectEvent;
        }

        private void PlayerOnOnPostAddOnTileAnimationEffectEvent(Permutation perm, Player player, List<OnTileAnimationEffect> effects)
        {
            if (perm.ToTiles().Any(t => t.IsNumbered()))
            {
                Tile.Category mostlyPlayedCategory = perm.GetMostlyPlayedCategory();
                List<Tile> tiles = new(player.GetUnusedTilesInHand());
                foreach (var tile in tiles)
                {
                    if (tile.IsNumbered() && tile.GetCategory() != mostlyPlayedCategory)
                    {
                        effects.Add(new ChangeSuitEffect(tile, mostlyPlayedCategory)
                            .MaybeTriggerWithChance(2, "effect_undetermined_reversed")
                            .OnTile(tile));
                    }
                }
            }
        }
    }
}