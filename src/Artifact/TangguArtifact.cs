using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    public sealed class TangguArtifact : InstrumentArtifact, IMultiplierProvider
    {
        private const int RODS_PER_BEAT = 3;
        private const double MULTIPLIER_GAIN = 0.5;
        private int beats;
        private bool playedLastHit;

        public TangguArtifact() : base(RODS_PER_BEAT, "tanggu", Rarity.EPIC)
        {
            setIn.Add("mech_parts");
            tags.Add(ArtifactTag.INSTRUMENT);
            SetHighlightRequirement((tile, _) => CountRods(tile) > 0);
        }

        private static int CountRods(Tile tile) =>
            tile?.properties?.material is TileMaterialMechPart material
                ? material.GetPartCount(MechPartType.DriveRod) : 0;

        [SubscribeToEvent]
        private void EndRound(PlayerRoundEvent.End.Post evt)
        {
            beats = 0;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile)) return;
            int rods = CountRods(tile);
            for (int i = 0; i < rods; i++)
                effects.Add(new PlayInstrumentEffect(this));
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
            => effects.Add(ScoreEffect.MulFan(() => GetMul(player), this));

        protected override bool CanPlay(Player player, Permutation permutation, List<Effect> effects, Block block) => false;
        protected override void OnPlay(Player player) => beats++;
        public override void DecreaseCounter(Player player)
        {
            playedLastHit = IsActivating();
            base.DecreaseCounter(player);
        }

        protected override string GetEffectDisplay(Player player, Func<string, string> localizer) =>
            localizer(playedLastHit ? "effect_tanggu_play" : "effect_tanggu_hit");
        protected override string GetSoundEffectName() => "Tanggu";
        public double GetMul(Player player) => 1.0 + beats * MULTIPLIER_GAIN;

        public override (string, double) GetAdditionalDisplayingInfo(Player player) =>
            ("<style=\"mul\">X{0:F1}</style>", GetMul(player));

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(base.GetDescription(localizer), RODS_PER_BEAT,
                MULTIPLIER_GAIN.ToString("F1"), GetMul(null).ToString("F1"));

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            beats = 0;
            playedLastHit = false;
        }

        public override string Serialize() => base.Serialize() + "," + beats;
        public override void Deserialize(string data)
        {
            ResetArtifactState();
            string[] parts = (data ?? "").Split(',');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int counter) ||
                !int.TryParse(parts[1], out int savedBeats)) return;
            currentCounter = Mathf.Clamp(counter, 0, maxCounter - 1);
            beats = Math.Max(0, savedBeats);
        }
    }
}
