using System;
using UnityEngine;

namespace Aotenjo
{
    /// <summary>
    /// One playable level. Serializable subclasses and their serialized fields survive saves.
    /// Keep event subscriptions and other runtime-only references nonserialized.
    /// </summary>
    [Serializable]
    public abstract class GameLevel
    {
        public const int StandardRunCompletionLevel = 16;

        protected GameLevel(int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number), "Level number must be positive.");

            this.number = number;
        }

        [SerializeField] private int number;

        public int Number => number;

        public virtual bool IsBossLevel => false;

        public virtual bool IsChapterStart => Number % 4 == 1;

        public virtual bool IsPostBossChapterStart => IsChapterStart && Number > 1;

        public virtual bool IsRunCompletionLevel => Number == StandardRunCompletionLevel;

        public virtual int BaseBonusMoney => 4;

        public virtual LevelExtraInfo ExtraInfo => null;

        public bool HasExtraInfo => ExtraInfo != null;

        public virtual void Enter(Player player)
        {
        }

        public virtual void Exit(Player player)
        {
        }
    }

    [Serializable]
    public sealed class NormalLevel : GameLevel
    {
        public NormalLevel(int number) : base(number)
        {
        }
    }

    [Serializable]
    public class BossLevel : GameLevel
    {
        [SerializeField] private string bossName;
        [SerializeField] private bool harderBoss;
        [NonSerialized] private Boss boss;
        [NonSerialized] private LevelExtraInfo extraInfo;

        public BossLevel(int number, Boss boss) : base(number)
        {
            this.boss = boss ?? throw new ArgumentNullException(nameof(boss));
            bossName = boss.name;
            harderBoss = Attribute.IsDefined(boss.GetType(), typeof(HarderBossAttribute), true);
        }

        public Boss Boss
        {
            get
            {
                if (boss == null)
                {
                    boss = Bosses.CreateEncounterOrElseRedraw(bossName, harderBoss);
                    bossName = boss.name;
                }

                return boss;
            }
        }

        public override bool IsBossLevel => true;

        public override int BaseBonusMoney => 12;

        public override LevelExtraInfo ExtraInfo => extraInfo ??= new BossLevelExtraInfo(Boss);

        public override void Enter(Player player)
        {
            Boss.SubscribeToPlayerEvents(player);
        }

        public override void Exit(Player player)
        {
            Boss.UnsubscribeFromPlayerEvents(player);
        }
    }

    public abstract class LevelExtraInfo
    {
        public abstract string GetHeader(Player player, Func<string, string> localize);

        public abstract string GetDescription(Player player, Func<string, string> localize);
    }

    internal sealed class BossLevelExtraInfo : LevelExtraInfo
    {
        private readonly Boss boss;

        public BossLevelExtraInfo(Boss boss)
        {
            this.boss = boss;
        }

        public override string GetHeader(Player player, Func<string, string> localize)
        {
            string bossName = boss.GetName(player, localize);
            return string.Format(localize("ui_encounter_boss_format"), bossName);
        }

        public override string GetDescription(Player player, Func<string, string> localize)
        {
            return boss.GetDescription(player, localize);
        }
    }

    public static class GameLevelFactory
    {
        public static GameLevel Create(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            string bossName = player.currentBossName;
            if (string.IsNullOrEmpty(bossName) && IsScheduledBossLevel(player.Level))
                bossName = player.GetOrCreateBossNameForLevel(player.Level);

            if (string.IsNullOrEmpty(bossName))
                return new NormalLevel(player.Level);

            Boss boss = Bosses.CreateEncounterOrElseRedraw(bossName, player.HarderBossesEnabled());
            return boss == null
                ? new NormalLevel(player.Level)
                : new BossLevel(player.Level, boss);
        }

        public static bool IsScheduledBossLevel(int levelNumber)
        {
            return levelNumber > 0 && levelNumber % 4 == 0;
        }
    }
}
