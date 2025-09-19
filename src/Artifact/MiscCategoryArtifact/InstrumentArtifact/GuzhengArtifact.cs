using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class GuzhengArtifact : InstrumentArtifact
{
    public GuzhengArtifact() : base(4, "guzheng", Rarity.RARE)
    {
    }

    protected override bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block)
    {
        return block.IsABC();
    }

    public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile,
        List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!IsActivating() || !player.GetCurrentSelectedBlocks().First().IsABC()) return;
        effects.Add(new CleanseEffect(this, tile));
    }

    protected override void OnPlay(Player player)
    {
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