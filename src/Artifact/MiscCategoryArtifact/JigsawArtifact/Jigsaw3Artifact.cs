namespace Aotenjo
{
    public class Jigsaw3Artifact : JigsawArtifact
    {
        public Jigsaw3Artifact() : base("jigsaw_3", Rarity.EPIC)
        {
        }

        protected override bool EligibleToDowngrade(Block block, Block.Jiang jiang)
        {
            return block.All(t => t.IsNumbered()) && block.Any(b => b.GetOrder() == 2) &&
                   jiang.All(t => t.IsNumbered() && t.GetOrder() == 2);
        }

        protected override Effect GetPlainEffect()
        {
            return ScoreEffect.MulFan(MUL, this);
        }
    }
}