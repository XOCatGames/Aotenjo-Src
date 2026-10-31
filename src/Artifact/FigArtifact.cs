using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 仅五彩玩家按已打出花牌集合判断；其他套牌即使没有花牌也不会获得该加成。
    /// </summary>
    public sealed class FigArtifact : Artifact
    {
        public FigArtifact() : base("fig", Rarity.COMMON)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            if (player is not RainbowDeck.RainbowPlayer rainbowPlayer) return;
            if (!rainbowPlayer.PlayedFlowerTiles.Any()) effects.Add(ScoreEffect.AddFu(50, this));
        }
    }
}
