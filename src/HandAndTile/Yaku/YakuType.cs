using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public sealed class YakuType
    {
        [SerializeField]
        private FixedYakuType fixedYakuType;
        [SerializeField]
        private string customId;
        private static readonly Dictionary<string, YakuType> lookup = new();

        // --- 静态构造函数：类第一次被使用时运行 ---
        static YakuType()
        {
            foreach (FixedYakuType val in Enum.GetValues(typeof(FixedYakuType)))
            {
                new YakuType(val);
            }
        }
        public YakuType(FixedYakuType fixedYakuType)
        {
            this.fixedYakuType = fixedYakuType;
            this.customId = "none";
            lookup[fixedYakuType.ToString()] = this;
        }
        
        public YakuType(string customId)
        {
            this.fixedYakuType = FixedYakuType.CustomYaku;
            this.customId = customId;
            lookup[ToString()] = this;
        }

        public override bool Equals(object obj)
        {
            if (obj is YakuType other)
            {
                if (fixedYakuType != FixedYakuType.CustomYaku && other.fixedYakuType == fixedYakuType) return true;
                return fixedYakuType == other.fixedYakuType && customId == other.customId;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine((int)fixedYakuType, customId);
        }
        
        public static YakuType FromString(string key)
        {
            return lookup.TryGetValue(key, out var value) ? value :
                new YakuType(key);
        }
        
        public static bool operator ==(YakuType left, YakuType right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left is null || right is null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(YakuType left, YakuType right)
        {
            return !(left == right);
        }
        
        public override string ToString()
        {
            return fixedYakuType == FixedYakuType.CustomYaku
                ? $"custom_yaku:{customId}"
                : fixedYakuType.ToString();
        }

        public static implicit operator YakuType(FixedYakuType fixedType)
            => new YakuType(fixedType);
    }
}