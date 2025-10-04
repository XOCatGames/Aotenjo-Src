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
        List<Tile> tiles = player.GetAllTiles().Where(t => t.CompactWith(tile)).ToList();
        effects.Add(new SuppressTileGroupAnimationEffect(tiles, tile));
    }

    #region 葫芦效果

    
    public class UnfuritenBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
    
        private Dictionary<Tile, int> tempGrowths = new Dictionary<Tile, int>();

        public UnfuritenBossReversedArtifact(Artifact baseArtifact) : base("Unfuriten_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AddDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AddDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            onDiscardTileEffects.Add(new GroupTempGrowFuEffect(this, tile, baseArtifact));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            tempGrowths.Clear();
        }

        private void Upgrade(Tile tile)
        {
            tempGrowths.TryAdd(tile, 0);
            tempGrowths[tile]++;
        }

        public class GroupTempGrowFuEffect : OnMultipleTileAnimationEffect
        {
            private readonly UnfuritenBossReversedArtifact artifact;
            private readonly Tile tile;
            private readonly Artifact baseArtifact1;

            public GroupTempGrowFuEffect(UnfuritenBossReversedArtifact artifact, Tile tile, Artifact baseArtifact1): base(
                new SimpleEffect("effect_unfuriten_reversed", baseArtifact1, p => artifact.Upgrade(tile))
            )
            {
                this.artifact = artifact;
                this.tile = tile;
                this.baseArtifact1 = baseArtifact1;
            }

            public override List<Tile> GetAffectedTiles(Player player)
            {
                return player.GetHandDeckCopy().Union(player.GetSelectedTilesCopy()).Where(t => t.CompactWith(tile)).ToList();
            }

            public override Tile GetMainTile(Player player)
            {
                return tile;
            }
        }
    }

    #endregion
}
