using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PorcelainMirrorArtifact : Artifact
    {
        public PorcelainMirrorArtifact() : base("porcelain_mirror", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer));
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (!block.SelectingBy(player)) return;
            if (permutation.GetPermType() == PermutationType.THIRTEEN_ORPHANS)
            {
                return;
            }

            if (permutation.blocks.Any(b =>
                    b.tiles[0] != block.tiles[0] && player.DetermineShiftedPair(block, b, 0, false)))
            {
                effects.Add(new UpgradeEffect(this, block));
            }
        }

        private class UpgradeEffect : Effect
        {
            private PorcelainMirrorArtifact artifact;
            private Block block;

            public UpgradeEffect(PorcelainMirrorArtifact artifact, Block block)
            {
                this.artifact = artifact;
                this.block = block;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_porcelainize_name");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                List<Tile> cand = block.tiles.Where(t => t.CompactWithMaterial(TileMaterial.PLAIN, player)).ToList();
                if (cand.Count == 0)
                {
                    return;
                }

                Tile tile = cand[player.GenerateRandomInt(cand.Count)];

                LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();
                pool.AddRange(MaterialSet.Porcelain.GetMaterials().Where(m => m.GetRarity() == Rarity.COMMON).ToList(),
                    5);
                pool.AddRange(MaterialSet.Porcelain.GetMaterials().Where(m => m.GetRarity() == Rarity.RARE).ToList());

                tile.SetMaterial(pool.Draw(player.GenerateRandomInt), player);
            }
        }
    }
}