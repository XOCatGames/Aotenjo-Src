using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStats
{
    [SerializeField] public int maxLevel;

    [SerializeField] private double maxScore;
    
    
    [FormerlySerializedAs("yakuCounts")]
    [SerializeField] private SerializableMap<FixedYakuType, int> legacyYakuCounts;

    [FormerlySerializedAs("maxYakuLevel")]
    [SerializeField] private SerializableMap<FixedYakuType, int> legacyMaxYakuLevel;
    
    [SerializeField] private SerializableMap<YakuType, int> _yakuCountsMap;
    [SerializeField] private SerializableMap<YakuType, int> _maxYakuLevelMap;
    
    private SerializableMap<YakuType, int> yakuCountsMap
    {
        get
        {
            if (legacyYakuCounts != null && !legacyYakuCounts.IsEmpty() && _yakuCountsMap.IsEmpty())
            {
                foreach (var key in legacyYakuCounts.GetKeys())
                {
                    _yakuCountsMap.Add(new YakuType(key), legacyYakuCounts.Get(key));
                }
                legacyYakuCounts = new SerializableMap<FixedYakuType, int>();
            }
            return _yakuCountsMap;
        }
        set { _yakuCountsMap = value; }
    }
    
    private SerializableMap<YakuType, int> maxYakuLevelMap
    {
        get
        {
            if (legacyMaxYakuLevel != null && !legacyMaxYakuLevel.IsEmpty() && _maxYakuLevelMap.IsEmpty())
            {
                foreach (var key in legacyMaxYakuLevel.GetKeys())
                {
                    _maxYakuLevelMap.Add(new YakuType(key), legacyMaxYakuLevel.Get(key));
                }
                legacyMaxYakuLevel = new SerializableMap<FixedYakuType, int>();
            }
            return _maxYakuLevelMap;
        }
        set { _maxYakuLevelMap = value; }
    }

    [SerializeField] private long moneyEarned;

    [SerializeField] private long moneySpent;

    [SerializeField] private SerializableMap<string, int> tileMaterialObtainedCount;

    [SerializeField] private SerializableMap<string, int> tileFontPlayedCount;

    [SerializeField] private SerializableMap<string, int> tileMaskPlayedCount;

    [SerializeField] private SerializableMap<string, int> categoryPlayedCount;

    [SerializeField] private SerializableMap<int, int> artifactBoughtCount;

    [SerializeField] private SerializableMap<string, int> gadgetBoughtCount;

    [SerializeField] private List<RoundRecord> roundRecords;

    /// <summary>
    /// 常规出牌记录不记录分数占比
    /// </summary>
    [SerializeField] private List<SettleRecord> playSequence;

    [SerializeField] private SerializableMap<string, int> customStats;

    /// <summary>
    /// 上一次出牌记录 记录raw番数占比
    /// </summary>
    [SerializeField] public SettleRecord lastSettleRecord;

    [SerializeField] public SettleRecord bestSettleRecord;

    private PlayerStats()
    {
        maxLevel = 0;
        maxScore = 0;
        yakuCountsMap = new SerializableMap<YakuType, int>();
        maxYakuLevelMap = new SerializableMap<YakuType, int>();
        moneyEarned = 0;
        moneySpent = 0;
        tileMaterialObtainedCount = new SerializableMap<string, int>();
        tileFontPlayedCount = new SerializableMap<string, int>();
        tileMaskPlayedCount = new SerializableMap<string, int>();
        categoryPlayedCount = new SerializableMap<string, int>();
        artifactBoughtCount = new SerializableMap<int, int>();
        gadgetBoughtCount = new SerializableMap<string, int>();
        customStats = new SerializableMap<string, int>();

        roundRecords = new List<RoundRecord>();
        playSequence = new List<SettleRecord>();

        bestSettleRecord = new SettleRecord();
        lastSettleRecord = new SettleRecord();
    }

    public static PlayerStats New()
    {
        return new PlayerStats();
    }

    public void RecordCustomStats(string key, int count)
    {
        customStats.Add(key, customStats.Get(key) + count);
    }
    
    public void RecordCustomStats(PlayerStatsType key, int count)
    {
        RecordCustomStats(key.ToString().ToLower(), count);
    }

    public int GetCustomStats(string key)
    {
        return customStats.Get(key);
    }
    public int GetCustomStats(PlayerStatsType key)
    {
        return customStats.Get(key.ToString().ToLower());
    }
    public void RecordRound(Player player, string explicitMatSetName)
    {
        PlayerStats roundStat = player.stats;

        roundRecords.Add(new RoundRecord(roundStat,
            player.deck.regName,
            player.GetAscensionLevel(),
            player.won,
            player.materialSet.regName,
            player.GetArtifacts(),
            player.randomSeed,
            player.seededRun,
            explicitMatSetName)
        );

        if (player.seededRun) return;

        maxLevel = Math.Max(maxLevel, roundStat.maxLevel);
        maxScore = Math.Max(maxScore, roundStat.maxScore);
        moneyEarned += roundStat.moneyEarned;
        moneySpent += roundStat.moneySpent;

        playSequence.AddRange(roundStat.playSequence);

        foreach (var key in roundStat.customStats.GetKeys())
        {
            customStats.Add(key, customStats.Get(key) + roundStat.customStats.Get(key));
        }

        foreach (var yaku in roundStat.yakuCountsMap.GetKeys())
        {
            yakuCountsMap.Add(yaku, yakuCountsMap.Get(yaku) + roundStat.yakuCountsMap.Get(yaku));
            //Debug.Log($"{yaku.ToString()} : {yakuCounts.Get(yaku)}");
        }

        foreach (var tile in roundStat.tileMaterialObtainedCount.GetKeys())
        {
            tileMaterialObtainedCount.Add(tile,
                tileMaterialObtainedCount.Get(tile) + roundStat.tileMaterialObtainedCount.Get(tile));
        }

        foreach (var tile in roundStat.tileFontPlayedCount.GetKeys())
        {
            tileFontPlayedCount.Add(tile, tileFontPlayedCount.Get(tile) + roundStat.tileFontPlayedCount.Get(tile));
        }

        foreach (var tile in roundStat.tileMaskPlayedCount.GetKeys())
        {
            tileMaskPlayedCount.Add(tile, tileMaskPlayedCount.Get(tile) + roundStat.tileMaskPlayedCount.Get(tile));
        }

        foreach (var category in roundStat.categoryPlayedCount.GetKeys())
        {
            categoryPlayedCount.Add(category,
                categoryPlayedCount.Get(category) + roundStat.categoryPlayedCount.Get(category));
        }

        foreach (var artifact in roundStat.artifactBoughtCount.GetKeys())
        {
            artifactBoughtCount.Add(artifact,
                artifactBoughtCount.Get(artifact) + roundStat.artifactBoughtCount.Get(artifact));
        }

        foreach (var yaku in roundStat.maxYakuLevelMap.GetKeys())
        {
            maxYakuLevelMap.Add(yaku, Math.Max(maxYakuLevelMap.Get(yaku), roundStat.maxYakuLevelMap.Get(yaku)));
        }


        foreach (var gadget in roundStat.gadgetBoughtCount.GetKeys())
        {
            gadgetBoughtCount.Add(gadget, gadgetBoughtCount.Get(gadget) + roundStat.gadgetBoughtCount.Get(gadget));
        }
    }

    public int GetWonNumberByDeck(string deckName, int ascension = 0)
    {
        return roundRecords.Count(rec => !rec.seeded && (rec.won || rec.roundStats.maxLevel > 16) && rec.deckName.Equals(deckName) &&
                                         rec.acsensionLevel >= ascension);
    }

    public List<RoundRecord> GetRunRecords()
    {
        return new List<RoundRecord>(roundRecords);
    }

    public List<RoundRecord> GetUnseededRunRecords()
    {
        return new List<RoundRecord>(roundRecords.Where(r => !r.seeded));
    }

    public void SyncPlayer(Player player)
    {
        if (player.Level > maxLevel)
        {
            maxLevel = player.Level;
        }

        if (player.CurrentAccumulatedScore > maxScore)
        {
            maxScore = player.CurrentAccumulatedScore;
        }

        foreach (var yaku in player.GetSkillSet().GetYakus())
        {
            maxYakuLevelMap.Add(yaku, Math.Max(maxYakuLevelMap.Get(yaku), player.GetSkillSet().GetLevel(yaku)));
        }
    }

    public void RecordPlay(Permutation permutation, Player player, List<YakuType> activatedYakus, Score score)
    {
        SyncPlayer(player);
        
        foreach (var yakuType in activatedYakus)
        {
            yakuCountsMap.Add(yakuType, yakuCountsMap.Get(yakuType) + 1);
        }

        playSequence.Add(new SettleRecord(player.Level, player.CurrentPlayingStage,
            permutation.GetPermType(), permutation.ToTiles(), new(player.GetSelectedTilesCopy()), new(activatedYakus),
            score));

        foreach (Tile tile in permutation.ToTiles())
        {
            TileProperties properties = tile.properties;
            properties.material.GetLocalizeKey();
            
            string fontName = properties.font.GetLocalizeKey();
            string maskName = properties.mask.GetLocalizeKey();
            string cateName = Tile.CategoryToNameKey(tile.GetCategory());

            tileFontPlayedCount.Add(fontName, tileFontPlayedCount.Get(fontName) + 1);
            tileMaskPlayedCount.Add(maskName, tileMaskPlayedCount.Get(maskName) + 1);
            categoryPlayedCount.Add(cateName, categoryPlayedCount.Get(Tile.CategoryToNameKey(tile.GetCategory())) + 1);
        }
        
        //特殊出牌记录
        if (activatedYakus.Contains(FixedYakuType.WuMenQi) && player.Level == 16)
        {
            RecordCustomStats(PlayerStatsType.WUMENQI_WIN, 1);
        }

        if (playSequence.Count >= 2)
        {
            string hand1 = string.Join("", permutation.ToTiles().Select(t => t.ToString()).OrderBy(s => s));
            string hand2 = string.Join("", playSequence[^2].AllTiles.Select(t => t.ToString()).OrderBy(s => s));
            if (hand1.Equals(hand2))
            {
                RecordCustomStats(PlayerStatsType.SAME_TILES_TWO_IN_A_ROW, 1);
            }
        }
    }

    public void RecordObtainMaterial(TileMaterial tileMaterial)
    {
        tileMaterialObtainedCount.Add(tileMaterial.GetLocalizeKey(),
            tileMaterialObtainedCount.Get(tileMaterial.GetLocalizeKey()) + 1);
    }

    public int GetYakuCount(Yaku yaku)
    {
        return yakuCountsMap.Get(yaku.GetYakuType());
    }

    public double GetMaxScore()
    {
        return maxScore;
    }

    public long GetMoneyEarned()
    {
        return moneyEarned;
    }

    public string GetMostPlayedSuit(Func<string, string> localize)
    {
        string nameKey = "ui_none";
        int max = -1;

        foreach (var category in categoryPlayedCount.GetKeys())
        {
            if (categoryPlayedCount.Get(category) > max)
            {
                max = categoryPlayedCount.Get(category);
                nameKey = category;
            }
        }

        return localize(nameKey);
    }

    public YakuType GetRarestPlayedYaku()
    {
        YakuType yaku = FixedYakuType.Base;
        yakuCountsMap.GetKeys().ToList().ForEach(y =>
        {
            Yaku toCompare = YakuTester.InfoMap[y];
            Yaku currentMax = YakuTester.InfoMap[yaku];
            if (toCompare.rarity > currentMax.rarity ||
                (toCompare.fullFan > currentMax.fullFan && toCompare.rarity == currentMax.rarity))
            {
                yaku = y;
            }
        });
        return yaku;
    }

    public void MoneyEarned(int money)
    {
        moneyEarned += money;
    }

    internal void SpendMoney(int v)
    {
        moneySpent += v;
    }

    internal bool UnlockedYakuDetail(YakuType yaku)
    {
        if (Constants.DEBUG_MODE) return true;
        return PlayedYaku(yaku);
    }

    internal bool PlayedYaku(YakuType yaku)
    {
        return yakuCountsMap.Get(yaku) > 0;
    }

    public void OnObtainArtifact(Artifact artifact)
    {
        artifactBoughtCount.Add(artifact.GetNumberID(), artifactBoughtCount.Get(artifact.GetNumberID()) + 1);
    }

    public void OnBoughtGadget(Gadget gadget)
    {
        gadgetBoughtCount.Add(gadget.regName, gadgetBoughtCount.Get(gadget.regName) + 1);
    }

    public bool ArtifactOwned(Artifact artifact)
    {
        if (Constants.DEBUG_MODE) return true;
        return ArtifactBought(artifact);
    }

    public bool ArtifactBought(Artifact artifact)
    {
        return artifactBoughtCount.Get(artifact.GetNumberID()) > 0;
    }

    public int BoughtCount()
    {
        return artifactBoughtCount.GetKeys().Sum(k => artifactBoughtCount.Get(k))
               + gadgetBoughtCount.GetKeys().Sum(k => gadgetBoughtCount.Get(k));
    }

    public List<SettleRecord> GetPlayedHands()
    {
        return new(playSequence);
    }

    public bool ObtainedMaterial(TileMaterial tileMaterial)
    {
        if (Constants.DEBUG_MODE) return true;
        return tileMaterialObtainedCount.Get(tileMaterial.GetLocalizeKey()) > 0;
    }

    public int GetMaterialPlayedCount(TileMaterial tileMaterial)
    {
        return tileMaterialObtainedCount.Get(tileMaterial.GetLocalizeKey());
    }

    public int GetFontPlayedCount(TileFont tileFont)
    {
        return tileFontPlayedCount.Get(tileFont.GetLocalizeKey());
    }

    public int GetMaskPlayedCount(TileMask tileMask)
    {
        return tileMaskPlayedCount.Get(tileMask.GetLocalizeKey());
    }

    public bool PlayedHonorTile()
    {
        return categoryPlayedCount.Get(Tile.CategoryToNameKey(Tile.Category.Jian)) == 0 &&
               categoryPlayedCount.Get(Tile.CategoryToNameKey(Tile.Category.Feng)) == 0;
    }

    public void recordHand(Permutation perm, Player player, SerializableMap<YakuType, double> yakuContributions,
        Score score)
    {
        SettleRecord rec = new SettleRecord(player.Level,
            player.CurrentPlayingStage,
            perm.GetPermType(),
            perm.ToTiles().Select(t => new Tile(t)).ToList(),
            player.GetSelectedTilesCopy().Select(t => new Tile(t)).ToList(),
            yakuContributions.GetKeys().ToList(),
            score);

        rec.YakuFanMap = yakuContributions;

        lastSettleRecord = rec;
        if (bestSettleRecord.level == -1 || score.GetScore() > bestSettleRecord.score.GetScore())
        {
            bestSettleRecord = rec;
        }
    }

    public int GetHighestYakuLevel(YakuType yakuTypeID)
    {
        return maxYakuLevelMap.Get(yakuTypeID);
    }

    public int GetGadgetObtainedCount(Gadget g)
    {
        return gadgetBoughtCount.Get(g.regName);
    }

    internal bool UnlockedAscensionLevel(int i, string deckName)
    {
        return i == 0
               || (i <= 5 && GetUnseededRunRecords().Any(r => r.won && r.acsensionLevel >= 5)) //通过和级解锁所有和级以下进阶
               || GetUnseededRunRecords()
                   .Any(r => r.deckName.Equals(deckName) && r.won && r.acsensionLevel == i - 1); //通过上一级解锁
    }
}