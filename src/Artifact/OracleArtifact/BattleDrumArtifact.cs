using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class BattleDrumArtifact : Artifact
    {
        private const double FU = 40;
        
        public BattleDrumArtifact() : base("battle_drum", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), FU);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            effects.Add(ScoreEffect.AddFu(FU, this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Subscribe<OraclePlayer.PlayerSetOracleEvent.Final>(FinalSetOracle);
        }
        
        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if(player is OraclePlayer)
                EventBus.Unsubscribe<OraclePlayer.PlayerSetOracleEvent.Final>(FinalSetOracle);
        }

        private void FinalSetOracle(OraclePlayer.PlayerSetOracleEvent.Final obj)
        {
            var tiles = obj.permutation.blocks[0].tiles.ToList();
            foreach (var t in tiles)
            {
                obj.player.AddNewTileToPool(t);
            }
        }
    }
}