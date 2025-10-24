using System;
using System.Collections.Generic;
using Aotenjo;

[Serializable]
public class FlowerTile : Tile
{
    /// <summary>
    /// 花牌代码由1-4f,1-4g,1-4h,1-4i组成，各自分别代表 春夏秋冬 梅兰菊竹 琴棋书画 渔樵耕读
    /// </summary>
    /// <param name="representation"></param>
    protected FlowerTile(Category category, int order) : base(category, order)
    {
        if (category < Category.SiJi || order < 1 || order > 4)
        {
            throw new Exception("Invalid flower code received");
        }
    }

    public virtual FlowerTile Copy()
    {
        return (FlowerTile)FromCategoryAndOrder(category, GetBaseOrder());
    }

    public virtual string GetFlowerDescription(Func<string, string> loc)
    {
        return loc($"flower_{this}_description");
    }

    public virtual void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
    }

    public virtual void OnPlayed(Player player, Permutation perm)
    {
    }

    public virtual void AppendRoundEndEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
    }

    public override bool CompatWith(Tile cand)
    {
        return cand == this;
    }

    public override bool CompatWithCategory(Category category)
    {
        return false;
    }

    public override bool IsSameCategory(Tile cand)
    {
        return false;
    }

    public override bool IsSameOrder(Tile cand)
    {
        return false;
    }

    public static Tile FromCategoryAndOrder(Category category, int ord)
    {
        if (category == Category.SiJi)
        {
            return ord switch
            {
                1 => new SpringFlowerTile(),
                2 => new SummerFlowerTile(),
                3 => new AutumnFlowerTile(),
                4 => new WinterFlowerTile(),
                _ => new FlowerTile(category, ord)
            };
        }

        if (category == Category.JunZi)
        {
            return ord switch
            {
                1 => new PlumFlowerTile(),
                2 => new OrchidFlowerTile(),
                3 => new JuHuaFlowerTile(),
                4 => new BambooFlowerTile(),
                _ => new FlowerTile(category, ord)
            };
        }

        if (category == Category.SiYi)
        {
            return ord switch
            {
                1 => new MusicFlowerTile(),
                2 => new QiFlowerTile(),
                3 => new BookFlowerTile(),
                4 => new DrawingFlowerTile(),
                _ => new FlowerTile(category, ord)
            };
        }

        if (category == Category.SiYe)
        {
            return ord switch
            {
                1 => new FishingFlowerTile(),
                2 => new LoggingFlowerTile(),
                3 => new FarmingFlowerTile(),
                4 => new ReadingFlowerTile(),
                _ => new FlowerTile(category, ord)
            };
        }

        return new FlowerTile(category, ord);
    }
}