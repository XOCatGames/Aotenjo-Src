using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SettleRecord
{
    [SerializeField] public int level;

    [SerializeField] public int stage;

    [SerializeField] public PermutationType type;

    [SerializeReference] public List<Tile> AllTiles;

    [SerializeReference] public List<Tile> SelectedTiles;

    [SerializeField] public List<FixedYakuType> activatedYakuTypes;
    
    [SerializeField] public List<YakuType> activatedYakus;

    public List<YakuType> ActivatedYakuTypes
    {
        get
        {
            if (activatedYakuTypes != null && activatedYakuTypes.Any() && !activatedYakus.Any())
            {
                activatedYakus = activatedYakuTypes.Select(t => new YakuType(t)).ToList();
                activatedYakuTypes.Clear();
            }
            return activatedYakus;
        }
    }

    [SerializeField] public Score score;

    [SerializeField] public SerializableMap<FixedYakuType, double> yakuFanMap;

    [SerializeField] public SerializableMap<YakuType, double> yakuFan;
    
    public SerializableMap<YakuType, double> YakuFanMap
    {
        get
        {
            if (yakuFanMap != null && !yakuFanMap.IsEmpty() && yakuFan.IsEmpty())
            {
                yakuFan = new SerializableMap<YakuType, double>();
                foreach (var kvp in yakuFanMap.GetKeys())
                {
                    yakuFan[kvp] = yakuFanMap[kvp];
                }
                yakuFanMap.Clear();
            }
            return yakuFan;
        }
        
        set { yakuFan = value; }
    }

    public SettleRecord()
    {
        level = -1;
        stage = -1;
        type = PermutationType.NORMAL;
        AllTiles = new List<Tile>();
        SelectedTiles = new List<Tile>();
        activatedYakus = new List<YakuType>();
        score = new Score(0, 0);
        YakuFanMap = new SerializableMap<YakuType, double>();
    }

    public SettleRecord(int level, int stage,
        PermutationType type, List<Tile> allTiles,
        List<Tile> selectedTiles, List<YakuType> activatedYakuTypes,
        Score score)
    {
        this.level = level;
        this.stage = stage;
        this.type = type;
        AllTiles = allTiles;
        SelectedTiles = selectedTiles;
        this.activatedYakus = activatedYakuTypes;
        this.score = score;
        YakuFanMap = new SerializableMap<YakuType, double>();
    }
}