using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using UnityEngine;

[Serializable]
public class DrawingFlowerTile : OneTimeUseFlowerTile
{
    private const double MUL = 5f;

    [SerializeReference] private Block block = defaultBlock;

    [SerializeField] private bool activated;

    private static Block defaultBlock = new("000z");

    public DrawingFlowerTile() : base(Category.SiYi, 4)
    {
    }

    public override FlowerTile Copy()
    {
        FlowerTile flowerTile = base.Copy();
        ((DrawingFlowerTile)flowerTile).block = block;
        return flowerTile;
    }

    public override string GetFlowerDescription(Func<string, string> loc)
    {
        if (block.CompactWith(defaultBlock))
            return loc("flower_drawing_description_unowned");
        return string.Format(base.GetFlowerDescription(loc), block.GetSpriteString());
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);

        if (activated)
        {
            effects.Add(new OnTileAnimationEffect(this, ScoreEffect.MulFan(MUL, null)));
            return;
        }

        if (used) return;
        used = true;

        if (player.GetCurrentSelectedBlocks().Any(playingBlock => playingBlock.CompactWith(block)))
        {
            effects.Add(new OnTileAnimationEffect(this, new DrawingEffect(this)));
            effects.Add(new OnTileAnimationEffect(this, ScoreEffect.MulFan(MUL, null)));
        }
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        base.SubscribeToPlayerEvents(player);
        if (block == defaultBlock)
            block = player.GenerateRandomBlock();
        player.PostRoundEndEvent += ChangeBlock;
    }

    public override void UnsubscribeFromPlayer(Player player)
    {
        base.UnsubscribeFromPlayer(player);
        player.PostRoundEndEvent -= ChangeBlock;
    }

    private void ChangeBlock(PlayerEvent playerEvent)
    {
        activated = false;
        block = playerEvent.player.GenerateRandomBlock();
    }

    private class DrawingEffect : Effect
    {
        private DrawingFlowerTile drawingFlowerTile;

        public DrawingEffect(DrawingFlowerTile drawingFlowerTile)
        {
            this.drawingFlowerTile = drawingFlowerTile;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_flower_drawing_activated");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            drawingFlowerTile.activated = true;
        }
    }
}