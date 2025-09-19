using System;
using System.Collections;
using System.Collections.Generic;

namespace Aotenjo
{
    public abstract class ScoringEffectAppendEffect : Effect
    {
        protected readonly Stack<IAnimationEffect> effectStack;

        protected ScoringEffectAppendEffect(Stack<IAnimationEffect> effectStack)
        {
            this.effectStack = effectStack;
        }

        public override bool WillTrigger()
        {
            return false;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            throw new NotImplementedException();
        }

        public override Artifact GetEffectSource()
        {
            throw new NotImplementedException();
        }

        public override void Ingest(Player player)
        {
            List<IAnimationEffect> effects = GetEffects();
            effects.Reverse();
            foreach (var effect in effects)
            {
                if (effect is OnTileAnimationEffect onTileEffect)
                {
                    List<OnTileAnimationEffect> neighbors = new List<OnTileAnimationEffect>() { onTileEffect };
                    player.TriggerOnAddSingleTileAnimationEffectEvent(player.GetCurrentSelectedPerm(), neighbors, onTileEffect, onTileEffect.tile);
                    neighbors.Reverse();
                    foreach (var neighbor in neighbors)
                    {
                        PushScoringEffectToStack(neighbor, player);
                    }
                }
                else
                {
                    PushScoringEffectToStack(effect, player);
                }
            }
        }

        private void PushScoringEffectToStack(IAnimationEffect effect, Player player)
        {
            List<IAnimationEffect> neighbors = new List<IAnimationEffect>() { effect };
            player.TriggerOnAddSingleAnimationEffectEvent(neighbors, effect);
            neighbors.Reverse();
            foreach (var neighbor in neighbors)
            {
                effectStack.Push(neighbor);
            }
        }

        public abstract List<IAnimationEffect> GetEffects();
    }
}