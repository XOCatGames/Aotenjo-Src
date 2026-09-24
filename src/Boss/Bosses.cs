using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Bosses
{
    private sealed class Registration
    {
        public Func<Boss> Factory;
        public Boss Preview;
        public bool Available;
        public bool FinalBoss;
        public bool HardBoss;
        public Boss HarderPreview;

        public Boss GetPreview(bool harderBoss)
        {
            return harderBoss ? HarderPreview ??= Factory().GetHarderBoss() : Preview;
        }

        public Boss CreateEncounter(bool harderBoss)
        {
            Boss boss = Factory();
            return harderBoss ? boss.GetHarderBoss() : boss;
        }
    }

    private static readonly List<Registration> registrations = new();
    private static readonly Dictionary<string, Registration> registrationsByName = new(StringComparer.Ordinal);

    // Registration order is the seeded selection order. Keep existing entries in place.
    // These fields are compatibility previews; never subscribe them as active encounters.
    public static readonly Boss Knowledgeless = Register(() => new KnowledgelessBoss());
    public static readonly Boss Colorless = Register(() => new ColorlessBoss());
    public static readonly Boss Heartless = Register(() => new HeartlessBoss());
    public static readonly Boss Greedless = Register(() => new GreedlessBoss());
    public static readonly Boss Desireless = Register(() => new DesirelessBoss());
    public static readonly Boss Unobstinate = Register(() => new UnobstinateBoss());
    public static readonly Boss Undisturbed = Register(() => new UndisturbedBoss());
    public static readonly Boss Unwashed = Register(() => new UnwashedBoss());
    public static readonly Boss Unyielding = Register(() => new UnyieldingBoss());
    public static readonly Boss Straightless = Register(() => new StraightlessBoss());
    public static readonly Boss Uneven = Register(() => new UnevenBoss());
    public static readonly Boss Unstable = Register(() => new UnstableBoss());
    public static readonly Boss Undetermined = Register(() => new UndeterminedBoss());
    public static readonly Boss Unaided = Register(() => new UnaidedBoss(), finalBoss: true);
    public static readonly Boss Directionless = Register(() => new DirectionlessBoss());
    public static readonly Boss Undefeatable = Register(() => new UndefeatableBoss(), finalBoss: true);
    public static readonly Boss Timeless = Register(() => new TimelessBoss(), finalBoss: true);
    public static readonly Boss Powerless = Register(() => new PowerlessBoss(), finalBoss: true);
    public static readonly Boss Flawless = Register(() => new FlawlessBoss(), finalBoss: true, hardBoss: true);
    public static readonly Boss Unfuriten = Register(() => new UnfuritenBoss());
    public static readonly Boss Uncompleted = Register(() => new UncompletedBoss());
    public static readonly Boss Numberless = Register(() => new NumberlessBoss(), available: false);

    public static readonly Boss[] BossList = registrations.Where(r => r.Available).Select(r => r.Preview).ToArray();
    public static readonly Boss[] FinalBossList = registrations.Where(r => r.Available && r.FinalBoss).Select(r => r.Preview).ToArray();
    public static readonly Boss[] HardBossList = registrations.Where(r => r.Available && r.HardBoss).Select(r => r.Preview).ToArray();

    private static Boss Register(Func<Boss> factory, bool finalBoss = false, bool hardBoss = false, bool available = true)
    {
        Boss preview = factory();
        Registration registration = new()
        {
            Factory = factory,
            Preview = preview,
            Available = available,
            FinalBoss = finalBoss,
            HardBoss = hardBoss
        };
        registrationsByName.Add(preview.name, registration);
        registrations.Add(registration);
        return preview;
    }

    private static Registration FindOrRedraw(string bossName)
    {
        if (bossName != null && registrationsByName.TryGetValue(bossName, out Registration registration))
            return registration;

        return BossList.Length == 0 ? null : registrationsByName[BossList[Random.Range(0, BossList.Length)].name];
    }

    /// <summary>
    /// Returns a shared preview for names, descriptions and selection. Do not mutate or subscribe it.
    /// Null or unknown names redraw from the available pool, preserving the requested difficulty.
    /// </summary>
    public static Boss GetPreviewOrElseRedraw(string bossName, bool harderBoss)
    {
        return FindOrRedraw(bossName)?.GetPreview(harderBoss);
    }

    /// <summary>
    /// Creates fresh mutable state owned by one encounter. Never returns a registry preview.
    /// Null or unknown names redraw from the available pool, preserving the requested difficulty.
    /// </summary>
    public static Boss CreateEncounterOrElseRedraw(string bossName, bool harderBoss)
    {
        return FindOrRedraw(bossName)?.CreateEncounter(harderBoss);
    }

    [Obsolete("Use GetPreviewOrElseRedraw for display/selection, or CreateEncounterOrElseRedraw for gameplay.")]
    public static Boss GetBossOrElseRedraw(string bossName, bool harderBoss)
    {
        // Preserve legacy identity, difficulty and unavailable-name fallback behavior for external callers.
        if (bossName != null && registrationsByName.TryGetValue(bossName, out Registration registration) && registration.Available)
            return harderBoss ? registration.Preview.GetHarderBoss() : registration.Preview;

        return BossList.Length > 0 ? BossList[Random.Range(0, BossList.Length)] : null;
    }

    [Obsolete("Use CreateEncounterOrElseRedraw to make encounter ownership explicit.")]
    public static Boss CreateBossOrElseRedraw(string bossName, bool harderBoss)
    {
        return CreateEncounterOrElseRedraw(bossName, harderBoss);
    }
}
