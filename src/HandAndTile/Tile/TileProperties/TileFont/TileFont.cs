using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileFont : TileAttribute
    {
        public readonly Rarity rarity;
        
        public TileFont(int fontID, string nameKey, Effect effect, Rarity rarity = Rarity.COMMON) : base(fontID, nameKey + "_font", effect)
        {
        }

        public virtual TileFont Copy()
        {
            return this;
        }

        public static readonly TileFont PLAIN = new(0, "plain", null);

        public static readonly TileFont BLUE = new(1, "blue", ScoreEffect.MulFan(1.5, null), Rarity.RARE);

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

        protected override string GetSpriteSheetName()
        {
            return Constants.FILE_TILE_FRONT_SHEET;
        }
        
        public override string GetSubheader(Func<string, string> loc)
        {
            string rarityName = GetRarity().ToString().ToLower();
            string rarityText = loc($"rarity_{rarityName}_name") + " " + loc("tile_font_name");
            return rarityText;
        }
    }
}