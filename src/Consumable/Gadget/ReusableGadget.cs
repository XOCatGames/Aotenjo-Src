using System;
using Aotenjo;

[Serializable]
public class ReusableGadget : Gadget
{
    protected override Gadget CreateCopy() => new ReusableGadget(regName, GetID(), maxUseCount, Price);

    public int maxUseCount;

    public ReusableGadget(string name, int id, int maxUseCount, int price) : base(name, id, maxUseCount, price)
    {
        this.maxUseCount = maxUseCount;
    }

    public override bool IsConsumable()
    {
        return false;
    }

    public override void OnObtained(Player player)
    {
        uses = maxUseCount;
    }

    public override void OnRoundStart(Player player)
    {
        uses = maxUseCount;
    }

    protected override void CopyStateTo(Gadget copy)
    {
        base.CopyStateTo(copy);
        ((ReusableGadget)copy).maxUseCount = maxUseCount;
    }

    public override string GetDescription(Func<string, string> localize)
    {
        return string.Format(base.GetDescription(localize), uses);
    }
}
