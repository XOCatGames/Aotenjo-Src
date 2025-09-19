using System.Linq;

namespace Aotenjo
{
    public class Jigsaw12Artifact : JigsawArtifact
    {
        public Jigsaw12Artifact() : base("jigsaw_12", Rarity.RARE)
        {
        }

        protected override bool EligibleToDowngrade(Block block, Block.Jiang jiang)
        {
            return (block.All(t => t.IsNumbered()) && jiang.All(t => t.IsNumbered()))
                   && block.tiles.Sum(t => t.GetOrder()) + jiang.tile1.GetOrder() + jiang.tile2.GetOrder() <= 12;
        }

        protected override Effect GetPlainEffect()
        {
            return ScoreEffect.AddFan(FAN, this);
        }
    }
}