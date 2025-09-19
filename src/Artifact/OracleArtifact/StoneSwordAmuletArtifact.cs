using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class StoneSwordAmuletArtifact : Artifact
    {
        private const double FU_DEDUCTION = 40;
        public StoneSwordAmuletArtifact() : base("stone_sword", Rarity.COMMON)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU_DEDUCTION);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Subscribe<OraclePlayer.PlayerSetOracleEvent.Pre>(PreSetOracle);
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Unsubscribe<OraclePlayer.PlayerSetOracleEvent.Pre>(PreSetOracle);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFu(-FU_DEDUCTION, this));
        }

        private void PreSetOracle(OraclePlayer.PlayerSetOracleEvent.Pre obj)
        {
            var fstTile = obj.block.tiles[0];
            obj.block = new Block(new[]
                { new Tile(fstTile), new Tile(fstTile), new Tile(fstTile), new Tile(fstTile) });
        }
    }
}