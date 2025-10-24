using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using UnityEngine;

[Serializable]
public class DrawingFlowerTile : OneTimeUseFlowerTile
{
    private const double MUL = 5f;

    [SerializeReference] private Block block = _defaultBlock;

    [SerializeField] private bool activated;

    private static Block _defaultBlock = new("000z");

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
        if (block.CompatWith(_defaultBlock))
            return loc("flower_drawing_description_unowned");
        return string.Format(base.GetFlowerDescription(loc), block.GetSpriteString());
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);

        if (activated)
        {
            effects.Add(ScoreEffect.MulFan(MUL, null).OnTile(this));
            return;
        }

        if (used) return;
        used = true;

        if (player.GetCurrentSelectedBlocks().Any(playingBlock => playingBlock.CompatWith(block)))
        {
            effects.Add(new DrawingEffect(this).OnTile(this));
            effects.Add(ScoreEffect.MulFan(MUL, null).OnTile(this));
        }
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        base.SubscribeToPlayerEvents(player);
        if (block == _defaultBlock)
            block = player.GenerateRandomBlock();
        EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
    }

    public override void UnsubscribeFromPlayer(Player player)
    {
        base.UnsubscribeFromPlayer(player);
        EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
    }

    private void PostRoundEnd(PlayerEvent playerEvent)
    {
        activated = false;
        block = playerEvent.player.GenerateRandomBlock();
    }

    private class DrawingEffect : Effect
    {
        private readonly DrawingFlowerTile drawingFlowerTile;

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