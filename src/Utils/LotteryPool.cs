using System;
using System.Collections.Generic;
using System.Linq;

//作者: 黄上栩 Shangxu Huang
//联系邮箱: shear.s.huang@gmail.com
//此文件内代码使用 MIT License 许可

/// <summary>
/// 树状抽奖池
/// </summary>
/// <typeparam name="T">抽奖物品类型参数</typeparam>
public class LotteryPool<T>
{
    /// <summary>
    /// 可抽取物品池
    /// </summary>
    private readonly List<LotteryPool<T>> pool = new();

    /// <summary>
    /// 与可抽取物对应的权重值
    /// </summary>
    private readonly List<int> weights = new();

    /// <summary>
    /// 增添一个池子作为可抽取物
    /// </summary>
    /// <param name="item">另一个抽奖池</param>
    /// <param name="weight">对应权重值</param>
    /// <returns>增添完后的池子</returns>
    public virtual LotteryPool<T> Add(LotteryPool<T> item, int weight)
    {
        pool.Add(item);
        weights.Add(weight);
        return this;
    }

    /// <summary>
    /// 增添一个单一物品作为可抽取物
    /// </summary>
    /// <param name="item">物品指针</param>
    /// <param name="weight">对应权重值</param>
    /// <returns>增添完后的池子</returns>
    public virtual LotteryPool<T> Add(T item, int weight)
    {
        pool.Add(new LotteryItem(item));
        weights.Add(weight);
        return this;
    }

    /// <summary>
    /// 帮助函数，增添多个同权重单一物品
    /// </summary>
    /// <param name="items">集合指针</param>
    /// <param name="individualWeights">所有物品单独对应的权重值</param>
    /// <returns>增添完后的池子</returns>
    public virtual LotteryPool<T> AddRange(IEnumerable<T> items, int individualWeights = 1)
    {
        foreach (T item in items)
        {
            Add(item, individualWeights);
        }

        return this;
    }

    /// <summary>
    /// 主抽取函数
    /// </summary>
    /// <param name="rng">随机数生成器</param>
    /// <param name="withReplacement">同一物品是否可以重复抽取</param>
    /// <returns>抽取结果</returns>
    public virtual T Draw(Func<int, int> rng, bool withReplacement = true)
    {
        //首先生成一个介于0与权重总和之间的目标数，这个数字代表了这一次抽取结果位于的位置
        int totalWeight = weights.Sum();
        int targetNum = rng(totalWeight);

        //开始遍历所有的物品
        for (int i = 0; i < weights.Count; i++)
        {
            //对于每个物品，用目标数减去这个物品的权重值，如果减后目标数小于0，则确定命中当前物品
            targetNum -= weights[i];
            if (targetNum < 0)
            {
                //命中后进行递归，在命中的池或单一物品内进行抽取
                T result = pool[i].Draw(rng, withReplacement);

                //如果此次抽取为不可重复抽取，则把对应的物品从池中移除
                if (pool[i] is LotteryItem || (pool[i] is not LotteryItem && pool[i].IsEmpty()))
                {
                    pool.RemoveAt(i);
                    weights.RemoveAt(i);
                }

                return result;
            }
        }

        //如果所有权重值均为0，则抛出异常
        throw new ArgumentException("EMPTY");
    }

    /// <summary>
    /// 帮助函数，一次抽取多个物品
    /// </summary>
    /// <param name="rng">随机数生成器</param>
    /// <param name="count">抽取物品数量</param>
    /// <param name="withReplacement">同一物品是否可以重复抽取</param>
    /// <returns>抽取结果</returns>
    public virtual List<T> DrawRange(Func<int, int> rng, int count, bool withReplacement = true)
    {
        List<T> res = new List<T>();
        for (int i = 0; i < count; i++)
        {
            try
            {
                T drawRes = Draw(rng, withReplacement);
                res.Add(drawRes);
            }
            catch (ArgumentException)
            {
                break;
            }
        }

        return res;
    }

    /// <summary>
    /// 清空所有物品
    /// </summary>
    public virtual void Clear()
    {
        pool.Clear();
        weights.Clear();
    }

    /// <returns>池子是否空</returns>
    public virtual bool IsEmpty()
    {
        return pool.Count == 0;
    }

    /// <summary>
    /// 单一物品类，使用私有修饰符封装
    /// </summary>
    private class LotteryItem : LotteryPool<T>
    {
        /// <summary>
        /// 命中时抽取到的单一物品
        /// </summary>
        private readonly T item;

        /// <summary>
        /// 构造函数，接受一个单一物品作为参数，该物品将作为抽取结果返回
        /// </summary>
        /// <param name="item">将会抽取到的物品</param>
        public LotteryItem(T item)
        {
            this.item = item;
        }

        /// <summary>
        /// 拦截抽取逻辑，直接返回对应物品
        /// </summary>
        /// <param name="rng">随机数生成器</param>
        /// <param name="withReplacement">未用参数：是否不重复抽取</param>
        /// <returns>抽取结果</returns>
        public override T Draw(Func<int, int> rng, bool withReplacement)
        {
            return item;
        }
    }
    
    /// <summary>
    /// 缩写函数，直接从集合中抽取一个物品，该函数会自动创建一个抽奖池，并将集合中的物品添加到池中，然后进行抽取。
    /// </summary>
    /// <param name="items">物品集合</param>
    /// <param name="rng">随机数生成器</param>
    /// <returns>命中到的物品</returns>
    public static T DrawFromCollection(IEnumerable<T> items, Func<int, int> rng)
    {
        return new LotteryPool<T>().AddRange(items).Draw(rng);
    }
}