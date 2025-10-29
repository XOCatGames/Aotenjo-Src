using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class WildcardWindVaneArtifact : CraftableArtifact
    {
        public WildcardWindVaneArtifact() : base("wildcard_windvane", Rarity.RARE)
        {
            SetHighlightRequirement((tile, player) => tile.IsPlayerWind(player));
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), WindVaneArtifact.MONEY, WindVaneArtifact.FAN,
                WindVaneArtifact.FU);
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (tile.GetCategory() != Tile.Category.Feng || !player.IsPlayerWind(tile.GetOrder())) return;
            if (player.Selecting(tile))
            {
                effects.Add(new EarnMoneyEffect(WindVaneArtifact.MONEY, this));
            }

            effects.Add(ScoreEffect.AddFu(WindVaneArtifact.FU, this));
            effects.Add(ScoreEffect.AddFan(WindVaneArtifact.FAN, this));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.DeterminePlayerWindEvent += DetermineWind;
            player.DeterminePrevalentWindEvent += DetermineWind;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DeterminePlayerWindEvent -= DetermineWind;
            player.DeterminePrevalentWindEvent -= DetermineWind;
        }

        private void DetermineWind(PlayerEvent playerEvent)
        {
            playerEvent.canceled = false;
        }
    }
}