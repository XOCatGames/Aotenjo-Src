using System;
using UnityEngine;

namespace Aotenjo
{
    public abstract class LevelingArtifact : Artifact
    {
        private int _level;

        public int Level
        {
            set => SetLevel(value);
            get => _level;
        }

        private void SetLevel(int value)
        {
            _level = value;
        }

        public int GetLevel(Player player)
        {
            return Level;
        }

        public int initLevel;

        public LevelingArtifact(string name, Rarity rarity, int initLevel) : base(name, rarity)
        {
            _level = initLevel;
            this.initLevel = initLevel;
        }


        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            Level = initLevel;
        }

        public override string Serialize()
        {
            return Level.ToString();
        }

        public override void Deserialize(string data)
        {
            base.Deserialize(data);
            Level = Int32.Parse(data);
        }

        protected Effect GetLevelUpEffect()
        {
            return new SimpleEffect($"effect_{GetNameID()}_leveling", this, _ => Level++);
        }
    }
}