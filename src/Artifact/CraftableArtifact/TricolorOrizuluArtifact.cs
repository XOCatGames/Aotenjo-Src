using System.Collections.Generic;

namespace Aotenjo
{
    public class TricolorOrizuluArtifact : CraftableArtifact
    {
        public TricolorOrizuluArtifact() : base("tricolor_orizulu", Rarity.RARE)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            Block.Jiang jiang = permutation.jiang;
            Dictionary<Tile.Category, Effect> effectsMap = new()
            {
                { Tile.Category.Bing, new EarnMoneyEffect(2, this) },
                { Tile.Category.Wan, ScoreEffect.MulFan(1.5f, this) },
                { Tile.Category.Suo, ScoreEffect.AddFu(100, this) }
            };

            if (jiang.All(t => t.IsNumbered()))
            {
                effects.Add(effectsMap[jiang.GetCategory()]);
            }
        }
    }
}