using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class RustChestArtifact : Artifact
    {
        private const int CHANCE = 7;

        public RustChestArtifact() : base("rust_chest", Rarity.COMMON)
        {
            
            SetHighlightRequirement((tile, player) => !((ScarletPlayer)player).IsCompatibleWithMainCategory(tile.GetCategory()));
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), CHANCE, GetChanceMultiplier(player));
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> effects,
            bool withForce, bool isClone)
        {
            base.AppendDiscardTileEffects(player, tile, effects, withForce, isClone);

            if (player is not ScarletPlayer scarlet) return;
            if (scarlet.IsCompatibleWithMainCategory(tile.GetCategory())) return;

            effects.Add(new RustChestEffect(this).MaybeTriggerWithChance(CHANCE, "rust_chest").OnTile(tile));
        }

        private class RustChestEffect : TextEffect
        {
            public RustChestEffect(RustChestArtifact artifact) : base("effect_rust_chest", artifact, "ReceiveGadget")
            {
            }

            public override void Ingest(Player player)
            {
                List<Gadget> generated = player.GenerateGadgets(1, g => g.IsConsumable(), false);
                if (generated.Count > 0)
                {
                    Gadget gadget = generated[0];
                    gadget.uses = 1;
                    player.AddGadget(gadget);

                    MessageManager.Instance.OnSoundEvent("ReceiveGadget");
                }
            }
        }
    }
}