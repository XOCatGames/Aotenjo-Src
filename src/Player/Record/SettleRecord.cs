using System;
using System.Collections.Generic;
using Aotenjo;
using UnityEngine;

[Serializable]
public class SettleRecord
{
    [SerializeField] public int level;

    [SerializeField] public int stage;

    [SerializeField] public PermutationType type;

    [SerializeReference] public List<Tile> AllTiles;

    [SerializeReference] public List<Tile> SelectedTiles;

    [SerializeField] public List<YakuType> activatedYakuTypes;

    [SerializeField] public Score score;

    [SerializeField] public SerializableMap<YakuType, double> yakuFanMap;

    public SettleRecord()
    {
        level = -1;
        stage = -1;
        type = PermutationType.NORMAL;
        AllTiles = new List<Tile>();
        SelectedTiles = new List<Tile>();
        activatedYakuTypes = new List<YakuType>();
        score = new Score(0, 0);
        yakuFanMap = new SerializableMap<YakuType, double>();
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
        this.activatedYakuTypes = activatedYakuTypes;
        this.score = score;
        yakuFanMap = new SerializableMap<YakuType, double>();
    }
}