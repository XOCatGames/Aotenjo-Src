using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aotenjo;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Localization.Tables;
#endif
using static Skill;

[Serializable]
public class Yaku : ScriptableObject, IComparable<Yaku>, ITileHighlighter
{
    [LabelText("番种类型")] public YakuType type;
    
    [FormerlySerializedAs("yakuTypeID")]
    [LabelText("番种类型枚举")] public FixedYakuType fixedYakuType;

    [LabelText("稀有度"), EnumToggleButtons] public Rarity rarity;

    [LabelText("完整番数")] public int fullFan;

    [LabelText("大小调整系数"), Range(1, 4)] public double growthFactor;

    [LabelText("成长番数")] public double levelingFactor;

    [FormerlySerializedAs("includedYakus")] [LabelText("（旧）继承番种")]
    public FixedYakuType[] legacyIncludedYakus;
    
    [LabelText("继承番种")] public YakuType[] includedYakus;

    [LabelText("可用番种范围")] public string[] groups;

    [LabelText("例牌")] public string example;

    public Dictionary<SkillType, int> requiredSkillLevel = new Dictionary<SkillType, int>();
    
    [LabelText("所在番种包")] public List<int> yakuCategories = new List<int>();

    [LabelText("番种图显示顺序")] public int order;

    public Yaku(YakuType yakuType, int fullFan, double growthFactor, double levelingFactor, YakuType[] includedYakus,
        string[] groups, Rarity rarity, string example, int[] yakuCategories)
    {
        type = yakuType;
        this.fullFan = fullFan;
        this.growthFactor = growthFactor;
        this.levelingFactor = levelingFactor;
        this.includedYakus = includedYakus;
        this.groups = groups;
        this.rarity = rarity;
        this.example = example;
        this.yakuCategories = yakuCategories.ToList();
    }

    public Yaku(YakuType yakuType, int fullFan, double growthFactor, double levelingFactor, YakuType[] includedYakus,
        string[] groups, Rarity rarity, string example, int v, int[] yakuCategories) : this(yakuType, fullFan, growthFactor, levelingFactor,
        includedYakus, groups, rarity, example, yakuCategories)
    {
        order = v;
    }

    public string GetNameLocalizeKey()
    {
        return "yaku_" + type + "_name";
    }

    public void SyncType()
    {
        type = fixedYakuType;
        
        if (includedYakus == null || includedYakus.Length == 0)
        {
            includedYakus = legacyIncludedYakus.Select(a => (YakuType)a).ToArray();
            legacyIncludedYakus = new FixedYakuType[0];
        }
    }

    public string GetNameRomajiKey()
    {
        return $"yaku_{type.ToString()}_romaji_name";
    }

    public string GetDescriptionLocalizationKey()
    {
        return "yaku_" + type + "_description";
    }

    public string GetExampleText()
    {
        StringBuilder sb = new();
        sb.Append("<size=72px>");
        //往示例牌型面板添加番种的对应示例牌型
        List<Tile> tiles = new Hand(example).tiles;
        int b = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            sb.Append($"<sprite name=\"{tiles[i]}\">");
            int interval = (Utils.IsSevenPairYaku(this) ? 2 : (type == FixedYakuType.ShiSanYao ? 12 : 3));
            if (Utils.IsKongYaku(this))
                interval = 4;
            if ((i + 1) % interval == 0)
            {
                sb.Append(" ");
                b++;
            }

            if (b == 2 && tiles.Count - i > 3)
            {
                sb.Append("\n");
                b = 0;
            }
        }

        sb.Append("</size>");
        return sb.ToString();
    }

#if UNITY_EDITOR
    public string ToChineseLabel()
    {
        var table = AssetDatabase.LoadAssetAtPath<StringTable>(
            "Assets/LocalizationConfig/Tables/YakuInfo_zh-Hans.asset");
        var entry = table.GetEntry(GetNameLocalizeKey());
        return entry.GetLocalizedString();
    }
