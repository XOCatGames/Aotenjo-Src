using Aotenjo;

public class SlotStarterPlayerEffect : StarterBoostEffect
{
    public SlotStarterPlayerEffect() : base("starter_extra_slot")
    {
    }

    public override void Boost(Player player)
    {
        player.SetArtifactLimit(player.GetArtifactLimit() + 1);
        MessageManager.Instance.OnSoundEvent("upgrade");
    }

    public class More : StarterBoostEffect
    {
        public More() : base("starter_extra_slot_more")
        {
        }

        public override void Boost(Player player)
        {
            player.SetArtifactLimit(player.GetArtifactLimit() + 2);
            MessageManager.Instance.OnSoundEvent("upgrade");
        }
    }
}