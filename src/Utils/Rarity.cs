using Sirenix.OdinInspector;

namespace Aotenjo
{
    public enum Rarity
    {
        [LabelText("普通")] COMMON,

        [LabelText("稀有")] RARE,

        [LabelText("史诗")] EPIC,

        [LabelText("传说")] LEGENDARY,

        [LabelText("远古")] ANCIENT
    }
}