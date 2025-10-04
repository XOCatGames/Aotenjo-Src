using System;
using System.Collections.Generic;
using Aotenjo;

public class ErhuArtifact : InstrumentArtifact
{
    public ErhuArtifact() : base(2, "erhu", Rarity.COMMON)
    {
    }

    protected override bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block)
    {
        return true;
    }

    protected override void OnPlay(Player player)
    {
        int fu = 60;
        if (player.IsArtifactDebuffed(this)) fu = 0;
        player.RoundAccumulatedScore = player.RoundAccumulatedScore.AddFu(fu);
    }

    public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
    {
    }

    public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
    {
        base.AddOnBlockEffects(player, permutation, block, effects);
        if (block.Any(t => !player.Selecting(t)) || !CanPlay(player, permutation, effects, block)) return;
        effects.Add(new PlayInstrumentEffect(this));
    }

    protected override string GetEffectDisplay(Player player, Func<string, string> func)
    {
        int chordNum = maxCounter - currentCounter;

        if ((IsTuned(player) ? 2 : 1) == chordNum)
        {
            int fu = 60;
            player.GetArtifacts();
            if (player.IsArtifactDebuffed(this))
            {
                fu = 0;
            }

            return $"{string.Format(func("effect_add_fu_format"), fu)}";
        }

        return string.Format(func($"effect_instrument_{GetNameID()}_name"), chordNum);
    }

    public override string GetDescription(Func<string, string> func)
    {
        int chordNum = maxCounter - currentCounter;
        if (IsActivating())
        {
            return string.Format(func($"artifact_{GetNameID()}_description_ready"), chordNum);
        }

        return string.Format($"{base.GetDescription(func)}", chordNum);
    }
}