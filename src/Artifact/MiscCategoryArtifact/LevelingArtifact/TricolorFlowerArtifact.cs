using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class TricolorFlowerArtifact : LevelingArtifact, IFanProvider
    {
        private Tile.Category bounty = Tile.Category.Suo;

        private const int LEVEL_PER_PROC = 3;

        public TricolorFlowerArtifact() : base("tricolor_flower", Rarity.RARE, 0)
        {
        }

        public override (string, double) GetAdditionalDisplayingInfo(Player player)
        {
            return ToAddFanFormat(GetFan(player));
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            return tile.CompactWithCategory(bounty);
        }

        protected override int GetSpriteID()
        {
            return bounty switch
            {
                Tile.Category.Wan => 88,
                Tile.Category.Bing => 89,
                Tile.Category.Suo => 90,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void ResetArtifactState()
        {
            Level = 0;
            bounty = Tile.Category.Suo;
        }

        public override string Serialize()
        {
            return $"{Level.ToString()},{((int)bounty)}";
        }

        public override void Deserialize(string data)
        {
            var parts = data.Split(new[] { ',' });
            Level = int.Parse(parts[0]);
            int categoryIndex = int.Parse(parts[1]);
            bounty = (Tile.Category)categoryIndex;
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            string typeFormat = localizer(Tile.CategoryToNameKey(bounty));
            return string.Format(base.GetDescription(localizer), Level, typeFormat, LEVEL_PER_PROC);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFan(() => GetFan(player), this));
            if (player.GetCurrentSelectedBlocks().Any(b => b.OfCategory(bounty)))
            {
                Tile.Category newCat = bounty switch
                {
                    Tile.Category.Wan => Tile.Category.Bing,
                    Tile.Category.Bing => Tile.Category.Suo,
                    Tile.Category.Suo => Tile.Category.Wan,
                    _ => Tile.Category.Wan
                };
                effects.Add(new UpgradeEffect(this, newCat));
            }
        }
        
        public double GetFan(Player player) => Level;

        public class UpgradeEffect : Effect
        {
            private readonly TricolorFlowerArtifact artifact;
            private readonly Tile.Category category;

            public UpgradeEffect(TricolorFlowerArtifact artifact, Tile.Category category)
            {
                this.artifact = artifact;
                this.category = category;
                tags.Add(EffectTag.CyclingSuit);
            }

            public override void Ingest(Player player)
            {
                artifact.Level += LEVEL_PER_PROC;
                artifact.bounty = category;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_tricolor_flower");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }

    }
}