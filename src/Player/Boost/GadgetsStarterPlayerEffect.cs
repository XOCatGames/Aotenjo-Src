using System;
using Aotenjo;

public class GadgetsStarterPlayerEffect : StarterBoostEffect
{
    public const int AMOUNT = 2;

    public GadgetsStarterPlayerEffect() : base("starter_gadget")
    {
    }

    public override string GetLocalizedDesc(Func<string, string> loc)
    {
        return string.Format(base.GetLocalizedDesc(loc), AMOUNT);
    }

    public override void Boost(Player player)
    {
        player.GenerateGadgets(AMOUNT, g => !g.IsConsumable(), true, 0, 10).ForEach(g => player.AddGadget(g));
        EventManager.Instance.OnSoundEvent("ReceiveGadget");
    }

    public class Lite : StarterBoostEffect
    {
        public Lite() : base("starter_gadget_lite")
        {
        }

        public override string GetLocalizedDesc(Func<string, string> loc)
        {
            return string.Format(loc("player_effect_starter_gadget_desc"), 1);
        }

        public override void Boost(Player player)
        {
            player.GenerateGadgets(1, g => !g.IsConsumable(), true, 0, 10).ForEach(g => player.AddGadget(g));
            EventManager.Instance.OnSoundEvent("ReceiveGadget");
        }
    }

    public class Slot : StarterBoostEffect
    {
        public Slot() : base("starter_gadget_slot")
        {
        }

        public override void Boost(Player player)
        {
            player.SetGadgetLimit(player.GetGadgetLimit() + 1);
            EventManager.Instance.OnSoundEvent("ReceiveGadget");
        }
    }
}