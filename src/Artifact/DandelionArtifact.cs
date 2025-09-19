using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class DandelionArtifact : Artifact
    {
        private static readonly int CHANCE = 2;

        public DandelionArtifact() : base("dandelion", Rarity.RARE)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block,
            List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            if (block.tiles.Any(t => !player.Selecting(t)))
            {
                return;
            }

            if (block is PairBlock or ThirteenOrphansBlock)
            {
                return;
            }

            if (block.IsAAA() && block.OfCategory(Tile.Category.Feng))
            {
                int order = block.tiles[0].GetOrder();
                effects.Add(new SimpleEffect("effect_dandelion_success", this, p =>
                {
                    DandelionSeedGadget gadget = new DandelionSeedGadget(order);
                    gadget.uses = 1;
                    player.AddGadget(gadget);
                }).MaybeTriggerWithChance(CHANCE, "dandelion"));
            }
        }
    }
}