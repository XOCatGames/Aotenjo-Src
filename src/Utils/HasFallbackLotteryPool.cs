using System;

namespace Aotenjo
{
    public class HasFallbackLotteryPool<T> : LotteryPool<T>
    {
        /// <summary>
        /// 备用抽奖池，在主抽奖池空时使用
        /// </summary>
        private readonly LotteryPool<T> fallbackPool;

        public HasFallbackLotteryPool(LotteryPool<T> fallbackPool)
        {
            this.fallbackPool = fallbackPool;
        }

        public override T Draw(Func<int, int> rng, bool withReplacement = true)
        {
            return base.IsEmpty() ? fallbackPool.Draw(rng, withReplacement) : base.Draw(rng, withReplacement);
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() &&  fallbackPool.IsEmpty();
        }
    }
}