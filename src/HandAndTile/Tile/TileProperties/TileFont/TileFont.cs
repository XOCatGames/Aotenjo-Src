using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileFont : TileAttribute
    {
        public TileFont(int fontID, string nameKey, Effect effect) : base(fontID, nameKey + "_font", effect)
        {
        }

        public virtual TileFont Copy()
        {
            return this;
        }

        public static readonly TileFont PLAIN = new(0, "plain", null);

        public static readonly TileFont BLUE = new(1, "blue", ScoreEffect.MulFan(1.5, null));

        public static readonly TileFont RED = new(2, "red", ScoreEffect.AddFan(3, null));

        public static TileFont Neon() => new TileFontNeon();
        public static readonly TileFont COLORLESS = new TileFontColorless();

        public static TileFont[] Fonts() => new[]
        {
            PLAIN, BLUE, RED, Neon(), COLORLESS
        };

        public static TileFont GetFont(string id)
        {
            string toSearch = id.EndsWith("_font") ? id : $"{id}_font";
            return Fonts().First(m => m.GetRegName() == toSearch);
        }
    }
}