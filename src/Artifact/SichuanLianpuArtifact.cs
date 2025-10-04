using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Aotenjo
{
    public class SichuanLianPuArtifact : Artifact
    {
        private const int EARN_AMOUNT = 1;

        private int level;
        private Tile.Category bounty = Tile.Category.Bing;

        public SichuanLianPuArtifact() : base("sichuan_lian_pu", Rarity.COMMON)
        {
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            return tile.CompactWithCategory(bounty);
        }

        protected override int GetSpriteID()
        {
            return bounty == Tile.Category.Wan ? 87 : bounty == Tile.Category.Bing ? 86 : 85;
        }

        public override void ResetArtifactState()
        {
            level = 0;
            bounty = Tile.Category.Bing;
        }

        public override string Serialize()
        {
            return $"{level.ToString()},{((int)bounty)}";
        }

        public override void Deserialize(string data)
        {
            var parts = data.Split(new[] { ',' });
            level = Int32.Parse(parts[0]);
            int categoryIndex = Int32.Parse(parts[1]);
            bounty = (Tile.Category)categoryIndex;
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            string typeFormat = localizer(Tile.CategoryToNameKey(bounty));
            return string.Format(base.GetDescription(localizer), level * EARN_AMOUNT, typeFormat, EARN_AMOUNT);
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, permutation, effects);


            if (player.GetCurrentSelectedBlocks().Any(b => b.OfCategory(bounty)))
            {
                Tile.Category newCat = bounty switch
                {
                    Tile.Category.Wan => Tile.Category.Bing,
                    Tile.Category.Bing => Tile.Category.Suo,
                    Tile.Category.Suo => Tile.Category.Wan,
                    _ => Tile.Category.Wan
                };
                effects.Add(new EarnMoneyEffect(EARN_AMOUNT, this));
                effects.Add(new UpgradeEffect(this, newCat));
            }
        }

        public class UpgradeEffect : Effect
        {
            private readonly SichuanLianPuArtifact artifact;
            private readonly Tile.Category category;

            public UpgradeEffect(SichuanLianPuArtifact artifact, Tile.Category category)
            {
                this.artifact = artifact;
                this.category = category;
                tags.Add(EffectTag.CyclingSuit);
            }

            public override void Ingest(Player player)
            {
                artifact.level++;
                artifact.bounty = category;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_change_face");
            }

            public override string GetSoundEffectName()
            {
                return "LianPu" + Random.Range(1, 3);
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }
        }
    }
}