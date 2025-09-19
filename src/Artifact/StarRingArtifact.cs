using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class StarRingArtifact : LevelingArtifact, IMultiplierProvider
    {
        private const float MUL_PER_LEVEL = 0.2f;
        public Block block;

        public StarRingArtifact() : base("star_ring", Rarity.EPIC, 0)
        {
            block = new Block("000z");
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            if (block.ToFormat() == "000z") return false;
            return player.GetCombinator().ASuccB(tile, block.tiles[0], false);
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToMulFanFormat(GetMul(player));
        }

        public override string GetDescription(Player p, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Utils.NumberToFormat(GetMul(p)),
                block.ToFormat() == "000z" ? "" : block.GetSpriteString());
        }

        public double GetMul(Player player)
        {
            return 1f + Level * MUL_PER_LEVEL;
        }
        
        public override string Serialize()
        {
            return base.Serialize() + $"[BLOCK]{block.ToFormat()}[BLOCK]";
        }

        public override void Deserialize(string content)
        {
            string[] parts = content.Split("[BLOCK]");
            block = new(parts[1]);
            base.Deserialize(parts[0]);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (!block.SelectingBy(player)) return;
            if (player.DetermineShiftedPair(this.block, block, 1, false))
            {
                if (Level > 0)
                    effects.Add(ScoreEffect.MulFan(() => GetMul(player), this));
                effects.Add(new UpgradeEffect(this, block));
            }
            else
            {
                effects.Add(new EmptyLevelEffect(this, block));
            }
        }

        private class UpgradeEffect : Effect
        {
            private StarRingArtifact starRingArtifact;
            private Block block;

            public UpgradeEffect(StarRingArtifact starRingArtifact, Block block)
            {
                this.starRingArtifact = starRingArtifact;
                this.block = block;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_star_ring_upgrade");
            }

            public override Artifact GetEffectSource()
            {
                return starRingArtifact;
            }

            public override void Ingest(Player player)
            {
                starRingArtifact.Level++;
                if (block is PairBlock)
                    starRingArtifact.block =
                        new PairBlock(block.tiles.Select(t => new Tile(t.GetCategory(), t.GetOrder())).ToArray());
                else
                    starRingArtifact.block =
                        new Block(block.tiles.Select(t => new Tile(t.GetCategory(), t.GetOrder())).ToArray());
            }
        }

        private class EmptyLevelEffect : Effect
        {
            private StarRingArtifact starRingArtifact;
            private Block block;

            public EmptyLevelEffect(StarRingArtifact starRingArtifact, Block block)
            {
                this.starRingArtifact = starRingArtifact;
                this.block = block;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_star_ring_downgrade");
            }

            public override Artifact GetEffectSource()
            {
                return starRingArtifact;
            }

            public override void Ingest(Player player)
            {
                starRingArtifact.Level = 0;
                if (block is PairBlock)
                    starRingArtifact.block =
                        new PairBlock(block.tiles.Select(t => new Tile(t.GetCategory(), t.GetOrder())).ToArray());
                else
                    starRingArtifact.block =
                        new Block(block.tiles.Select(t => new Tile(t.GetCategory(), t.GetOrder())).ToArray());
            }
        }


    }
}