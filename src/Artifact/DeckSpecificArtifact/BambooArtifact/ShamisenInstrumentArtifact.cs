using System;
using System.Collections.Generic;
using Aotenjo;

public class ShamisenInstrumentArtifact : InstrumentArtifact
{
    public ShamisenInstrumentArtifact() : base(3, "shamisen", Rarity.COMMON)
    {
    }

    protected override bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block)
    {
        return block.IsABC();
    }

    protected override void OnPlay(Player player)
    {
        if (player is BambooDeckPlayer bamPlayer)
        {
            bamPlayer.RevealIndicator(1);
        }
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

    protected override string GetSoundEffectName()
    {
        return "Shamisen";
    }
}