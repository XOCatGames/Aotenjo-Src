using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class SimpleAppendEffect : ScoringEffectAppendEffect
    {
        private readonly Func<List<IAnimationEffect>> action;

        public SimpleAppendEffect(Stack<IAnimationEffect> effectStack, Func<List<IAnimationEffect>> action) : base(effectStack) => this.action = action;
        public override List<IAnimationEffect> GetEffects() => action();
        
        public static SimpleAppendEffect Create(Stack<IAnimationEffect> effectQueue, Func<List<IAnimationEffect>> action)
            => new(effectQueue, action);
    }
}