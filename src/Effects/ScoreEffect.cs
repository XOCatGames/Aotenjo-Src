using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class ScoreEffect : Effect
    {
        [SerializeField] public EffectType type;
        [SerializeField] public double value;

        public delegate double ValueSupplier();

        private ValueSupplier supplier;

        [SerializeField] private Artifact effectSource;

        private ScoreEffect(EffectType type, double value, Artifact effectSource)
        {
            this.type = type;
            this.value = value;
            this.effectSource = effectSource;
            supplier = (() => value);
        }

        private ScoreEffect(EffectType type, ValueSupplier supplier, Artifact effectSource)
        {
            this.type = type;
            this.effectSource = effectSource;
            this.supplier = supplier;
        }

        protected ScoreEffect()
        {
        }

        public override bool NoDefaultSound()
        {
            return type == EffectType.ADD_FAN;
        }

        public override string GetSoundEffectName()
        {
            return type switch
            {
                EffectType.ADD_FU => "AddFu",
                EffectType.ADD_FAN => "AddFan",
                EffectType.MUL_FAN => "MulFan",
                _ => throw new InvalidOperationException("Invalid EffectType for ScoreEffect"),
            };
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            if (supplier != null)
                value = supplier();
            return type switch
            {
                EffectType.ADD_FU =>
                    $"{string.Format(func(value >= 0 ? "effect_add_fu_format" : "effect_minus_fu_format"), Math.Abs(value).ToShortString())}",
                EffectType.ADD_FAN =>
                    $"{string.Format(func(value >= 0 ? "effect_add_fan_format" : "effect_minus_fan_format"), Math.Abs(value).ToShortString())}",
                EffectType.MUL_FAN => $"{string.Format(func("effect_mul_fan_format"), value.ToShortString())}",
                _ => throw new InvalidOperationException("Invalid EffectType for ScoreEffect"),
            };
        }

        public override void Ingest(Player player)
        {
            if (supplier != null)
                value = supplier();
            Score score = player.RoundAccumulatedScore;

            switch (type)
            {
                case EffectType.ADD_FU: player.RoundAccumulatedScore = score.AddFu(value); break;
                case EffectType.ADD_FAN: player.RoundAccumulatedScore = score.AddFan(value); break;
                case EffectType.MUL_FAN: player.RoundAccumulatedScore = score.MultiplyFan(value); break;
                default: throw new InvalidOperationException("Invalid EffectType for ScoreEffect");
            }
        }

        public override Artifact GetEffectSource()
        {
            return effectSource;
        }

        public static ScoreEffect AddFu(double val, Artifact source)
        {
            return new ScoreEffect(EffectType.ADD_FU, val, source);
        }

        public static ScoreEffect AddFu(ValueSupplier val, Artifact source)
        {
            return new ScoreEffect(EffectType.ADD_FU, val, source);
        }

        public static ScoreEffect AddFan(double val, Artifact source)
        {
            return new ScoreEffect(EffectType.ADD_FAN, val, source);
        }

        public static ScoreEffect AddFan(ValueSupplier val, Artifact source)
        {
            return new ScoreEffect(EffectType.ADD_FAN, val, source);
        }

        public static ScoreEffect MulFan(double val, Artifact source)
        {
            return new ScoreEffect(EffectType.MUL_FAN, val, source);
        }

        public static ScoreEffect MulFan(ValueSupplier val, Artifact source)
        {
            return new ScoreEffect(EffectType.MUL_FAN, val, source);
        }

        public enum EffectType
        {
            ADD_FU,
            ADD_FAN,
            MUL_FAN
        }
    }
}