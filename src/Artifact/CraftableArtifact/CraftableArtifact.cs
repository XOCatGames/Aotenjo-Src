using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CraftableArtifact : Artifact
    {
        public CraftableArtifact(string name, Rarity rarity) : base(name, rarity)
        {
        }

        public override bool IsAvailableInShops(Player player)
        {
            return false;
        }

        public override List<string> GetComponentIds()
        {
            return ArtifactRecipes.recipes
                .First(r => r.outputID.Contains(GetNameID()))
                .inputID
                .Append(GetRegName())
                .ToList();
        }
    }
}