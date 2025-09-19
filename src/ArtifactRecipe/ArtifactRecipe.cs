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
            this.inputID = inputID;
            this.outputID = outputID;
        }

        public virtual bool CheckFulfillRecipeRequirement(Player player)
        {
            return inputID.All(s => player.GetArtifacts().Any(a => a.GetNameID() == s));
        }

        public virtual void OnFulfillRecipeResult(Player player)
        {
            int flag = 2; //Temp
            foreach (Artifact artifact in player.GetArtifacts())
            {
                if (artifact.IsBroken && flag > 1)
                {
                    flag = 1;
                }

                if (!artifact.IsBroken && !artifact.IsTemporary && flag > 0)
                {
                    flag = 0;
                }

                if (inputID.Contains(artifact.GetNameID()))
                {
                    player.RemoveArtifact(artifact, true, false);
                }
            }

            foreach (string id in outputID)
            {
                Artifact artifact = Artifacts.GetArtifact(id);
                if (artifact != null)
                {
                    artifact.IsBroken = flag == 1;
                    artifact.IsTemporary = flag == 2;
                    player.ObtainArtifact(artifact);
                }
            }
        }

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