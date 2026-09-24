using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class RoundRecord
    {
        /// <summary>
        /// 该局统计信息
        /// </summary>
        [SerializeField] public PlayerStats roundStats;

        /// <summary>
        /// 套牌
        /// </summary>
        [SerializeField] public string deckName;

        /// <summary>
        /// 进阶等级
        /// </summary>
        [SerializeField] public int acsensionLevel;

        /// <summary>
        /// 是否获胜
        /// </summary>
        [SerializeField] public bool won;

        /// <summary>
        /// 牌体集
        /// </summary>
        [SerializeField] public string materialSet;
        
        /// <summary>
        /// 牌体集名（自定义牌体集时使用）
        /// </summary>
        [SerializeField] public string materialSetNameExplicit;

        /// <summary>
        /// 结束时间
        /// </summary>
        [SerializeField] public string endedTime;

        [Obsolete("Use newHeldArtifacts instead to adapt to artifact ID changes")]
        [SerializeReference] private List<int> heldArtifacts;
        
        /// <summary>
        /// 手持遗物（新版本，存储遗物RegName以适应遗物ID变动）
        /// </summary>
        [SerializeReference] private List<string> newHeldArtifacts;

        /// <summary>
        /// 种子
        /// </summary>
        [SerializeField] public string seed;

        /// <summary>
        /// 是否为Seeded Run
        /// </summary>
        [SerializeField] public bool seeded;

        /// <summary>
        /// 本局是否使用过控制台指令或加载了Mod
        /// </summary>
        [SerializeField] public bool isModified;

        public RoundRecord(PlayerStats roundStats, string deckName, int acsensionLevel, bool won, string materialSet,
            List<Artifact> artifacts, string seed, bool seededRun, string materialSetNameExplicit, bool isModified)
        {
            this.roundStats = roundStats;
            this.deckName = deckName;
            this.acsensionLevel = acsensionLevel;
            this.won = won;
            this.materialSet = materialSet;
            this.materialSetNameExplicit = materialSetNameExplicit;
            endedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.newHeldArtifacts = new(artifacts.Select(a => a.GetRegName()));
            this.heldArtifacts = new();
            this.seed = seed;
            seeded = seededRun;
            this.isModified = isModified;
        }

        public List<Artifact> GetHeldArtifacts()
        {
            if (newHeldArtifacts.Any()) return newHeldArtifacts.Select(a => Artifacts.GetArtifact(a)).ToList();
            return heldArtifacts.Select(a => Artifacts.ArtifactList.FirstOrDefault(ar => ar.GetNumberID() == a)).ToList();
        }
    }
}