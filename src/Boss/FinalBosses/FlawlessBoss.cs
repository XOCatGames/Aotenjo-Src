using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using Unity.VisualScripting;

public class FlawlessBoss : Boss
{
    public FlawlessBoss() : base("Flawless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent += Flawless;
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent -= Flawless;
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return Artifact.CreateOnSelfEffectArtifact($"{this.name}_reversed", 
            Rarity.COMMON,
            (player, perm, effects) =>
            {
                List<YakuType> yakuTypes =
                    perm.GetYakus(player).Where(y => player.GetSkillSet().GetLevel(y) >= 0).ToList();
                List<YakuPack> yakuPacks = GameManager.Instance.yakuPacks;
                HashSet<int> idList = new HashSet<int>();

                foreach (YakuType type in yakuTypes)
                {
                    idList.AddRange(YakuTester.InfoMap[type].GetYakuCategories());
                }

                foreach (YakuPack pack in yakuPacks)
                {
                    if (!idList.Contains(pack.id))
                    {
                        effects.Add(new TextEffect($"effect_boss_flawless_{pack.id}", baseArtifact));
                        effects.Add(ScoreEffect.MulFan(2, baseArtifact));
                    }
                }
            });
    }
    private void Flawless(Permutation permutation, Player player, List<IAnimationEffect> list)
    {
        List<YakuType> yakuTypes =
            permutation.GetYakus(player).Where(y => player.GetSkillSet().GetLevel(y) >= 0).ToList();
        List<YakuPack> yakuPacks = GameManager.Instance.yakuPacks;
        HashSet<int> idList = new HashSet<int>();

        foreach (YakuType type in yakuTypes)
        {
            idList.AddRange(YakuTester.InfoMap[type].GetYakuCategories());
        }

        foreach (YakuPack pack in yakuPacks)
        {
            if (!idList.Contains(pack.id))
            {
                list.Add(new OnBossAnimationEffect(new TextEffect($"effect_boss_flawless_{pack.id}")));
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
                .Where(a => !a.IsDebuffed)
                .ToList();
            if (cands.Any())
            {
                Artifact debuffedArtifact = cands[^1];
                debuffedArtifacts.Add(debuffedArtifact);
                var debuffEffect = new SimpleEffect("effect_debuff_artifact", debuffedArtifact, player =>
                {
                    if (player.GetArtifacts().All(a => a.IsDebuffed)) return;
                    player.GetArtifacts().Where(a => !a.IsDebuffed).ToList()[^1].IsDebuffed = true;
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
            player.PreAddScoringAnimationEffectEvent += Flawless;
            player.OnAddSingleAnimationEffectEvent += PostAddScoringEffect;
            player.PostSettlePermutationEvent += ResetArtifacts;
        }

        private void ResetArtifacts(PlayerPermutationEvent permutationEvent)
        {
            debuffedArtifacts.Clear();
            permutationEvent.player.GetArtifacts().ForEach(a => a.IsDebuffed = false);
        }

        public override void UnsubscribeFromPlayerEvents(Player player)
        {
            player.PreAddScoringAnimationEffectEvent -= Flawless;
            player.OnAddSingleAnimationEffectEvent -= PostAddScoringEffect;
            player.PostSettlePermutationEvent -= ResetArtifacts;
        }

        private void PostAddScoringEffect(Player player, List<IAnimationEffect> list, IAnimationEffect eff)
        {
            list.RemoveAll(effect =>
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