using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class MechanicalOrizuluArtifact : Artifact
    {
        public MechanicalOrizuluArtifact() : base("mechanical_orizulu", Rarity.COMMON)
        {
            SetHighlightRequirement((t, _) =>
                t.IsNumbered() && (t.GetOrder() == 2 || t.GetOrder() == 5 || t.GetOrder() == 8));
        }

        public override void AddOnSelfEffects(Player player, Permutation perm, List<Effect> effects)
        {
            base.AddOnSelfEffects(player, perm, effects);
            if (perm.JiangFulfillAll((t =>
                    t.IsNumbered() && (t.GetOrder() == 2 || t.GetOrder() == 5 || t.GetOrder() == 8))))
                effects.Add(new MechanicalOrizuluEffect(this, true));
        }

        private class MechanicalOrizuluEffect : Effect
        {
            private MechanicalOrizuluArtifact artifact;
            private bool success;

            public MechanicalOrizuluEffect(MechanicalOrizuluArtifact mechanicalOrizuluArtifact, bool v)
            {
                artifact = mechanicalOrizuluArtifact;
                success = v;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func($"effect_mechanical_orizulu_name_{(success ? "success" : "failed")}");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                if (!success) return;
                Gadget gadget = player.GenerateGadgets(1, false, 19, 1)[0];
                if (gadget.IsConsumable()) gadget.uses = 1;
                player.AddGadget(gadget);
            }
        }
    }
}