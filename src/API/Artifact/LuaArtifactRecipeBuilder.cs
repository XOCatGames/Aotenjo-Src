using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public static class LuaArtifactRecipeBuilder
    {
        public static ArtifactRecipe Build(string id, List<Artifact> materials, Artifact result)
        {
            ArtifactRecipe recipe = ArtifactRecipe.Create(id, materials, result);
            return recipe;
        }

        public static ArtifactRecipe BuildAndRegister(string id, List<Artifact> materials, Artifact result)
        {
            ArtifactRecipe recipe = Build(id, materials, result);

            if (ArtifactRecipes.recipes.Any(r => recipe.inputID.All(r2 => r.inputID.Contains(r2))))
            {
                Logger.LogError("ArtifactRecipe with same input already exists: " + id);
                return null;
            }
            
            ArtifactRecipes.recipes.Add(recipe);
            return recipe;
        }
    }
}