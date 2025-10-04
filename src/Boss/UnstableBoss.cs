using System;
using System.Collections.Generic;
using Aotenjo;
using UnityEngine;

[Serializable]
public class UnstableBoss : Boss
{
    [SerializeField] public int state = EVEN_AND_WIND;

    private const int EVEN_AND_WIND = 0;
    private const int ODD_AND_DRAGON = 1;

    public UnstableBoss() : base("Unstable")
    {
    }

    public override string GetDescription(Player player, Func<string, string> loc)
    {
        return loc($"boss_{name}_description_{state}");
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.PostSkipRoundEndEvent += OnSkip;
        player.PostSettlePermutationEvent += PostSettle;
        player.OnPostAddOnTileAnimationEffectEvent += Persist;
        state = EVEN_AND_WIND;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.PostSkipRoundEndEvent -= OnSkip;
        player.PostSettlePermutationEvent -= PostSettle;
        player.OnPostAddOnTileAnimationEffectEvent -= Persist;
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnstableBossReversedArtifact(baseArtifact);
    }
    private void PostSettle(PlayerPermutationEvent permutationEvent)
    {
        Switch();
    }

    private void OnSkip(PlayerEvent playerEvent)
    {
        Switch();
    }

    private void Switch()
    {
        state = (state == EVEN_AND_WIND ? ODD_AND_DRAGON : EVEN_AND_WIND);
    }

    private void Persist(Permutation permutation, Player player, List<OnTileAnimationEffect> list)
    {
        if (permutation == null) return;
        List<Tile> tiles = new(player.GetSelectedTilesCopy());
        foreach (var tile in tiles)
        {
            if (state == EVEN_AND_WIND)
            {
                if ((tile.IsNumbered() && tile.GetOrder() % 2 == 0) || tile.GetCategory() == Tile.Category.Feng)
                    list.Add(new SuppressEffect(tile).OnTile(tile));
            }
            else
            {
                if ((tile.IsNumbered() && tile.GetOrder() % 2 == 1) || tile.GetCategory() == Tile.Category.Jian)
                    list.Add(new SuppressEffect(tile).OnTile(tile));
            }
        }
    }
    


    #region 葫芦效果

    public class UnstableBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
        private int state = EVEN_AND_WIND;
        private HashSet<Tile> affectedTiles = new HashSet<Tile>();
        public UnstableBossReversedArtifact(Artifact baseArtifact) : base("Unstable_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }
        
        public override string GetDescription(Player player, Func<string, string> loc)
        {
            return loc("artifact_unstable_reversed_description_" + state);
        }
        
        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostSkipRoundEndEvent += OnSkip;
            player.PostSettlePermutationEvent += PostSettle;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostSkipRoundEndEvent -= OnSkip;
            player.PostSettlePermutationEvent -= PostSettle;
        }
        
        private void PostSettle(PlayerPermutationEvent permutationEvent)
        {
            Switch();
        }
        
        private void OnSkip(PlayerEvent playerEvent)
        {
            Switch();
        }
        
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            if (!player.Selecting(tile)) return;
            
            if (state == EVEN_AND_WIND)
            {
                if (!(tile.IsNumbered() && tile.GetOrder() % 2 == 0) && tile.GetCategory() != Tile.Category.Feng)
                    return;
            }
            else
            {
                if (!(tile.IsNumbered() && tile.GetOrder() % 2 == 1) && tile.GetCategory() != Tile.Category.Jian)
                    return;
            }
            
            //防止递归
            if (!affectedTiles.Add(tile)) return;
            effects.Add(new TextEffect("effect_unstable_reversed", baseArtifact));
            //重复计分
            effects.Add(new TileScoringEffectAppendEffect(player, tile, permutation, player.playHandEffectStack));
            effects.Add(new SilentEffect(() => affectedTiles.Remove(tile)));
        }
        
        private void Switch()
        {
            state = (state == EVEN_AND_WIND ? ODD_AND_DRAGON : EVEN_AND_WIND);
        }
    }
    

    #endregion

}