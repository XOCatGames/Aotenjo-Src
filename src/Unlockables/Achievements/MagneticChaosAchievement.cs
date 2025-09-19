using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MagneticChaosAchievement : Achievement
    {
        public MagneticChaosAchievement(string id) : base(id)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            player.ObtainGadgetEvent += OnObtainGadget;
            player.PostObtainArtifactEvent += OnPostObtainArtifact;
        }

        private void OnPostObtainArtifact(PlayerArtifactEvent evt)
        {
            CheckPlayer(evt.player);
        }

        private void OnObtainGadget(Player player, Gadget gadget)
        {
            CheckPlayer(player);
        }

        public override void UnsubscribeFromPlayer(Player player)
        {
            player.ObtainGadgetEvent -= OnObtainGadget;
            player.PostObtainArtifactEvent -= OnPostObtainArtifact;
        }

        private void CheckPlayer(Player player)
        {
            if (player.GetArtifacts().Contains(Artifacts.MagnetiteSample) &&
                player.GetArtifacts().Contains(Artifacts.Compass))
            {
                List<Gadget> gadgets = player.GetGadgets();
                int sum = gadgets.Select(g => (g.regName == Gadgets.Magnet.regName) ? g.uses : 0).Sum();
                if (sum >= 5)
                {
                    SetComplete();
                }
            }
        }
    }
}