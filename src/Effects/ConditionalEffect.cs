using System;
using Aotenjo;

[Serializable]
public class ConditionalEffect : Effect
{
    private Func<Player, bool> condition;
    private Effect effect;
    private string descriptionKey;

    public ConditionalEffect(Func<Player, bool> condition, Effect effect, string descriptionKey)
    {
        this.condition = condition;
        this.effect = effect;
        this.descriptionKey = descriptionKey;
    }

    public override string GetEffectDisplay(Func<string, string> func)
    {
        return func(descriptionKey);
    }

    public override Artifact GetEffectSource()
    {
        return effect.GetEffectSource();
    }

    public override void Ingest(Player player)
    {
        if (condition(player))
            effect.Ingest(player);
    }
}