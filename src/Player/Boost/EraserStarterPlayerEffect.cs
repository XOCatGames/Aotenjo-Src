using Aotenjo;

public class EraserStarterPlayerEffect : StarterBoostEffect
{
    public EraserStarterPlayerEffect() : base("starter_eraser")
    {
    }

    public override void Boost(Player player)
    {
        player.AddGadget(new EraserGadget());
        EventManager.Instance.OnSoundEvent("ReceiveGadget");
    }
}