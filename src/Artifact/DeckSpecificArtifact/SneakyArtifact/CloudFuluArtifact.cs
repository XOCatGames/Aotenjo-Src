using System.Linq;

namespace Aotenjo
{
    public class CloudFuluArtifact : SneakyArtifact
    {
        public CloudFuluArtifact() : base("cloud_fulu", Rarity.COMMON)
        {
        }

        public override void OnObtain(Player player)
        {
            base.OnObtain(player);
            if (player is SneakyPlayer && player.GetGadgets().Any(g => g is CloudGlovesGadget))
            {
                ((CloudGlovesGadget)player.GetGadgets().First(g => g is CloudGlovesGadget)).maxUseCount++;
            }
        }

        public override void OnRemoved(Player player)
        {
            base.OnRemoved(player);
            if (player is SneakyPlayer && player.GetGadgets().Any(g => g is CloudGlovesGadget))
            {
                ((CloudGlovesGadget)player.GetGadgets().First(g => g is CloudGlovesGadget)).maxUseCount--;
            }
        }
    }
}