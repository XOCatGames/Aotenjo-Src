using Aotenjo;

public class EraserStarterPlayerEffect : StarterBoostEffect
{
    public EraserStarterPlayerEffect() : base("starter_eraser")
    {
    }

    public override void Boost(Player player)
    {
        player.AddGadget(new EraserGadget());
        MessageManager.Instance.OnSoundEvent("ReceiveGadget");
    }
}