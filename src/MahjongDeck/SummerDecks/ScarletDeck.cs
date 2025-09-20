using System;
using System.Collections.Generic;
using System.Linq;
using static Aotenjo.Tile;

namespace Aotenjo
{
    public class ScarletDeck : MahjongDeck
    {
        public ScarletDeck() : base("scarlet", "scarlet", "ore", "intermediate", 6)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            return new ScarletPlayer(Hand.NumberedFullHand().tiles, seed, this, set, asensionLevel);
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return stats.GetUnseededRunRecords().Any(rec => (rec.acsensionLevel >= 3 && rec.won) || !rec.roundStats.PlayedHonorTile()) || 
                   Constants.DEBUG_MODE;
        }
    }

    [Serializable]
    public class ScarletPlayer : Player
    {
        public delegate void PlayerGetScarletCoreLevelEventListener(PlayerGetScarletCoreLevelEvent evt);

        public event PlayerGetScarletCoreLevelEventListener GetScarletCoreLevelEvent;

        public delegate void PlayerDetermineScarletCoreCategoryEventListener(
            PlayerDetermineScarletCoreCategoryEvent evt);

        public event PlayerDetermineScarletCoreCategoryEventListener DetermineScarletCoreCategoryEvent;

        public ScarletPlayer(List<Tile> tilePool, string randomSeed, MahjongDeck deck, MaterialSet set,
            int ascensionLevel) :
            base(tilePool, randomSeed, deck, PlayerProperties.DEFAULT, 0, SkillSet.ScarletSkillSet(), set,
                ascensionLevel)
        {
            ObtainArtifact(Artifacts.ScarletCore);
        }

        public ScarletCoreArtifact GetScarletCore()
        {
            if (GetArtifacts().Count(a => (a is ScarletCoreArtifact)) != 1)
                throw new Exception("ScarletPlayer does not have a ScarletCoreArtifact");
            return (ScarletCoreArtifact)GetArtifacts().First(a => a is ScarletCoreArtifact);
        }

        public bool IsCompatibleWithMainCategory(Category category)
        {
            var core = GetScarletCore().data;
            bool baseResult = category == core.mainCategory;

            var evt = new PlayerDetermineScarletCoreCategoryEvent(this, category, ScarletCategoryType.Main, baseResult);
            DetermineScarletCoreCategoryEvent?.Invoke(evt);
            return evt.rawResult;
        }

        public bool IsCompatibleWithSubCategory(Category category)
        {
            var core = GetScarletCore().data;
            bool baseResult = category == core.subCategory;

            var evt = new PlayerDetermineScarletCoreCategoryEvent(this, category, ScarletCategoryType.Sub, baseResult);
            DetermineScarletCoreCategoryEvent?.Invoke(evt);
            return evt.rawResult;
        }

        public bool IsCompatibleWithUnusedCategory(Category category)
        {
            //TODO: 看看需不需要加个事件支持篡改逻辑
            return category == GetScarletCore().data.unusedCategory;
        }

        public void ChangeAndUpgradeMainCategory(Category main, Category sub)
        {
            ScarletCoreArtifact scarletCore = GetScarletCore();
            ScarletCoreArtifact.ScarletCoreArtifactData data = scarletCore.data;

            if (data.mainCategory != main) ObtainSwitchMainCategoryReward(main);

            data.mainCategory = main;
            data.subCategory = sub;

            switch (main)
            {
                case Category.Wan:
                    if (data.wanLevel < 4)
                        data.wanLevel += 1;
                    break;
                case Category.Suo:
                    if (data.suoLevel < 4)
                        data.suoLevel += 1;
                    break;
                case Category.Bing:
                    if (data.bingLevel < 4)
                        data.bingLevel += 1;
                    break;
                default:
                    throw new Exception("Invalid main category selected");
            }

            data.unusedCategory = new[] { Category.Bing, Category.Wan, Category.Suo }
                .FirstOrDefault(c => c != main && c != sub);
        }

        private void ObtainSwitchMainCategoryReward(Category main)
        {
        }

        public int GetCategoryBaseLevel(Category category)
        {
            return GetScarletCore().GetCategoryLevel(category);
        }

        public int GetCategoryLevel(Category category, string source = "")
        {
            int baseLevel = GetCategoryBaseLevel(category);
            PlayerGetScarletCoreLevelEvent evt = new PlayerGetScarletCoreLevelEvent(this, category, baseLevel);
            evt.message = source;
            GetScarletCoreLevelEvent?.Invoke(evt);
            return Math.Min(evt.level, 4);
        }

        public override BlockCombinator GetCombinator()
        {
            if (GetArtifacts().Contains(Artifacts.DottledPig)) return BlockCombinator.Default;
            ScarletCoreArtifact scarletCore = GetScarletCore();
            ScarletCoreArtifact.ScarletCoreArtifactData data = scarletCore.data;
            return BlockCombinator.Scarlet(data.unusedCategory);
        }

        public override List<YakuPack> TryDrawYakuPack(int v, List<YakuPack> yakuPacks)
        {
            yakuPacks.RemoveAll(t => t.id == 0);
            return base.TryDrawYakuPack(v, yakuPacks);
        }

        public (Category, Category)[] GetNewSuitCombinations()
        {
            Category[] AllCategories = { Category.Wan, Category.Suo, Category.Bing };

            (Category, Category)[] AllPairs = AllCategories
                .SelectMany(c1 => AllCategories.Where(c2 => c2 != c1).Select(c2 => (c1, c2))).ToArray();

            var pair1 = AllPairs[GenerateRandomInt(AllPairs.Length)];
            (Category, Category)[] lefts = AllPairs
                .Where(p => p.Item1 != pair1.Item1 && !(p.Item1 == pair1.Item2 && p.Item2 == pair1.Item1)).ToArray();
            var pair2 = lefts[GenerateRandomInt(lefts.Length)];
            return new[] { pair1, pair2 };
        }

        public override List<Tile> GetFullDeck()
        {
            return Hand.NumberedFullHand().tiles;
        }

        public override List<Tile> GetUniqueFullDeck()
        {
            return Hand.NumberedUniqueHand().tiles;
        }

        public override int DiscardTile(Tile tile, bool forced)
        {
            if (IsCompatibleWithUnusedCategory(tile.GetCategory()))
                stats.RecordCustomStats("discard_abandoned_suit", 1);
            return base.DiscardTile(tile, forced);
        }

        public override bool GenerateRandomDeterminationResult(int v)
        {
            if (GetCategoryLevel(Category.Bing) >= 2)
            {
                return GenerateRandomInt(v) <= 1;
            }

            return base.GenerateRandomDeterminationResult(v);
        }
    }
}