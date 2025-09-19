using Aotenjo;
using UnityEngine;

public class Bosses
{
    public static readonly Boss Knowledgeless = new KnowledgelessBoss();
    public static readonly Boss Colorless = new ColorlessBoss();
    public static readonly Boss Heartless = new HeartlessBoss();
    public static readonly Boss Greedless = new GreedlessBoss();
    public static readonly Boss Desireless = new DesirelessBoss();
    public static readonly Boss Undisturbed = new UndisturbedBoss();
    public static readonly Boss Unobstinate = new UnobstinateBoss();
    public static readonly Boss Unwashed = new UnwashedBoss();
    public static readonly Boss Unyielding = new UnyieldingBoss();
    public static readonly Boss Straightless = new StraightlessBoss();
    public static readonly Boss Uneven = new UnevenBoss();
    public static readonly Boss Unstable = new UnstableBoss();
    public static readonly Boss Undetermined = new UndeterminedBoss();
    public static readonly Boss Directionless = new DirectionlessBoss();

    public static readonly Boss Numberless = new NumberlessBoss();
    public static readonly Boss Unfuriten = new UnfuritenBoss();
    public static readonly Boss Uncompleted = new UncompletedBoss();

    //北风北专属Boss
    //不借
    public static readonly Boss Unaided = new UnaidedBoss();

    //无敌
    public static readonly Boss Undefeatable = new UndefeatableBoss();

    //不朽
    public static readonly Boss Timeless = new TimelessBoss();

    //不接
    public static readonly Boss Powerless = new PowerlessBoss();

    //无瑕
    public static readonly Boss Flawless = new FlawlessBoss();

    public static readonly Boss[] BossList =
    {
        Knowledgeless, Colorless, Heartless, Greedless, Desireless, Unobstinate, Undisturbed,
        Unwashed, Unyielding, Straightless, Uneven, Unstable, Undetermined, Unaided, Directionless, Undefeatable,
        Timeless, Powerless, Flawless, Unfuriten, Uncompleted
    };

    public static readonly Boss[] FinalBossList =
    {
        Unaided, Undefeatable, Timeless, Powerless, Flawless
    };

    public static readonly Boss[] HardBossList =
    {
        Flawless
    };

    public static Boss GetBossOrElseRedraw(string bossName, bool harderBoss)
    {
        foreach (var boss in BossList)
        {
            if (boss.name == bossName)
            {
                return harderBoss? boss.GetHarderBoss() : boss;
            }
        }

        return BossList.Length > 0 ? BossList[Random.Range(0, BossList.Length)] : null;
    }
}