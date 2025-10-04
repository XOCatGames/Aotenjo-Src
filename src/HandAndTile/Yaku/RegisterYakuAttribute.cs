using System;

namespace Aotenjo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RegisterYakuAttribute : Attribute
    {
        public FixedYakuType FixedYaku { get; }
        public string CustomId { get; }

        // 固定番种
        public RegisterYakuAttribute(FixedYakuType fixedYaku)
        {
            FixedYaku = fixedYaku;
            CustomId = null;
        }

        // 自定义番种
        public RegisterYakuAttribute(string customId)
        {
            FixedYaku = FixedYakuType.CustomYaku;
            CustomId = customId;
        }
    }
}