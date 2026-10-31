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
        EventBus.Subscribe<PlayerEvents.OnPostAddOnBlockAnimationEffectEvent>(player, BlockEventListener);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddOnBlockAnimationEffectEvent>(player, BlockEventListener);
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UnwashedReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class UnwashedReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UnwashedReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            if(permutation.blocks.Any(b => b != block && b.OfSameCategory(block)))
                effects.Add(ScoreEffect.MulFan(1.2f, baseArtifact));
        }
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