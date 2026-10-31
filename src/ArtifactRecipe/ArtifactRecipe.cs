using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class ArtifactRecipe
    {
        public string name;
        public List<string> inputID;
        public List<string> outputID;

        private ArtifactRecipe(string name, List<string> inputID, List<string> outputID)
        {
            this.name = name;
            // 模组调用者可以复用和清空传入列表，配方必须保有自己的材料与产物快照。
            this.inputID = inputID == null ? new List<string>() : new List<string>(inputID);
            this.outputID = outputID == null ? new List<string>() : new List<string>(outputID);
        }

        // 兼容公开列表的旧 API，同时在执行边界拦截被直接改坏的配方，避免空材料自动合成。
        public bool HasValidStructure => inputID != null && outputID != null && inputID.Count > 0 && outputID.Count > 0 &&
            !inputID.Any(string.IsNullOrWhiteSpace) && !outputID.Any(string.IsNullOrWhiteSpace) &&
            !inputID.Intersect(outputID).Any();

        public virtual bool CheckFulfillRecipeRequirement(Player player)
        {
            return HasValidStructure && player.CanCraftWithArtifactAffixes(this);
        }

        public virtual void OnFulfillRecipeResult(Player player)
        {
            TryFulfillRecipeResult(player);
        }

        public bool TryFulfillRecipeResult(Player player) => player.TryCraftArtifactRecipe(this);

        public static ArtifactRecipe Create(string name, List<string> inputID, string outputID)
        {
            return new ArtifactRecipe(name, inputID, new List<string> { outputID });
        }

        public static ArtifactRecipe Create(string name, List<Artifact> inputID, Artifact outputID)
        {
            return new ArtifactRecipe(name, inputID.Select(a => a.GetNameID()).ToList(),
                new List<string> { outputID.GetNameID() });
        }
    }
}
