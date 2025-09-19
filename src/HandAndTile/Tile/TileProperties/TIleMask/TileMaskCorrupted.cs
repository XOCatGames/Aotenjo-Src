using System;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaskCorrupted : TileMask
    {
        public const float intensityPerRound = 1f;

        public TileMaskCorrupted(int id) : base(id, "corrupted", null)
        {
        }

        public override TileMask Copy()
        {
            return new TileMaskCorrupted(0);
        }

        public override bool IsDebuff()
        {
            return true;
        }

        public override Effect[] GetEffects()
        {
            return new Effect[] { new CorruptedEffect() };
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            int round = player.GetPrevalentWind() + 4 * ((player.Level - 1) / 16);
            return string.Format(base.GetDescription(localizer), GetIntensity(round));
        }

        private static float GetIntensity(int round)
        {
            float coeff = round switch
            {
                1 => 1f,
                2 => 2f,
                3 => 6f,
                4 => 12f,
                _ => (round - 4) * 6 + 6
            };
            return coeff * intensityPerRound;
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            player.PreDiscardTileEvent += PreDiscardEventListener;
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            player.PreDiscardTileEvent -= PreDiscardEventListener;
        }

        public void PreDiscardEventListener(PlayerDiscardTileEvent.Pre eventData)
        {
            if (eventData.canceled) return;

            Tile originalTile = eventData.tile;
            if (originalTile.properties.mask != this) return;
            Player player = eventData.player;

            eventData.canceled = true;

            originalTile.SetMask(NONE, player);

            if (!eventData.keepPos)
                player.SortDeck();
        }

        public class CorruptedEffect : Effect
        {
            public float coefficient = -1;

            public override void Ingest(Player player)
            {
                int round = 1 + (player.Level - 1) / 4;
                double intensity = GetIntensity(round);
                player.RoundAccumulatedScore = player.RoundAccumulatedScore.AddFan(coefficient * intensity);
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return "ERROR";
            }

            public override string GetEffectDisplay(Player player, Func<string, string> func)
            {
                int round = 1 + (player.Level - 1) / 4;
                float intensity = GetIntensity(round);
                return string.Format(func(coefficient > 0 ? "effect_add_fan_format" : "effect_minus_fan_format"),
                    Mathf.Abs(coefficient * intensity));
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }
        }
    }
}