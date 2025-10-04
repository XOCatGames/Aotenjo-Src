namespace Aotenjo
{
    public class SpringArmArtifact : Artifact
    {
        public SpringArmArtifact() : base("spring_arm", Rarity.EPIC)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            EventBus.Subscribe<PlayerRoundEvent.Start.Post>(PostRoundStart);
            player.ObtainGadgetEvent += OnObtainGadget;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            EventBus.Unsubscribe<PlayerRoundEvent.Start.Post>(PostRoundStart);
            player.ObtainGadgetEvent -= OnObtainGadget;
        }

        private void PostRoundStart(PlayerEvent playerEvent)
        {
            Player player = playerEvent.player;
            player.GetGadgets().ForEach(gadget =>
            {
                if (!gadget.IsConsumable())
                {
                    gadget.uses++;
                }
            });
        }

        private void OnObtainGadget(Player player, Gadget gadget)
        {
            if (!gadget.IsConsumable())
            {
                gadget.uses++;
            }
        }
    }
}