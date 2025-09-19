using Aotenjo;

public class LessGadgetSlotEffect : StarterBoostEffect
{
    public LessGadgetSlotEffect() : base("starter_lose_gadget_slot")
    {
    }

    public override void Boost(Player player)
    {
        player.SetGadgetLimit(player.GetGadgetLimit() - 1);
        MessageManager.Instance.OnSoundEvent("ReceiveGadget");
    }
}