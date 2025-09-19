using System.Linq;
using Aotenjo;

public class ArtifactStarterPlayerEffect : StarterBoostEffect
{
    public ArtifactStarterPlayerEffect() : base("starter_artifact")
    {
    }

    public override void Boost(Player player)
    {
        player.ObtainArtifact(player.DrawRandomArtifact(Rarity.RARE, 1).First());
        player.ObtainArtifact(player.DrawRandomArtifact(Rarity.COMMON, 1).First());
    }

    public class More : StarterBoostEffect
    {
        public More() : base("starter_artifact_plus")
        {
        }

        public override void Boost(Player player)
        {
            player.ObtainArtifact(player.DrawRandomArtifact(Rarity.RARE, 1).First());
            player.ObtainArtifact(player.DrawRandomArtifact(Rarity.RARE, 1).First());
        }
    }

    public class Extreme : StarterBoostEffect
    {
        public Extreme() : base("starter_artifact_extreme")
        {
        }

        public override void Boost(Player player)
        {
            player.ObtainArtifact(player.DrawRandomArtifact(Rarity.EPIC, 1).First());
            player.ObtainArtifact(player.DrawRandomArtifact(Rarity.RARE, 1).First());
        }
    }

    public class Common : StarterBoostEffect
    {
        public Common() : base("starter_artifact_common")
        {
        }

        public override void Boost(Player player)
        {
            player.ObtainArtifact(player.DrawRandomArtifact(Rarity.COMMON, 1).First());
        }
    }
}