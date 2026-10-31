using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Aotenjo
{
    [Serializable]
    public class RoundRecord
    {
        [SerializeReference, FormerlySerializedAs("history")] private RunHistorySnapshot savedHistory;
        [NonSerialized] private Func<RunHistorySnapshot> loadSnapshot;
        [NonSerialized] private Func<RoundRecord> loadDetails;
        [NonSerialized] private int archivedMaxLevel;
        [NonSerialized] private string originalArchiveJson;
        [NonSerialized] internal Func<string> loadArchiveJson;
        public string preservedArchiveJson
        {
            get => originalArchiveJson ??= loadArchiveJson?.Invoke();
            set { originalArchiveJson = value; loadArchiveJson = null; }
        }

        public RunHistorySnapshot history
        {
            get
            {
                if (loadSnapshot != null)
                {
                    savedHistory = loadSnapshot();
                    loadSnapshot = null;
                }
                return savedHistory;
            }
            set { savedHistory = value; loadSnapshot = null; }
        }

        public int MaxLevel => loadDetails != null ? archivedMaxLevel : savedRoundStats?.maxLevel ?? 0;

        internal void DeferArchive(int maxLevel, Func<RunHistorySnapshot> snapshot, Func<RoundRecord> details)
        {
            archivedMaxLevel = maxLevel;
            loadSnapshot = snapshot;
            loadDetails = details;
        }

        private void EnsureDetails()
        {
            if (loadDetails == null) return;
            var restored = loadDetails();
            savedRoundStats = restored.savedRoundStats;
            heldArtifacts = restored.heldArtifacts;
            newHeldArtifacts = restored.newHeldArtifacts;
            if (loadSnapshot != null) history = restored.history;
            loadDetails = null;
        }

        public RoundRecord() { roundStats = PlayerStats.New(); }

        // A list entry only needs its snapshot; the complete run graph is loaded
        // when a caller asks for roundStats or held artifacts.
        public RunHistorySnapshot GetHistorySnapshot() => history;
        /// <summary>
        /// 该局统计信息
        /// </summary>
        [SerializeField, FormerlySerializedAs("roundStats")] private PlayerStats savedRoundStats;
        public PlayerStats roundStats
        {
            get { EnsureDetails(); return savedRoundStats; }
            set { savedRoundStats = value; loadDetails = null; }
        }

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
            EnsureDetails();
            if (newHeldArtifacts != null && newHeldArtifacts.Any()) return newHeldArtifacts.Select(id => ArtifactRegistry.TryGetDefinition(id, out _) ? ArtifactRegistry.CreatePreview(id) : null).ToList();
            return (heldArtifacts ?? new List<int>()).Select(number => ArtifactRegistry.FindLegacyDefinition(number))
                .Select(entry => entry == null ? null : ArtifactRegistry.CreatePreview(entry.Id)).ToList();
        }
    }
}
