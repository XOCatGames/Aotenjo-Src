using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aotenjo
{
    public class ZhouPiBookArtifact : Artifact
    {
        public ZhouPiBookArtifact() : base("zhou_pi_book", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            Permutation perm = player.GetCurrentSelectedPerm();
            if (perm == null) perm = player.GetAccumulatedPermutation();

            StringBuilder fanText = new StringBuilder();
            int[] numbers = { 4, 9, 15, 22, 30 };

            for (int i = 0; i < numbers.Length; i++)
            {
                if (perm != null && numbers[i] == GetAddedFan(perm))
                {
                    fanText.Append($"<color=\"red\">{numbers[i]}</color>");
                }
                else
                {
                    fanText.Append(numbers[i]);
                }

                if (i < numbers.Length - 1)
                {
                    fanText.Append("/");
                }
            }

            return string.Format(base.GetDescription(localizer), fanText);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            int addedFan = GetAddedFan(permutation);

            effects.Add(ScoreEffect.AddFan(addedFan, this));
        }

        private static int GetAddedFan(Permutation permutation)
        {
            float count = permutation.ToTiles().Select(t => t.GetCategory()).Distinct().Count();
            int addedFan = (int)(0.5f * count * count + 3.5 * count);
            return addedFan;
        }
    }
}