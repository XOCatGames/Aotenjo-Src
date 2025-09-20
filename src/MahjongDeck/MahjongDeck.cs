using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public abstract class MahjongDeck : IUnlockable
    {
        public string regName;
        public string yakuRange;
        public string augmentRange;
        public string difficultyLvl;
        public int id;


        public MahjongDeck(string name, string yakuRange, string augmentRange, string difficultyLvl, int id)
        {
            regName = name;
            this.yakuRange = yakuRange;
            this.augmentRange = augmentRange;
            this.difficultyLvl = difficultyLvl;
            this.id = id;
        }

        public List<Yaku> GetAvailableYakus()
        {
            return YakuTester.InfoMap.Values.Where(yaku => yaku.AvailableIn(yakuRange)).ToList();
        }

        public int GetBasicMaterialSpriteID()
        {
            return id * 10;
        }

        public abstract Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel);
        public abstract bool IsUnlocked(PlayerStats stats);

        public string GetRegName()
        {
            return regName;
        }

        public virtual bool HasYakuType(YakuType yakuType)
        {
            return GetAvailableYakus().Any(yaku => yaku.GetYakuType() == yakuType);
        }

        public string GetAttributeName(Func<string, string> localizer)
        {
            return localizer($"deck_attribute_{regName}_name");
        }

        public string GetAttributeDescription(Func<string, string> localizer)
        {
            return localizer($"deck_attribute_{regName}_description");
        }

        public virtual MaterialSet[] GetAvailableMaterialSets()
        {
            return MaterialSet.MaterialSets.Except(new []{ MaterialSet.Basic }).ToArray();
        }

        public string GetLocalizedName(Func<string, string> gameLoc)
        {
            return gameLoc($"deck_{regName}_name");
        }
        
        public string GetLockedDescription(Func<string, string> localize)
        {
            return localize($"deck_{regName}_description_locked");
        }

        public virtual string GetAscensionDescription(int ascensionIndex, Func<string, string> loc)
        {
            return loc($"ascension_{ascensionIndex}_description");
        }

        public static MahjongDeck GreenDeck = new GreenDeck();
        public static MahjongDeck BlueDeck = new BlueDeck();
        public static MahjongDeck GalaxyDeck = new GalaxyDeck();
        public static MahjongDeck BambooDeck = new BambooDeck();
        public static MahjongDeck RainbowDeck = new RainbowDeck();
        public static MahjongDeck SneakyDeck = new SneakyDeck();
        public static MahjongDeck ScarletDeck = new ScarletDeck();
        public static MahjongDeck YingYangDeck = new YingYangDeck();
        public static MahjongDeck HuluDeck = new GourdDeck();
        public static MahjongDeck OracleDeck = new OracleDeck();

        public static MahjongDeck[] decks =
        {
            GreenDeck, BlueDeck, GalaxyDeck, BambooDeck, RainbowDeck, ScarletDeck, SneakyDeck, YingYangDeck, HuluDeck, OracleDeck
        };

        public static MahjongDeck[] ascensionDecks =
        {
            BlueDeck, GalaxyDeck, BambooDeck, RainbowDeck, SneakyDeck, ScarletDeck, YingYangDeck, HuluDeck, OracleDeck
        };

    }
}