using System;
using System.Collections.Generic;
using Aotenjo;

public class GuzhengArtifact : InstrumentArtifact
{
    private bool playedThisSettlement;

    public GuzhengArtifact() : base(4, "guzheng", Rarity.RARE)
    {
    }

    public override void ResetArtifactState()
    {
        base.ResetArtifactState();
        playedThisSettlement = false;
    }

    public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
    {
        playedThisSettlement = false;
        base.AppendOnSelfEffects(player, permutation, effects);
    }

    protected override bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block)
    {
        return block.IsABC();
    }

    public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile,
        List<Effect> effects)
    {
        base.AddOnTileEffectsPostEvents(player, permutation, tile, effects);
        // The counter already points to the next string by this stage of settlement.
        if (!playedThisSettlement) return;
        effects.Add(new CleanseEffect(this, tile));
    }

    protected override void OnPlay(Player player)
    {
        playedThisSettlement = true;
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
        return "Guzheng";
    }
}
