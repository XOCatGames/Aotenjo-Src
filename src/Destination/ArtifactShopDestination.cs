using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class ArtifactShopDestination : Destination
    {
        public ArtifactShopDestination(Player player,
            bool onSale) : base("artifact_shop", onSale, player)
        {
        }

        public List<ReducableContainer<Artifact>> DrawArtifacts()
        {
            int artifactPolls = 3;
            List<ReducableContainer<Artifact>> artifactDraws = player.TryDrawRandomArtifact(artifactPolls)
                .Select(a => new ReducableContainer<Artifact>(onSale, a)).ToList();
            return artifactDraws;
        }

        public List<ReducableContainer<YakuPack>> DrawYakuPacks(List<YakuPack> yakuPacks)
        {
            int yakuPackPolls = 2;
            List<ReducableContainer<YakuPack>> yakuPackDraws = player.TryDrawYakuPack(yakuPackPolls, yakuPacks)
                .Select(a => new ReducableContainer<YakuPack>(onSale, a)).ToList();
            return yakuPackDraws;
        }

        public override Destination GetRandomRedEventVariant(Player player)
        {
            return new ArtifactShopDestination(player, true);
        }
    }
}