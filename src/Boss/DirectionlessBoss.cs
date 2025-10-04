using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class DirectionlessBoss : Boss
{
    protected virtual double Multiplier => 1.08D;
    private List<YakuType> playedYakus = new List<YakuType>();

    public DirectionlessBoss() : base("Directionless")
    {
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent += Directionless;
        player.PostRoundEndEvent += Reset;
        playedYakus.Clear();
    }


    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        player.OnPostAddScoringAnimationEffectEvent -= Directionless;
        player.PostRoundEndEvent -= Reset;
    }

    private void Reset(PlayerEvent eventData)
    {
        playedYakus.Clear();
    }

    private void Directionless(Permutation permutation, Player player, List<IAnimationEffect> list)
    {
        Permutation perm = permutation;

        List<YakuType> yakus = perm.GetYakus(player)
            .Where(a => player.GetSkillSet().GetLevel(a) > 0 && !playedYakus.Contains(a)).ToList();
        foreach (YakuType yaku in yakus)
        {
            playedYakus.Add(yaku);
            list.Add(new OnBossAnimationEffect(new NewPatternEffect(yaku, Multiplier)));
        }
    }
    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new DirectionlessBossReversedArtifact(baseArtifact);
    }

    public class DirectionlessBossReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
        private List<YakuType> playedYakus = new List<YakuType>();

        public DirectionlessBossReversedArtifact(Artifact baseArtifact) : base("Directionless_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);
            List<YakuType> yakus = permutation.GetYakus(player)
                .Where(a => player.GetSkillSet().GetLevel(a) > 0 && !playedYakus.Contains(a)).ToList();
            foreach (YakuType yaku in yakus)
            {
                playedYakus.Add(yaku);
                effects.Add(new NewPatternReversedEffect(yaku, this));
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new SilentEffect(() => playedYakus.Clear()));
        }

        public class NewPatternReversedEffect : Effect
        {
            private YakuType yaku;
            private readonly DirectionlessBossReversedArtifact artifact;

            public NewPatternReversedEffect(YakuType yaku,
                DirectionlessBossReversedArtifact artifact)
            {
                this.yaku = yaku;
                this.artifact = artifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return string.Format(func("effect_directionless_format"), YakuTester.InfoMap[yaku].GetFormattedName(func));
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                player.levelTarget *= 0.94D;
                EventManager.Instance.OnSetProgressBarLength(0.94f);
            }
        }
    }


    private class NewPatternEffect : Effect
    {
        private YakuType yaku;
        private readonly double mul;

        public NewPatternEffect(YakuType yaku, double mul)
        {
            this.yaku = yaku;
            this.mul = mul;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return string.Format(func("effect_directionless_format"), YakuTester.InfoMap[yaku].GetFormattedName(func));
        }

        public override Artifact GetEffectSource()
        {
            return null;
        }

        public override void Ingest(Player player)
        {
            player.levelTarget *= mul;
        }
    }

    public override Boss GetHarderBoss() => new DirectionlessHarderBoss();
}

[Serializable]
[HarderBoss]
public class DirectionlessHarderBoss : DirectionlessBoss
{
    protected override double Multiplier => 1.10D;

    public DirectionlessHarderBoss() : base() { }

    public override Boss GetHarderBoss() => this;
}