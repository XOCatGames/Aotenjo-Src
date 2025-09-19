using System;

namespace Aotenjo
{
    public class MaybeEffect : Effect
    {
        private string effectName;
        private int chance;
        private Effect innerEffect;

        private bool success;

        [Obsolete("Use Effect.MaybeTriggerWithChance instead.")]
        public MaybeEffect(string effectName, int chance, Effect transformMaterialEffect)
        {
            this.effectName = effectName;
            this.chance = chance;
            innerEffect = transformMaterialEffect;
            success = false;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func($"effect_{effectName}_" + (success ? "success" : "failed"));
        }

        public override Artifact GetEffectSource()
        {
            return innerEffect.GetEffectSource();
        }

        public override void Ingest(Player player)
        {
            if (player.GenerateRandomDeterminationResult(chance))
            {
                success = true;
                innerEffect.Ingest(player);
                return;
            }

            success = false;
        }
    }
}