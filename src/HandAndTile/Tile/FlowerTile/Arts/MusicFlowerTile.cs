using System;
using System.Collections.Generic;
using Aotenjo;
using UnityEngine;

/// <summary>
/// 花牌 - 四艺：琴 复制上一枚打出花牌的所有效果
/// </summary>
[Serializable]
public class MusicFlowerTile : FlowerTile
{
    /// <summary>
    /// 当前复制的花牌, 使用装饰器模式桥接所有逻辑至内置的复制对象
    /// </summary>
    [SerializeReference] public FlowerTile mimicTile;

    public MusicFlowerTile() : base(Category.SiYi, 1)
    {
        mimicTile = null;
    }

    /// <summary>
    /// 若还未打出任何花牌, 则显示默认描述, 否则显示复制对象的描述
    /// </summary>
    /// <param name="loc">本地化函数</param>
    /// <returns>装饰后的复制牌描述</returns>
    public override string GetFlowerDescription(Func<string, string> loc)
    {
        if (mimicTile == null || mimicTile.GetBaseOrder() == 0)
        {
            return loc("tile_1h_description_plain");
        }

        return string.Format(base.GetFlowerDescription(loc), mimicTile.GetLocalizedName(loc),
            mimicTile.GetFlowerDescription(loc));
    }

    /// <summary>
    /// 订阅玩家事件时调用复制对象的订阅函数
    /// </summary>
    /// <param name="player">玩家实例</param>
    public override void SubscribeToPlayerEvents(Player player)
    {
        base.SubscribeToPlayerEvents(player);
        RainbowDeck.RainbowPlayer rainBowPlayer = (RainbowDeck.RainbowPlayer)player;
        rainBowPlayer.PostPlayFlowerTileEvent += OnPostPlayFlowerTileEvent;
    }

    /// <summary>
    /// 打出其他花牌后将复制对象转换为正在打出的花牌的一份Copy
    /// </summary>
    /// <param name="player">玩家实例</param>
    /// <param name="flowerTile">玩家打出的花牌</param>
    private void OnPostPlayFlowerTileEvent(Player player, FlowerTile flowerTile)
    {
        RainbowDeck.RainbowPlayer rainbowPlayer = (RainbowDeck.RainbowPlayer)player;

        if (flowerTile == this) return;
        if (rainbowPlayer.PlayedFlowerTiles.Contains(this)) return;

        //若此牌已有复制对象, 则将已有的复制对象订阅的玩家事件取消订阅
        if (mimicTile != null && mimicTile.GetBaseOrder() != 0)
            mimicTile.UnsubscribeFromPlayer(player);

        //若打出的是另一枚 四艺：琴，则将此牌的复制对象设置为另一枚琴的复制对象的复制
        if (flowerTile is MusicFlowerTile musicFlowerTile)
        {
            if (musicFlowerTile.mimicTile != null && musicFlowerTile.mimicTile.GetBaseOrder() != 0)
            {
                mimicTile = musicFlowerTile.mimicTile.Copy();
            }
        }
        //否则直接复制这枚打出的花牌
        else
        {
            mimicTile = flowerTile.Copy();
        }

        //使复制对象订阅玩家事件
        mimicTile?.SubscribeToPlayerEvents(player);
    }

    /// <summary>
    /// 取消订阅玩家事件时, 连带复制对象一同取消订阅避免副作用
    /// </summary>
    /// <param name="player">玩家实例</param>
    public override void UnsubscribeFromPlayer(Player player)
    {
        base.UnsubscribeFromPlayer(player);
        RainbowDeck.RainbowPlayer rainBowPlayer = (RainbowDeck.RainbowPlayer)player;
        rainBowPlayer.PostPlayFlowerTileEvent -= OnPostPlayFlowerTileEvent;
        mimicTile?.UnsubscribeFromPlayer(player);
    }

    //模仿一切复制对象的行为

    public override void OnPlayed(Player player, Permutation perm)
    {
        base.OnPlayed(player, perm);
        mimicTile?.OnPlayed(player, perm);
    }

    public override void AppendRoundEndEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendRoundEndEffect(effects, player, perm);
        mimicTile?.AppendRoundEndEffect(effects, player, perm);
    }

    public override void AppendScoringEffect(List<IAnimationEffect> effects, Player player, Permutation perm)
    {
        base.AppendScoringEffect(effects, player, perm);
        mimicTile?.AppendScoringEffect(effects, player, perm);
    }
}