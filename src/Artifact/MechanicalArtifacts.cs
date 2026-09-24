using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aotenjo
{
    public static class MechanicalArtifactUtility
    {
        public static readonly MechPartType[] CommonParts =
        {
            MechPartType.Gear, MechPartType.DriveRod, MechPartType.NetworkCard,
            MechPartType.Led, MechPartType.Shield
        };

        public static readonly MechPartType[] EpicParts =
        {
            MechPartType.IntegratedChip, MechPartType.Reactor
        };

        public static bool TryGetMaterial(Tile tile, out TileMaterialMechPart material)
        {
            material = tile?.properties?.material as TileMaterialMechPart;
            return material != null;
        }

        public static bool TryInstall(Player player, Tile tile, MechPartType part, bool temporary = false)
        {
            if (player == null || tile == null) return false;
            if (TryGetMaterial(tile, out TileMaterialMechPart material))
            {
                if (!material.TryInstall(part, temporary)) return false;
            }
            else
            {
                TileMaterialMechPart replacement = TileMaterialMechPart.Create(part, temporary);
                if (temporary)
                    replacement.RememberMaterialBeforeTemporaryConversion(tile.properties.material);
                tile.SetMaterial(replacement, player);
            }

            EventBus.Publish(new MechanicalPartChangedEvent(player, tile,
                MechanicalPartChangeKind.Installed, part, part, 1));
            return true;
        }

        public static void InstallParts(Player player, Tile tile, MechPartType part, int count, bool temporary = false)
        {
            for (int i = 0; i < count; i++)
                if (!TryInstall(player, tile, part, temporary)) break;
        }

        public static MechPartType RandomCommon(Player player) =>
            CommonParts[player.GenerateRandomInt(CommonParts.Length, "mech_common_part")];

        public static MechPartType RandomEpic(Player player) =>
            EpicParts[player.GenerateRandomInt(EpicParts.Length, "mech_epic_part")];

        public static string GetFormattedPartName(MechPartType part, Func<string, string> localizer)
        {
            string nameKey = TileMaterialMechPart.GetNameKey(part);
            string name = localizer($"tile_{nameKey}_material_name");
            return $"<style=\"yellow\"><link=\"tilemat_{nameKey}\">{name}</link></style>";
        }

        public static Effect ActionEffect(Artifact source, string textKey, Action<Player> action, string sound = "AddFu") =>
            new SimpleEffect(textKey, source, action, sound);

        public static IEnumerable<Tile> InstallableTiles(IEnumerable<Tile> tiles) =>
            tiles.Where(tile => tile != null &&
                (tile.properties.material is not TileMaterialMechPart material || material.HasEmptySlot));
    }

    public abstract class MechanicalArtifact : Artifact
    {
        protected MechanicalArtifact(string name, Rarity rarity) : base(name, rarity)
        {
            setIn.Add("mech_parts");
            SetHighlightRequirement((tile, _) => tile?.properties?.material is TileMaterialMechPart);
        }

        protected static int CommonTriggers(TileMaterialMechPart material) => material.GetCommonTriggerCount();
    }

    public sealed class TwistingDeviceArtifact : MechanicalArtifact
    {
        private const int INSTALL_CHANCE_DENOMINATOR = 5;
        private const int INSTALL_COUNT = 1;

        public TwistingDeviceArtifact() : base("twisting_device", Rarity.RARE) { }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile)) return;
            if (MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material) &&
                material.GetPartCount(MechPartType.Gear) > 0)
                effects.Add(new CorruptEffect(tile));

            if (tile.GetCategory() is Tile.Category.Wan or Tile.Category.Bing &&
                player.GenerateRandomDeterminationResult(INSTALL_CHANCE_DENOMINATOR))
                effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_twisting_device_install",
                    p => MechanicalArtifactUtility.InstallParts(p, tile, MechPartType.Gear, INSTALL_COUNT)));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), INSTALL_CHANCE_DENOMINATOR, INSTALL_COUNT);
    }

    public sealed class GearboxArtifact : MechanicalArtifact
    {
        private const int ADD_FAN = 5;
        private const int INSTALL_COUNT = 1;

        public GearboxArtifact() : base("gearbox", Rarity.RARE) { }
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects) =>
            effects.Add(ScoreEffect.AddFan(ADD_FAN, this));

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile) || tile.GetCategory() != Tile.Category.Suo) return;
            if (MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material) &&
                material.GetPartCount(MechPartType.DriveRod) > 0) return;
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_gearbox_install",
                p => MechanicalArtifactUtility.InstallParts(p, tile, MechPartType.DriveRod, INSTALL_COUNT)));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FAN, INSTALL_COUNT);
    }

    public sealed class TinfoilHatArtifact : MechanicalArtifact
    {
        private const int GIFT_COUNT = 5;

        public TinfoilHatArtifact() : base("tinfoil_hat", Rarity.RARE) =>
            SetMaterialGift(TileMaterial.MechShield, GIFT_COUNT);

        [SubscribeToEvent]
        private void CleanseAppliedDebuff(PlayerEvents.PostSetTilePropertiesEvent evt)
        {
            if (!evt.appliedDebuff ||
                !MechanicalArtifactUtility.TryGetMaterial(evt.tile, out TileMaterialMechPart material) ||
                material.GetPartCount(MechPartType.Shield) == 0) return;
            new CleanseEffect(this, evt.tile).Ingest(evt.player);
        }
    }

    public sealed class ConveyorBeltArtifact : MechanicalArtifact
    {
        private const double FAN_MULTIPLIER = 1.2;
        private const int GIFT_COUNT = 5;

        public ConveyorBeltArtifact() : base("conveyor_belt", Rarity.EPIC) =>
            SetMaterialGift(TileMaterial.MechGear, GIFT_COUNT);

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material)) return;
            int triggers = material.GetPartCount(MechPartType.Gear) * CommonTriggers(material);
            for (int i = 0; i < triggers; i++) effects.Add(ScoreEffect.MulFan(FAN_MULTIPLIER, this));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), FAN_MULTIPLIER);
    }

    public sealed class ClassifiedBlueprintArtifact : MechanicalArtifact
    {
        private const double BASE_MULTIPLIER = 1.0;
        private const double MULTIPLIER_PER_YAKU = 0.1;

        public ClassifiedBlueprintArtifact() : base("classified_blueprint", Rarity.EPIC) { }

        public static double GetMultiplier(Player player, Permutation permutation)
        {
            if (player == null || permutation == null || YakuTester.InfoMap == null) return BASE_MULTIPLIER;
            int commonYakus = permutation.GetYakus(player).Distinct()
                .Count(type => YakuTester.InfoMap.TryGetValue(type, out Yaku yaku) && yaku.rarity == Rarity.COMMON);
            return BASE_MULTIPLIER + commonYakus * MULTIPLIER_PER_YAKU;
        }

        public override string GetDescription(Func<string, string> localizer) => GetDescription(null, localizer);

        public override string GetDescription(Player player, Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer),
                GetMultiplier(player, player?.GetCurrentSelectedPerm() ?? player?.GetAccumulatedPermutation())
                    .ToString("0.0", CultureInfo.InvariantCulture), MULTIPLIER_PER_YAKU);

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (permutation == null || !MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material)) return;
            int shieldTriggers = material.GetPartCount(MechPartType.Shield) * CommonTriggers(material);
            if (shieldTriggers == 0) return;
            double multiplier = GetMultiplier(player, permutation);
            for (int i = 0; i < shieldTriggers; i++) effects.Add(ScoreEffect.MulFan(multiplier, this));
        }
    }

    public sealed class PhotovoltaicPanelArtifact : MechanicalArtifact
    {
        private const int GROW_CHANCE_DENOMINATOR = 3;
        private const int GIFT_COUNT = 5;

        public PhotovoltaicPanelArtifact() : base("photovoltaic_panel", Rarity.COMMON) =>
            SetMaterialGift(TileMaterial.MechLed, GIFT_COUNT);

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile) || !tile.IsNumbered() ||
                tile.properties.mask.GetRegName() == TileMask.Grow().GetRegName() ||
                !MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material)) return;
            int attempts = material.GetPartCount(MechPartType.Led) * CommonTriggers(material) *
                           TileMaterialMechPart.CountLedColors(tile, player);
            if (Enumerable.Range(0, attempts).Any(_ => player.GenerateRandomDeterminationResult(GROW_CHANCE_DENOMINATOR)))
                effects.Add(new GrowEffect(tile, this));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), GROW_CHANCE_DENOMINATOR);
    }

    public sealed class NetworkSwitchArtifact : MechanicalArtifact
    {
        private const int BASE_FU = 20;
        private const int EXTRA_FU = 10;

        public NetworkSwitchArtifact() : base("network_switch", Rarity.EPIC) { }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (permutation == null || !MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material)) return;
            int localCards = material.GetPartCount(MechPartType.NetworkCard);
            if (localCards == 0) return;
            int allCards = player.GetScoringTiles(permutation).Select(t => t.properties.material as TileMaterialMechPart)
                .Where(m => m != null).Sum(m => m.GetPartCount(MechPartType.NetworkCard));
            int value = BASE_FU + Math.Max(0, allCards - 1) * EXTRA_FU;
            for (int i = 0; i < localCards * CommonTriggers(material); i++) effects.Add(ScoreEffect.AddFu(value, this));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), BASE_FU, EXTRA_FU);
    }

    public sealed class MolecularReconstructorArtifact : MechanicalArtifact
    {
        private MechPartType from = MechPartType.Gear;
        private MechPartType to = MechPartType.DriveRod;
        public MolecularReconstructorArtifact() : base("molecular_reconstructor", Rarity.RARE) { }

        public override void PreGameInitialized(Player player)
        {
            from = MechanicalArtifactUtility.RandomCommon(player);
            do to = MechanicalArtifactUtility.RandomCommon(player); while (to == from);
        }

        public override string GetDescription(Func<string, string> localizer) => GetDescription(null, localizer);

        public override string GetDescription(Player player, Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer),
                MechanicalArtifactUtility.GetFormattedPartName(from, localizer),
                MechanicalArtifactUtility.GetFormattedPartName(to, localizer));

        public override string Serialize() => $"{(int)from},{(int)to}";
        public override void Deserialize(string data)
        {
            string[] values = data?.Split(',');
            if (values == null || values.Length != 2 ||
                !int.TryParse(values[0], out int f) || !int.TryParse(values[1], out int t) ||
                f == t || !MechanicalArtifactUtility.CommonParts.Contains((MechPartType)f) ||
                !MechanicalArtifactUtility.CommonParts.Contains((MechPartType)t))
                throw new FormatException("Invalid molecular reconstructor state.");

            from = (MechPartType)f;
            to = (MechPartType)t;
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile) || !MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material) ||
                material.GetPartCount(from) == 0) return;
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_molecular_reconstructor_transform",
                p => material.TransformAll(p, tile, from, to)));
        }
    }

    public sealed class FiberOpticArtifact : MechanicalArtifact
    {
        private const int TRANSFER_COUNT = 1;
        private const int GIFT_COUNT = 5;

        public FiberOpticArtifact() : base("fiber_optic", Rarity.RARE) =>
            SetMaterialGift(TileMaterial.MechNetworkCard, GIFT_COUNT);

        public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> effects,
            bool withForce, bool isClone)
        {
            if (!MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart source)) return;
            int triggers = source.GetPartCount(MechPartType.NetworkCard) * CommonTriggers(source) * TRANSFER_COUNT;
            for (int i = 0; i < triggers; i++)
            {
                effects.Add(SimpleAppendEffect.Create(player.discardTileEffectsStack, () =>
                {
                    List<IAnimationEffect> transferEffects = new();
                    if (!MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart current) ||
                        current.GetParts().All(part => part == MechPartType.NetworkCard)) return transferEffects;
                    List<Tile> targets = player.GetHandDeckCopy().Where(candidate => candidate != tile &&
                        candidate.properties.material is TileMaterialMechPart target && target.HasEmptySlot).ToList();
                    if (targets.Count == 0) return transferEffects;
                    Tile target = targets[player.GenerateRandomInt(targets.Count, "fiber_optic_target")];
                    transferEffects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_fiber_optic_transfer",
                        p => current.TransferRandomNonNetworkPartTo(p, tile, target))
                        .OnMultipleTiles(new List<Tile> { tile, target }, tile));
                    return transferEffects;
                }));
            }
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), TRANSFER_COUNT);
    }

    public sealed class BlackbodyArtifact : MechanicalArtifact
    {
        private const int ADD_FU = 50;
        private const int ADD_MONEY = 2;
        private const int REQUIRED_COLOR_COUNT = 1;

        public BlackbodyArtifact() : base("blackbody", Rarity.COMMON) { }
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material) ||
                TileMaterialMechPart.CountLedColors(tile, player) != REQUIRED_COLOR_COUNT) return;
            int triggers = material.GetPartCount(MechPartType.Led) * CommonTriggers(material);
            for (int i = 0; i < triggers; i++)
            {
                effects.Add(ScoreEffect.AddFu(ADD_FU, this));
                effects.Add(new EarnMoneyEffect(ADD_MONEY, this));
            }
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FU, REQUIRED_COLOR_COUNT, ADD_MONEY);
    }

    public sealed class TriangularLampPostArtifact : MechanicalArtifact
    {
        private const int ADD_FAN = 5;
        private const int INSTALL_CHANCE_DENOMINATOR = 2;
        private const int INSTALL_COUNT = 1;

        public TriangularLampPostArtifact() : base("triangular_lamp_post", Rarity.COMMON) { }
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects) =>
            effects.Add(ScoreEffect.AddFan(ADD_FAN, this));
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (player.IsPlayingTile(tile) && tile.IsYaoJiu(player) && player.GenerateRandomDeterminationResult(INSTALL_CHANCE_DENOMINATOR))
                effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_triangular_lamp_post_install",
                    p => MechanicalArtifactUtility.InstallParts(p, tile, MechPartType.Led, INSTALL_COUNT)));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FAN, INSTALL_CHANCE_DENOMINATOR, INSTALL_COUNT);
    }

    public sealed class GreenScreenArtifact : MechanicalArtifact
    {
        private const double FAN_MULTIPLIER = 1.2;
        private const int GIFT_COUNT = 10;

        public GreenScreenArtifact() : base("green_screen", Rarity.EPIC) => SetMaterialGift(TileMaterial.MechLed, GIFT_COUNT);
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material) ||
                !tile.ContainsGreen(player) || TileMaterialMechPart.CountLedColors(tile, player) != 1) return;
            int triggers = material.GetPartCount(MechPartType.Led) * CommonTriggers(material);
            for (int i = 0; i < triggers; i++) effects.Add(ScoreEffect.MulFan(FAN_MULTIPLIER, this));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), FAN_MULTIPLIER);
    }

    public sealed class ScrapBinArtifact : MechanicalArtifact
    {
        private const int ADD_FU = 20;
        private const int MONEY_PER_PART = 1;
        private const int BASE_FU_PER_PART = 5;

        public ScrapBinArtifact() : base("scrap_bin", Rarity.COMMON) { }
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects) =>
            effects.Add(ScoreEffect.AddFu(ADD_FU, this));

        [SubscribeToEvent]
        private void RewardRecycling(MechanicalPartChangedEvent evt)
        {
            if (evt.kind is not (MechanicalPartChangeKind.Destroyed or MechanicalPartChangeKind.Transformed) ||
                TileMaterialMechPart.GetPartRarity(evt.from) != Rarity.COMMON || evt.count <= 0) return;
            evt.player.EarnMoney(MONEY_PER_PART * evt.count);
            if (evt.tile != null) evt.tile.addonFu += BASE_FU_PER_PART * evt.count;
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FU, MONEY_PER_PART, BASE_FU_PER_PART);
    }

    public sealed class StepperMotorArtifact : MechanicalArtifact
    {
        private const int ADD_FU = 20;
        private const int INSTALL_COUNT = 1;

        public StepperMotorArtifact() : base("stepper_motor", Rarity.COMMON) { }
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects) =>
            effects.Add(ScoreEffect.AddFu(ADD_FU, this));
        [SubscribeToEvent]
        private void AddDriveRod(MechanicalTileGrownEvent evt)
        {
            if (evt.tile.IsNumbered()) MechanicalArtifactUtility.InstallParts(evt.player, evt.tile, MechPartType.DriveRod, INSTALL_COUNT);
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FU, INSTALL_COUNT);
    }

    public sealed class PieMagnetArtifact : LevelingArtifact
    {
        private const int BASE_CHANCE = 10;
        private const int PERCENT_SCALE = 100;
        private const int ROUND_GROWTH = 3;
        private const int INSTALL_COUNT = 1;

        public PieMagnetArtifact() : base("pie_magnet", Rarity.RARE, 0)
        {
            setIn.Add("mech_parts");
            SetHighlightRequirement((tile, _) => tile.GetCategory() == Tile.Category.Bing);
        }
        public override int GetSellingPrice() => base.GetSellingPrice() + Level;
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            int chance = Math.Min(PERCENT_SCALE, BASE_CHANCE + Level);
            if (!player.IsPlayingTile(tile) || tile.GetCategory() != Tile.Category.Bing ||
                player.GenerateRandomInt(PERCENT_SCALE, "pie_magnet") >= chance) return;
            MechPartType part = MechanicalArtifactUtility.RandomCommon(player);
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_pie_magnet_install",
                p => MechanicalArtifactUtility.InstallParts(p, tile, part, INSTALL_COUNT, true)));
        }
        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects) =>
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_pie_magnet_level_up",
                _ => Level += ROUND_GROWTH, "SpendMoney"));
        public override (string, double) GetAdditionalDisplayingInfo(Player player) =>
            ("<style=\"money\">{0}</style>", Level);
        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ROUND_GROWTH, INSTALL_COUNT);
    }

    public sealed class OverclockModuleArtifact : MechanicalArtifact
    {
        private const int EXTRA_SCORES = 1;

        private readonly HashSet<Tile> scoring = new();
        public OverclockModuleArtifact() : base("overclock_module", Rarity.RARE) { }
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile) || tile.properties.mask is TileMaskSuppressed ||
                tile.properties.material is not TileMaterialMechPart ||
                player.playHandEffectStack == null || !scoring.Add(tile)) return;
            for (int i = 0; i < EXTRA_SCORES; i++)
                effects.Add(new TileScoringEffectAppendEffect(player, tile, permutation, player.playHandEffectStack));
            effects.Add(new SuppressEffect(tile));
            effects.Add(new SilentEffect(() => scoring.Remove(tile)));
        }
        public override void ResetArtifactState() { base.ResetArtifactState(); scoring.Clear(); }
        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), EXTRA_SCORES);
    }

    public sealed class WorkshopManualArtifact : MechanicalArtifact
    {
        private const int ADD_FU = 20;
        private const int UPGRADE_LEVELS = 1;
        private const int UPGRADE_COUNT = 1;

        public WorkshopManualArtifact() : base("workshop_manual", Rarity.RARE) { }
        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects) =>
            effects.Add(ScoreEffect.AddFu(ADD_FU, this));
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile) || tile.properties.material is not TileMaterialMechPart material ||
                material.GetParts().Count != TileMaterialMechPart.MaxParts) return;
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_workshop_manual_upgrade", p =>
            {
                if (YakuTester.InfoMap == null) return;
                List<YakuType> candidates = p.GetLearntYakus()
                    .Where(type => YakuTester.InfoMap.TryGetValue(type, out Yaku yaku) && yaku.rarity == Rarity.RARE)
                    .ToList();
                for (int i = 0; i < UPGRADE_COUNT && candidates.Count > 0; i++)
                    p.UpgradeYaku(candidates[p.GenerateRandomInt(candidates.Count, "workshop_manual")], UPGRADE_LEVELS);
            }, "book"));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FU, UPGRADE_LEVELS, UPGRADE_COUNT);
    }

    public sealed class LeverArtifact : MechanicalArtifact
    {
        private const int TARGET_COUNT = 2;
        private const int INSTALL_COUNT = 1;

        public LeverArtifact() : base("lever", Rarity.COMMON) { }
        [SubscribeToEvent]
        private void OnKong(PostKongTilesEvent evt)
        {
            List<Tile> candidates = evt.block?.tiles.Where(tile => tile != null &&
                (tile.properties.material is not TileMaterialMechPart material || material.HasEmptySlot)).ToList()
                ?? new List<Tile>();
            for (int i = 0; i < TARGET_COUNT && candidates.Count > 0; i++)
            {
                int index = evt.player.GenerateRandomInt(candidates.Count, "lever");
                Tile tile = candidates[index];
                candidates.RemoveAt(index);
                MechanicalArtifactUtility.InstallParts(evt.player, tile, MechPartType.DriveRod, INSTALL_COUNT);
            }
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), TARGET_COUNT, INSTALL_COUNT);
    }

    public sealed class ThreeProngTerminalStripArtifact : MechanicalArtifact
    {
        private const int INSTALL_COUNT = 1;

        private readonly HashSet<int> playedHonors = new();
        public ThreeProngTerminalStripArtifact() : base("three_prong_terminal_strip", Rarity.RARE)
        {
            deckIn.Add("galaxy");
        }
        [SubscribeToEvent] private void Reset(PlayerRoundEvent.Start.Pre evt) => playedHonors.Clear();
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (!player.IsPlayingTile(tile) || tile.GetCategory() != Tile.Category.Jian ||
                tile.GetOrder() is < 5 or > 7 || !playedHonors.Add(tile.GetOrder())) return;
            MechPartType part = tile.GetOrder() switch
            { 7 => MechPartType.Gear, 6 => MechPartType.Led, _ => MechPartType.Shield };
            string textKey = tile.GetOrder() switch
            {
                7 => "effect_three_prong_terminal_strip_red",
                6 => "effect_three_prong_terminal_strip_green",
                _ => "effect_three_prong_terminal_strip_white"
            };
            effects.Add(MechanicalArtifactUtility.ActionEffect(this, textKey,
                p => MechanicalArtifactUtility.InstallParts(p, tile, part, INSTALL_COUNT)));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), INSTALL_COUNT);
    }

    public sealed class MechanicalD9Artifact : MechanicalArtifact
    {
        private const int INSTALL_COUNT = 1;

        private const int Faces = 9;
        private int rolled = 1;
        public MechanicalD9Artifact() : base("mechanical_d9", Rarity.COMMON) { }
        [SubscribeToEvent]
        private void Roll(PlayerRoundEvent.Start.Pre evt)
        {
            rolled = evt.player.GenerateRandomInt(Faces, "mechanical_d9") + 1;
            if (evt.player.GetArtifacts().Contains(Artifacts.LeadBlock)) rolled = Faces;
        }

        public override string GetDescription(Func<string, string> localizer) => GetDescription(null, localizer);

        public override string GetDescription(Player player, Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), rolled, INSTALL_COUNT);
        public override (string, double) GetAdditionalDisplayingInfo(Player player) => ("{0}", rolled);
        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            rolled = 1;
        }
        public override string Serialize() => rolled.ToString();
        public override void Deserialize(string data)
        { rolled = int.TryParse(data, out int value) ? Math.Max(1, Math.Min(Faces, value)) : 1; }
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (player.IsPlayingTile(tile) && tile.IsNumbered() && tile.GetOrder() == rolled)
                effects.Add(MechanicalArtifactUtility.ActionEffect(this, "effect_mechanical_d9_install",
                    p => MechanicalArtifactUtility.InstallParts(p, tile, MechPartType.Shield, INSTALL_COUNT)));
        }
    }

    public sealed class LongBearingArtifact : MechanicalArtifact
    {
        private const int ADD_FAN = 5;
        private const int GIFT_COUNT = 5;

        public LongBearingArtifact() : base("long_bearing", Rarity.RARE) => SetMaterialGift(TileMaterial.MechGear, GIFT_COUNT);
        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            if (permutation == null || !MechanicalArtifactUtility.TryGetMaterial(tile, out TileMaterialMechPart material)) return;
            List<Block> sequences = permutation.blocks.Where(block => block.IsABC() && block.IsNumbered()).ToList();
            bool connected = sequences.Any(first => sequences.Any(second => first != second &&
                first.tiles.Max(t => t.GetOrder()) + 1 == second.tiles.Min(t => t.GetOrder()) &&
                (first.tiles.Contains(tile) || second.tiles.Contains(tile))));
            if (!connected) return;
            int parts = material.GetPartCount(MechPartType.Gear) + material.GetPartCount(MechPartType.DriveRod);
            for (int i = 0; i < parts * CommonTriggers(material); i++) effects.Add(ScoreEffect.AddFan(ADD_FAN, this));
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), ADD_FAN);
    }

    public sealed class EcosphereArtifact : MechanicalArtifact
    {
        private const int INSTALL_COUNT = 1;

        public EcosphereArtifact() : base("ecosphere", Rarity.RARE)
        {
            deckIn.Add("rainbow_deck");
        }
        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if (player is RainbowDeck.RainbowPlayer rainbow) rainbow.PostPlayFlowerTileEvent += OnFlowerPlayed;
        }
        public override void UnsubscribeToPlayer(Player player)
        {
            if (player is RainbowDeck.RainbowPlayer rainbow) rainbow.PostPlayFlowerTileEvent -= OnFlowerPlayed;
            base.UnsubscribeToPlayer(player);
        }
        private void OnFlowerPlayed(Player player, FlowerTile flower)
        {
            if (player is not RainbowDeck.RainbowPlayer rainbow) return;
            List<Tile> candidates = MechanicalArtifactUtility.InstallableTiles(player.GetHandDeckCopy()).ToList();
            if (candidates.Count == 0) return;
            bool completedSet = new[] { 1, 2, 3, 4 }.All(order => rainbow.PlayedFlowerTiles.Any(tile =>
                tile.GetCategory() == flower.GetCategory() && tile.GetOrder() == order));
            Tile target = candidates[player.GenerateRandomInt(candidates.Count, "ecosphere_target")];
            MechPartType part = completedSet ? MechanicalArtifactUtility.RandomEpic(player) :
                MechanicalArtifactUtility.RandomCommon(player);
            MechanicalArtifactUtility.InstallParts(player, target, part, INSTALL_COUNT);
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), INSTALL_COUNT);
    }

    public sealed class MechanicalPouchArtifact : MechanicalArtifact
    {
        private const int INSTALL_COUNT = 1;

        public MechanicalPouchArtifact() : base("mechanical_pouch", Rarity.RARE)
        {
            deckIn.Add("sneaky");
        }
        [SubscribeToEvent]
        private void AddTemporaryParts(PlayerRoundEvent.Start.Post evt)
        {
            if (evt.player is not SneakyPlayer sneaky) return;
            foreach (Tile tile in evt.player.GetHandDeckCopy().Where(sneaky.SneakedLastRound))
                MechanicalArtifactUtility.InstallParts(evt.player, tile,
                    MechanicalArtifactUtility.RandomCommon(evt.player), INSTALL_COUNT, true);
        }

        public override string GetDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, base.GetDescription(localizer), INSTALL_COUNT);
    }
}
