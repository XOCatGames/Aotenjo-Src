using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class CraftableArtifact : Artifact
    {
        public CraftableArtifact(string name, Rarity rarity) : base(name, rarity)
        {
        }

        public CraftableArtifact(string name, Rarity rarity,
            Action<Player, Permutation, Tile, List<Effect>> onTileEffect,
            Action<Player, Permutation, Block, List<Effect>> onBlockEffect,
            Action<Player, Permutation, List<Effect>> selfEffect) : base(name, rarity, onTileEffect, onBlockEffect,
            selfEffect)
        {
        }

        public override bool IsAvailableInShops(Player player)
        {
            return false;
        }

        public override List<Artifact> GetComponents()
        {
            return ArtifactRecipes.recipes
                .First(r => r.outputID.Contains(GetNameID()))
                .inputID
                .Select(i => Artifacts.ArtifactList.First(a => a.GetNameID() == i))
                .Append(this)
                .ToList();
        }
    }
}