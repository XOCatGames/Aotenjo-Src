using System.Linq;

namespace Aotenjo
{
    public class Jigsaw19Artifact : JigsawArtifact
    {
        public Jigsaw19Artifact() : base("jigsaw_19", Rarity.COMMON)
        {
        }

        protected override bool EligibleToDowngrade(Block block, Block.Jiang jiang)
        {
            return (block.All(t => t.IsNumbered()) && jiang.All(t => t.IsNumbered())) &&
                   block.tiles.Sum(t => t.GetOrder()) + jiang.tile1.GetOrder() + jiang.tile2.GetOrder() >= 20;
        }

        protected override Effect GetPlainEffect()
        {
            return ScoreEffect.AddFu(FU, this);
        }
    }
}