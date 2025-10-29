using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class GemBackpackArtifact : SneakyArtifact
    {
        public GemBackpackArtifact() : base("gem_backpack", Rarity.RARE)
        {
            
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new MineralizeEffect(this));
        }

        private class MineralizeEffect : Effect
        {
            private GemBackpackArtifact gemBackpackArtifact;

            public MineralizeEffect(GemBackpackArtifact gemBackpackArtifact)
            {
                this.gemBackpackArtifact = gemBackpackArtifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_mineralized");
            }

            public override Artifact GetEffectSource()
            {
                return gemBackpackArtifact;
            }

            public override void Ingest(Player player)
            {
                if (player is SneakyPlayer p)
                {
                    List<Tile> visitedTile = new List<Tile>();
                    for (int i = 0; i < 2; i++)
                    {
                        List<Tile> targetTileCands = p.sneakedTiles.Where(t =>
                            !visitedTile.Contains(t) &&
                            t.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName()).ToList();
                        if (targetTileCands.Count() == 0) return;
                        Tile target = targetTileCands[player.GenerateRandomInt(targetTileCands.Count)];
                        List<TileMaterial> cands = MaterialSet.Ore.GetMaterials().Where(m => m is not TileMaterialOre)
                            .ToList();
                        foreach (var item in new List<TileMaterial>(cands))
                        {
                            if (p.sneakedTiles.Any(t => p.DetermineMaterialCompatibility(t, item)))
                            {
                                cands.Remove(item);
                            }
                        }

                        if (cands.Count() == 0)
                            cands = MaterialSet.Ore.GetMaterials().Where(m => m is not TileMaterialOre).ToList();
                        LotteryPool<TileMaterial> materialPool = new LotteryPool<TileMaterial>();
                        materialPool.AddRange(cands.Where(m => m.GetRarity() == Rarity.COMMON), 3);
                        materialPool.AddRange(cands.Where(m => m.GetRarity() == Rarity.RARE));
                        TileMaterial mat = materialPool.Draw(player.GenerateRandomInt);
                        visitedTile.Add(target);

                        target.SetMaterial(mat, player);
                    }
                }
            }
        }
    }
}