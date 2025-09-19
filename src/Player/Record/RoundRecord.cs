using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class RoundRecord
    {
        [SerializeField] public PlayerStats roundStats;

        [SerializeField] public string deckName;

        [SerializeField] public int acsensionLevel;

        [SerializeField] public bool won;

        [SerializeField] public string materialSet;
        
        [SerializeField] public string materialSetNameExplicit;

        [SerializeField] public string endedTime;

        [SerializeReference] private List<int> heldArtifacts;
        
        [SerializeReference] private List<string> newHeldArtifacts;

        [SerializeField] public string seed;

        [SerializeField] public bool seeded;

        public RoundRecord(PlayerStats roundStats, string deckName, int acsensionLevel, bool won, string materialSet,
            List<Artifact> artifacts, string seed, bool seededRun, string materialSetNameExplicit)
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
        }

        public List<Artifact> GetHeldArtifacts()
        {
            if (newHeldArtifacts.Any()) return newHeldArtifacts.Select(a => Artifacts.GetArtifact(a)).ToList();
            return heldArtifacts.Select(a => Artifacts.ArtifactList.FirstOrDefault(ar => ar.GetNumberID() == a)).ToList();
        }
    }
}