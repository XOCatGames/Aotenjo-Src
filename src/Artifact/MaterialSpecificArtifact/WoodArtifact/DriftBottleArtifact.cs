using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class DriftBottleArtifact : Artifact
    {
        public Tile.Category cat;

        public DriftBottleArtifact() : base("drift_bottle", Rarity.COMMON)
        {
            SetHighlightRequirement((t, _) => t.GetCategory() == cat);
        }

        public override string Serialize()
        {
            return ((int)cat).ToString();
        }

        public override void Deserialize(string data)
        {
            cat = (Tile.Category)(Int64.Parse(data));
        }

        public override void PreGameInitialized(Player player)
        {
            cat = new LotteryPool<Tile.Category>()
                .AddRange(new List<Tile.Category> { Tile.Category.Suo, Tile.Category.Bing, Tile.Category.Wan })
                .Draw(player.GenerateRandomInt);
        }

        public override string GetDescription(Player player, Func<string, string> loc)
        {
            string cat_key = cat switch
            {
                Tile.Category.Suo => "ui_suozipai",
                Tile.Category.Bing => "ui_bingzipai",
                Tile.Category.Wan => "ui_wanzipai",
                _ => "ERROR"
            };
            return string.Format(base.GetDescription(player, loc), loc(cat_key));
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new DriftBottleEffect(this, cat));
        }


        private class DriftBottleEffect : TextEffect
        {
            private Tile.Category cat;

            public DriftBottleEffect(Artifact source, Tile.Category cat) : base("effect_drift_bottle", source,
                "WaterSplash")
            {
                this.cat = cat;
            }

            public override void Ingest(Player player)
            {
                List<Tile> cands = player.GetHandDeckCopy().Where(t =>
                        t.GetCategory() == cat && t.properties.material.GetRegName() == TileMaterial.PLAIN.GetRegName())
                    .ToList();

                List<Tile> targets = new List<Tile>();
                for (int i = 0; i < 2; i++)
                {
                    if (cands.Count == 0) break;
                    Tile tile = cands[player.GenerateRandomInt(cands.Count)];
                    if (tile == null) continue;
                    targets.Add(tile);
                    cands.Remove(tile);
                }

                targets.ForEach(t => t.SetMaterial(TileMaterial.MistWood(), player));
            }
        }
    }
}