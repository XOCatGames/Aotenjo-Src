using System;
using System.Collections.Generic;
using Aotenjo;

public class PipaArtifact : InstrumentArtifact, IPersistantAura
{
    private const int MULT = 5;
    private bool isAffecting;

    public PipaArtifact() : base(5, "pipa", Rarity.EPIC)
    {
    }

    public override string Serialize()
    {
        return base.Serialize() + "[ON]" + isAffecting + "[ON]";
    }

    public override void Deserialize(string data)
    {
        string[] parts = data.Split(new[] { "[ON]" }, StringSplitOptions.None);
        base.Deserialize(parts[0]);
        isAffecting = bool.Parse(parts[1]);
    }

    public override void ResetArtifactState()
    {
        base.ResetArtifactState();
        isAffecting = false;
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        player.PostRoundEndEvent += OnRoundEnd;
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        player.PostRoundEndEvent -= OnRoundEnd;
    }

    private void OnRoundEnd(PlayerEvent _)
    {
        isAffecting = false;
    }

    protected override bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block)
    {
        return true;
    }

    protected override void OnPlay(Player player)
    {
        isAffecting = true;
    }

    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!player.Selecting(tile) || player.GetCurrentSelectedBlocks().Count <= currentCounter) return;
        effects.Add(new FractureEffect(this, tile, "effect_pipa_fracture_name"));
    }

    public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
    {
        base.AddOnSelfEffects(player, permutation, effects);
        if (isAffecting)
        {
            effects.Add(ScoreEffect.MulFan(MULT, this));
            return;
        }

        if (player.GetCurrentSelectedBlocks().Count <= currentCounter) return;
        effects.Add(ScoreEffect.MulFan(MULT, this));
    }

    public override string GetDescription(Func<string, string> func)
    {
        int chordNum = maxCounter - currentCounter;
        if (IsActivating())
        {
            return string.Format(func($"artifact_{GetNameID()}_description_ready"), chordNum, MULT);
        }

        return string.Format($"{base.GetDescription(func)}", chordNum, MULT);
    }

    protected override string GetEffectDisplay(Player player, Func<string, string> func)
    {
        int chordNum = maxCounter - currentCounter;

        if ((IsTuned(player) ? 2 : 1) == chordNum)
        {
            return base.GetEffectDisplay(player, func);
        }

        if (chordNum % 2 == 0)
            return string.Format(func("effect_pipa_big_name"));
        return string.Format(func("effect_pipa_small_name"));
    }

    public bool IsAffecting(Player player)
    {
        return isAffecting;
    }

    protected override string GetSoundEffectName()
    {
        return "Pipa";
    }
}