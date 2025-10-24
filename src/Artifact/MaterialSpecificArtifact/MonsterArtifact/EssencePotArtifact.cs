using System.Linq;

namespace Aotenjo
{
    public class EssencePotArtifact : Artifact
    {
        public EssencePotArtifact() : base("essence_pot", Rarity.COMMON)
        {
            
        }

        public override bool IsAvailableInShops(Player player)
        {
            return player.GetAllTiles().Any(t => player.DetermineMaterialCompatibility(t, TileMaterial.Taotie()));
        }
    }
}