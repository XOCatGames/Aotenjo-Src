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

        [SerializeField] private bool hideWhenZero;

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

        public ScoreEffect HideWhenZero()
        {
            hideWhenZero = true;
            return this;
        }

        public override bool WillTrigger()
        {
            return !hideWhenZero || (supplier != null ? supplier() : value) != 0;
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

        private double ResolveValue(Player player)
        {
            if (supplier != null)
                value = supplier();
            if (player == null || effectSource == null) return value;
            // 每次先读取原始值，再让已订阅的来源监听修正本次结果；反复显示或执行不会重复折半。
            var evt = new ArtifactScoreValueEvent(player, effectSource, type, value);
            EventBus.PublishRequired(evt);
            return evt.Value;
        }

        public override string GetEffectDisplay(Func<string, string> func) =>
            GetEffectDisplay(effectSource?.RuntimeOwner, func);

        public override string GetEffectDisplay(Player player, Func<string, string> func)
        {
            double resolved = ResolveValue(player);
            return type switch
            {
                EffectType.ADD_FU =>
                    $"{string.Format(func(resolved >= 0 ? "effect_add_fu_format" : "effect_minus_fu_format"), Math.Abs(resolved).ToShortString())}",
                EffectType.ADD_FAN =>
                    $"{string.Format(func(resolved >= 0 ? "effect_add_fan_format" : "effect_minus_fan_format"), Math.Abs(resolved).ToShortString())}",
                EffectType.MUL_FAN => $"{string.Format(func("effect_mul_fan_format"), resolved.ToShortString())}",
                _ => throw new InvalidOperationException("Invalid EffectType for ScoreEffect"),
            };
        }

        public override void Ingest(Player player)
        {
            double resolved = ResolveValue(player);
            Score score = player.RoundAccumulatedScore;

            switch (type)
            {
                case EffectType.ADD_FU: player.RoundAccumulatedScore = score.AddFu(resolved); break;
                case EffectType.ADD_FAN: player.RoundAccumulatedScore = score.AddFan(resolved); break;
                case EffectType.MUL_FAN: player.RoundAccumulatedScore = score.MultiplyFan(resolved); break;
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
