using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public static class LuaArtifactRecipeBuilder
    {
        public static ArtifactRecipe Build(string id, List<string> materials, string result)
        {
            ArtifactRecipe recipe = ArtifactRecipe.Create(id, materials, result);
            if (!recipe.HasValidStructure)
            {
                Logger.LogError("Invalid artifact recipe: " + id);
                return null;
            }
            return recipe;
        }

        public static ArtifactRecipe BuildAndRegister(string id, List<string> materials, string result)
        {
            ArtifactRecipe recipe = Build(id, materials, result);
            if (recipe == null) return null;

            if (ArtifactRecipes.recipes.Any(r => recipe.inputID.OrderBy(value => value, System.StringComparer.Ordinal)
                    .SequenceEqual(r.inputID.OrderBy(value => value, System.StringComparer.Ordinal))))
            {
                Logger.LogError("ArtifactRecipe with same input already exists: " + id);
                return null;
            }
            
            ArtifactRecipes.recipes.Add(recipe);
            return recipe;
        }
    }
}
