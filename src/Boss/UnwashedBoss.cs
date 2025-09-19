using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class UnwashedBoss : Boss
{
    private static readonly double multiplier = 0.20D;

    public UnwashedBoss() : base("Unwashed")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddOnBlockAnimationEffectEvent += BlockEventListener;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddOnBlockAnimationEffectEvent -= BlockEventListener;
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnBlockEffectArtifact($"{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, block, effects) =>
            {
                if(perm.blocks.Any(b => b != block && b.OfSameCategory(block)))
                    effects.Add(ScoreEffect.MulFan(1.2f, baseArtifact));
            });
    }

    private void BlockEventListener(Permutation perm, Player player, List<IAnimationEffect> lst)
    {
        if (perm is ThirteenOrphansPermutation or SevenPairsPermutation) return;
        Block selectedBlock = player.GetCurrentSelectedBlocks()?[0];

        lst.AddRange(perm.blocks
            .Where(block => !block.Any(t => selectedBlock != null && selectedBlock.Any(t2 => t2 == t)))
            .Where(block => block.OfSameCategory(selectedBlock))
            .Select(block => new SameSuitEffect().OnBlock(block)));
    }

    private class SameSuitEffect : Effect
    {
        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_same_suit");
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            player.levelTarget *= 1D + multiplier;
        }
    }
}