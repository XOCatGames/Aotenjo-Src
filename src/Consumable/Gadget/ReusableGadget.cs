using System;
using Aotenjo;

[Serializable]
public class ReusableGadget : Gadget
{
    public int maxUseCount;

    public ReusableGadget(string name, int id, int maxUseCount, int price) : base(name, id, maxUseCount, price)
    {
        this.maxUseCount = maxUseCount;
    }

    public override bool IsConsumable()
    {
        return false;
    }

    public override void ResetState(Player player)
    {
        base.ResetState(player);
        uses = maxUseCount;
    }

    public override string GetDescription(Func<string, string> localize)
    {
        return string.Format(base.GetDescription(localize), uses);
    }
}