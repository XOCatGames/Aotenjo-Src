using System.Linq;
using Aotenjo;

public abstract class LoudspeakerArtifact : Artifact
{
    private int amplifyAmount;

    protected LoudspeakerArtifact(string name, Rarity rarity, int amount) : base(name, rarity)
    {
        amplifyAmount = amount;
    }

    public override bool CanObtainBy(Player player)
    {
        return base.CanObtainBy(player) && !player.GetArtifacts().Any(a => a is LoudspeakerArtifact);
    }

    public override void OnRemoved(Player player)
    {
        base.OnRemoved(player);
        player.SetHandLimit(player.GetHandLimit() - amplifyAmount);
    }

    public override void OnObtain(Player player)
    {
        base.OnObtain(player);
        player.SetHandLimit(player.GetHandLimit() + amplifyAmount);
    }
}