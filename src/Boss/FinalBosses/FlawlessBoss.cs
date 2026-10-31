using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Aotenjo;
using Unity.VisualScripting;

public class FlawlessBoss : Boss
{
    public FlawlessBoss() : base("Flawless")
    {
    }

    public override string GetDescription(Player player, Func<string, string> loc)
    {
        // 只展示本场无瑕已打出的牌型，避免选牌预览或下一关提示提前亮起；小局重置后自然恢复灰度。
        Permutation permutation = player?.inRound == true && player.CurrentLevel is BossLevel level && level.Boss == this
            ? player.GetAccumulatedPermutation() : null;
        // 与无瑕惩罚沿用相同的番种等级判定，普通和残酷版本共用显示逻辑，不额外维护类别状态。
        HashSet<int> categories = permutation == null ? new HashSet<int>() : new HashSet<int>(permutation.GetYakus(player)
            .Where(y => player.GetSkillSet().GetLevel(y) >= 0)
            .SelectMany(y => YakuTester.InfoMap[y].GetYakuCategories()));
        string[] names = { "wind", "forest", "fire", "mountain" };
        return Regex.Replace(base.GetDescription(player, loc), "<sprite\\s+name=\"yakutype_(wind|forest|fire|mountain)\"\\s*>", match =>
            categories.Contains(Array.IndexOf(names, match.Groups[1].Value)) ? match.Value
                : $"<sprite=\"YakuPacksGrey\" name=\"yakutype_{match.Groups[1].Value}\">");
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        Aotenjo.EventBus.Subscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, Flawless);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        Aotenjo.EventBus.Unsubscribe<PlayerEvents.OnPostAddScoringAnimationEffectEvent>(player, Flawless);
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new FlawlessReversedArtifact(name, baseArtifact);
    }

    /// <summary>
    /// 每次反转创建独立遗物类型；保留原触发条件、结算顺序及来源引用，不在结算时按 ID 重查吸收者。
    /// </summary>
    private sealed class FlawlessReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;

        public FlawlessReversedArtifact(string bossName, Artifact baseArtifact) : base($"{bossName}_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            List<YakuType> yakuTypes =
                permutation.GetYakus(player).Where(y => player.GetSkillSet().GetLevel(y) >= 0).ToList();
            HashSet<int> idList = new HashSet<int>();

            foreach (YakuType type in yakuTypes)
            {
                idList.AddRange(YakuTester.InfoMap[type].GetYakuCategories());
            }

            for(int i = 0; i < 4; i++)
            {
                if (!idList.Contains(i))
                {
                    effects.Add(new TextEffect($"effect_boss_flawless_{i}", baseArtifact));
                    effects.Add(ScoreEffect.MulFan(2, baseArtifact));
                }
            }
        }
    }
    private void Flawless(Permutation permutation, Player player, List<IAnimationEffect> list)
    {
        List<YakuType> yakuTypes =
            permutation.GetYakus(player).Where(y => player.GetSkillSet().GetLevel(y) >= 0).ToList();
        HashSet<int> idList = new HashSet<int>();

        foreach (YakuType type in yakuTypes)
        {
            idList.AddRange(YakuTester.InfoMap[type].GetYakuCategories());
        }

        for (int i = 0; i < 4; i++)
        {
            if (!idList.Contains(i))
            {
                list.Add(new OnBossAnimationEffect(new TextEffect($"effect_boss_flawless_{i}")));
                var debuffEffect = GetDebuffEffect(player);
                if(debuffEffect != null)
                    list.Add(debuffEffect);
            }
        }
    }

    protected virtual IAnimationEffect GetDebuffEffect(Player player)
    {
        return ScoreEffect.MulFan(0.5f, null).OnBoss();
    }

    public override Boss GetHarderBoss()
    {
        return new FlawlessHarderBoss();
    }

    [HarderBoss]
    private class FlawlessHarderBoss : FlawlessBoss
    {
        private List<Artifact> debuffedArtifacts = new List<Artifact>();
        private List<Effect> debuffEffects = new List<Effect>();
        
        protected override IAnimationEffect GetDebuffEffect(Player player1)
        {
            var cands = player1.GetArtifacts()
                .Except(debuffedArtifacts)
                .Where(a => !a.IsSuppressed)
                .ToList();
            if (cands.Any())
            {
                Artifact debuffedArtifact = cands[^1];
                debuffedArtifacts.Add(debuffedArtifact);
                var debuffEffect = new SimpleEffect("effect_debuff_artifact", debuffedArtifact, player =>
                {
                    if (player.GetArtifacts().All(a => a.IsSuppressed)) return;
                    player.GetArtifacts().Where(a => !a.IsSuppressed).ToList()[^1].IsSuppressed = true;
                });
                debuffEffects.Add(debuffEffect);
                return debuffEffect;
            }

            return null;
        }
        

        public override void SubscribeToPlayerEvents(Player player)
        {
            debuffedArtifacts = new List<Artifact>();
            debuffEffects = new List<Effect>();
            // 用特性标记的处理方法替换普通版的计分后订阅。
            SubscribeAttributeEvents(player);
        }

        [SubscribeToEvent]
        private void OnPreScoring(PlayerEvents.PreAddScoringAnimationEffectEvent eventData) =>
            Flawless(eventData.permutation, eventData.player, eventData.effects);

        [SubscribeToEvent]
        private void ResetArtifacts(PlayerEvents.PostSettlePermutationEvent permutationEvent)
        {
            debuffedArtifacts.Clear();
            permutationEvent.player.GetArtifacts().ForEach(a => a.IsSuppressed = false);
        }

        public override void UnsubscribeFromPlayerEvents(Player player)
        {
            UnsubscribeAttributeEvents();
        }

        [SubscribeToEvent]
        private void PostAddScoringEffect(PlayerEvents.OnAddSingleAnimationEffectEvent eventData)
        {
            eventData.effects.RemoveAll(effect =>
            {
                if (debuffEffects.Contains(effect) || !effect.GetEffect().WillTrigger()) return false;
                Effect innerEffect = effect.GetEffect();
                Artifact artifact = innerEffect.GetEffectSource();
                if (artifact?.GetNameID() == null || artifact.GetNameID() == "") return false;
                return debuffedArtifacts.Contains(artifact);
            });
        }
        
        
    }
}
