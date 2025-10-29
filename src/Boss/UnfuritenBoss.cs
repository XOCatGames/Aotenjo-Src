using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class UnfuritenBoss : Boss
{
    public UnfuritenBoss() : base("Unfuriten")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnAddSingleDiscardTileAnimationEffectEvent += OnDiscardTile;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnAddSingleDiscardTileAnimationEffectEvent -= OnDiscardTile;
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnfuritenBossReversedArtifact(baseArtifact);
    }

    private void OnDiscardTile(Player player, List<IAnimationEffect> effects, Tile tile, bool forced)
    {
        List<Tile> tiles = player.GetAllTiles().Where(t => t.CompatWith(tile)).ToList();
        effects.Add(new SuppressTileGroupAnimationEffect(tiles, tile));
    }

    #region 葫芦效果

    
    public class UnfuritenBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
    
        private Dictionary<string, int> tempGrowths = new Dictionary<string, int>();

        public UnfuritenBossReversedArtifact(Artifact baseArtifact) : base("Unfuriten_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AppendDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            onDiscardTileEffects.Add(new GroupTempGrowFuEffect(this, tile, baseArtifact));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            tempGrowths.Clear();
        }

        private void Upgrade(Tile tile)
        {
            tempGrowths.TryAdd(tile.ToString(), 0);
            tempGrowths[tile.ToString()]++;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerTileEvent.RetrieveBaseFu>(Listener);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerTileEvent.RetrieveBaseFu>(Listener);
        }

        private void Listener(PlayerTileEvent.RetrieveBaseFu obj)
        {
            if(obj.player != null && obj.tile != null && tempGrowths.TryGetValue(obj.tile.ToString(), out int growth))
            {
                obj.baseFu += 10 * growth;
            }
        }

        public class GroupTempGrowFuEffect : OnMultipleTileAnimationEffect
        {
            private readonly UnfuritenBossReversedArtifact artifact;
            private readonly Tile tile;

            public GroupTempGrowFuEffect(UnfuritenBossReversedArtifact artifact, Tile tile, Artifact baseArtifact1): base(
                new SimpleEffect("effect_Unfuriten_reversed", baseArtifact1, p => artifact.Upgrade(tile))
            )
            {
                this.artifact = artifact;
                this.tile = tile;
            }

            public override List<Tile> GetAffectedTiles(Player player)
            {
                return player.GetHandDeckCopy()
                    .Union(player.GetAccumulatedPermutation()?.ToTiles() ?? new List<Tile>())
                    .Where(t => t.CompatWith(tile))
                    .ToList();
            }

            public override Tile GetMainTile(Player player)
            {
                return tile;
            }
        }
    }

    #endregion
}
