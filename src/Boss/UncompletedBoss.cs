using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using static Aotenjo.Tile;

public class UncompletedBoss : Boss
{
    private List<Category> playedCategories = new List<Category>();

    public UncompletedBoss() : base("Uncomplete")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        EventBus.Subscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, CountTypes);
        EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
        playedCategories = new List<Category>();
    }

    private void PostRoundEnd(PlayerEvent eventData)
    {
        playedCategories.Clear();
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        EventBus.Unsubscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, CountTypes);
        EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
    }

    private void CountTypes(Permutation perm, Player player, List<IAnimationEffect> lst)
    {
        List<Tile> tiles = perm.blocks.SelectMany(b => b.tiles).ToList();
        foreach (Tile t in tiles)
        {
            if (!playedCategories.Contains(t.GetCategory()))
            {
                playedCategories.Add(t.GetCategory());
                lst.Add(new SuppressTileGroupAnimationEffect(
                    perm.blocks.First(b => b.Any(t2 => t2 == t)).tiles.ToList(), t));
            }
        }
    }
    
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new UncompletedReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class UncompletedReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public UncompletedReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            effects.Add(ScoreEffect.AddFu(40 * permutation.ToTiles()
                .Select(p => p.GetCategory())
                .Distinct()
                .Count(),
                baseArtifact));
        }
    }
}