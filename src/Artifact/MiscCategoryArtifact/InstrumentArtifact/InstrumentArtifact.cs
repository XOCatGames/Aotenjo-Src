using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public abstract class InstrumentArtifact : CountingArtifact
{
    public InstrumentArtifact(int maxCounter, string name, Rarity rarity) : base(maxCounter, name, rarity)
    {
    }

    public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
    {
        base.AppendOnSelfEffects(player, permutation, effects);
        List<Block> blocks = permutation.blocks.Where(b => b.tiles.All(t => player.Selecting(t))).ToList();
        foreach (var block in blocks)
        {
            if (!CanPlay(player, permutation, effects, block)) return;
            effects.Add(new PlayInstrumentEffect(this));
        }
    }

    public virtual void DecreaseCounter(Player player)
    {
        if (currentCounter <= 0)
        {
            currentCounter = maxCounter - (IsTuned(player) ? 2 : 1);
            return;
        }

        currentCounter--;
    }

    protected bool IsTuned(Player player)
    {
        List<Artifact> playerInventory = player.GetArtifacts();
        if (!playerInventory.Contains(Artifacts.TuningBianzhong)) return false;
        int selfIndex = playerInventory.IndexOf(this);
        int bianzhongIndex = playerInventory.IndexOf(Artifacts.TuningBianzhong);
        bool res = Math.Abs(selfIndex - bianzhongIndex) == 1;
        if (res)
        {
            AudioSystem.Play("TuningBianzhong");
        }

        return res;
    }

    protected abstract bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block);

    protected abstract void OnPlay(Player player);

    public bool IsActivating()
    {
        return currentCounter == 0;
    }

    protected virtual string GetEffectDisplay(Player player, Func<string, string> func)
    {
        int chordNum = maxCounter - currentCounter;


        if ((IsTuned(player) ? 2 : 1) == chordNum)
        {
            return func($"effect_instrument_{GetNameID()}_name_playing");
        }

        return string.Format(func($"effect_instrument_{GetNameID()}_name"), chordNum);
    }

    protected class PlayInstrumentEffect : Effect
    {
        private readonly InstrumentArtifact artifact;

        public PlayInstrumentEffect(InstrumentArtifact artifact)
        {
            this.artifact = artifact;
        }

        public override string GetSoundEffectName()
        {
            if (artifact.GetSoundEffectName() == "Default") return "AddFu";
            return artifact.GetSoundEffectName();
        }

        public override string GetEffectDisplay(Player player, Func<string, string> func)
        {
            return artifact.GetEffectDisplay(player, func);
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return "ERROR";
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override void Ingest(Player player)
        {
            if (artifact.currentCounter == 0)
            {
                artifact.OnPlay(player);
            }

            artifact.DecreaseCounter(player);
        }
    }

    protected virtual string GetSoundEffectName()
    {
        return "Default";
    }
}