#endif

    public YakuType GetYakuType()
    {
        return type;
    }

    public int CompareTo(Yaku other)
    {
        return other.rarity - rarity == 0 ? (other.fullFan - fullFan) : other.rarity - rarity;
    }

    public bool AvailableIn(string name)
    {
        return groups.Contains(name);
    }

    public string GetFormattedName(Func<string, string> loc)
    {
        return string.Format($"<style=\"{rarity.ToString()}\">{{0}}</style>", loc(GetNameLocalizeKey()));
    }

    public string GetYakuSkillRequirementText(Func<string, string> loc)
    {
        string format = loc("ui_require_skill_level_format");
        loc("ui_require_skill_level_completed_format");
        StringBuilder sb = new StringBuilder();
        Dictionary<SkillType, int> skillMap = requiredSkillLevel;
        try
        {
            Player player = GameManager.Instance.player;

            int baseLv = GetLevel(player) - player.GetSkillSet().GetExtraLevel(type);
            foreach (SkillType skill in skillMap.Keys.Where(s => skillMap[s] > 0))
            {
                sb.AppendLine(string.Format(format, (baseLv + 1) * skillMap[skill],
                    loc($"skill_{skill.ToString()}_name"), player.skillMap.Get(skill)));
            }

            return sb.ToString();
        }
        catch
        {
            foreach (SkillType skill in skillMap.Keys.Where(s => skillMap[s] > 0))
            {
                sb.AppendLine(string.Format(format, skillMap[skill], loc($"skill_{skill.ToString()}_name"), 0));
            }

            return sb.ToString();
        }
    }

    public List<int> GetYakuCategories()
    {
        List<YakuPack> packs = new List<YakuPack>();
        if (GameManager.Instance != null) packs = GameManager.Instance.yakuPacks;
        if (MainMenu._instance != null) packs = MainMenu._instance.yakuPacks;
        return packs.Where(pack => pack.ContainsYaku(this)).Select(p => p.id).ToList();
    }

    public List<SkillType> GetYakuRequiredSkills()
    {
        return requiredSkillLevel.Keys.Where(a => requiredSkillLevel[a] > 0).ToList();
    }

    public int GetLevel(Player player)
    {
        return GetLevel(player.skillMap, player.GetExtraLevel(this));
    }

    public int GetLevel(SerializableMap<SkillType, int> map, int addonLevel)
    {
        return addonLevel;
    }

    public int GetDistToNextLevel(Player player, SkillType skill)
    {
        int currentSkillLevel = player.skillMap.Get(skill);
        if (currentSkillLevel > GetNextUpgradeRequireLevel(player, skill))
        {
            return -1;
        }

        return GetNextUpgradeRequireLevel(player, skill) - currentSkillLevel;
    }

    public int GetNextUpgradeRequireLevel(Player player, SkillType skill)
    {
        int requirementForEveryLevel = requiredSkillLevel[skill];
        int currentYakuLevel = player.GetSkillSet().GetLevel(this) * requirementForEveryLevel -
                               player.GetSkillSet().GetExtraLevel(type);
        return (currentYakuLevel + 1) * requirementForEveryLevel;
    }

    public bool ShouldHighlightTile(Tile tile, Player player)
    {
        Permutation permutation = player.GetCurrentSelectedPerm() ?? player.GetAccumulatedPermutation();
        if(permutation == null) return false;
        YakuTester.TestYaku(permutation, type, player, out var tiles);
        return tiles.Contains(tile);
    }
}

public class YakuSkillRelationContainer
{
    public string yakuTypeID;
    public float Feng;
    public float Lin;
    public float Huo;
    public float Shan;
    public float YaoJiu;
    public float JiFeng;
    public float SanYuan;
    public float BuGao;
    public float TongShun;
    public float LianShun;
    public float JuShu;
    public float RanShou;
    public float QiMen;
    public float DuoGang;
    public float TongKe;
    public float JieGao;

    [JsonConstructor]
    public YakuSkillRelationContainer()
    {
    }

    public Dictionary<SkillType, int> ToMap()
    {
        Dictionary<SkillType, int> map = new Dictionary<SkillType, int>
        {
            { SkillType.Feng, (int)Feng },
            { SkillType.Lin, (int)Lin },
            { SkillType.Huo, (int)Huo },
            { SkillType.Shan, (int)Shan },
            { SkillType.YaoJiu, (int)YaoJiu },
            { SkillType.JiFeng, (int)JiFeng },
            { SkillType.SanYuan, (int)SanYuan },
            { SkillType.BuGao, (int)BuGao },
            { SkillType.TongShun, (int)TongShun },
            { SkillType.LianShun, (int)LianShun },
            { SkillType.JuShu, (int)JuShu },
            { SkillType.RanShou, (int)RanShou },
            { SkillType.QiMen, (int)QiMen },
            { SkillType.DuoGang, (int)DuoGang },
            { SkillType.TongKe, (int)TongKe },
            { SkillType.JieGao, (int)JieGao }
        };
        return map;
    }
}

public class YakuContainer
{
    public string yakuTypeID;

    public string rarity;

    public string chineseName;

    public int fullFan;
    public double growthFactor;
    public double levelingFactor;
    public string[] includedYakus;
    public string[] groups;

    public string example;

    public string englishRomajiName;
    public string chineseRomajiName;

    public int order;

    [JsonConstructor]
    public YakuContainer()
    {
    }

    public YakuContainer(string yakuTypeID, string rarity, string chineseName, int fullFan,
        double growthFactor, double levelingFactor, string[] includedYakus, string[] groups,
        string example, string engRoma, string chinRoma, int order)
    {
        this.yakuTypeID = yakuTypeID;
        this.rarity = rarity;
        this.chineseName = chineseName;
        this.fullFan = fullFan;
        this.growthFactor = growthFactor;
        this.levelingFactor = levelingFactor;
        this.includedYakus = includedYakus;
        this.groups = groups;
        this.example = example;
        englishRomajiName = engRoma;
        chineseRomajiName = chinRoma;
        this.order = order;
    }

    public Yaku GetYaku()
    {
        try
        {
            Yaku yaku = ScriptableObject.CreateInstance<Yaku>();
            var a = (YakuType)Enum.Parse(typeof(YakuType), yakuTypeID);
            
            yaku.type = a;
            yaku.fullFan = fullFan;
            yaku.growthFactor = growthFactor;
            yaku.levelingFactor = levelingFactor;
            yaku.includedYakus = includedYakus.Select(a => (YakuType)Enum.Parse(typeof(YakuType), a)).ToArray();
            yaku.groups = groups;
            yaku.rarity = Enum.Parse<Rarity>(rarity);
            yaku.example = example;
            yaku.order = order;
            
            return yaku;
        }
        catch
        {
            Debug.LogWarning($"Failed loading Yaku: [{yakuTypeID}]");
            return null;
        }
    }
}