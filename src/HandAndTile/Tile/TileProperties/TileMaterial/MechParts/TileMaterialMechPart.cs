using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialMechPart : TileMaterial
    {
        public const int MaxParts = 3;
        public const int IntegratedChipExtraTriggers = 2;
        public const int InstalledTileSpriteId = 82;

        private const int LevelsPerCircle = 4;
        private const int GearPreviousFan = 1;
        private const int GearInitialFan = 2;
        private const int DriveRodFu = 10;
        private const int NetworkCardDiscards = 1;
        private const int LedMoneyPerColor = 1;
        private const int ShieldFan = 8;
        private const int ReactorDestroyedParts = 1;
        private const int ReactorExtraScores = 1;

        private const string PartSpriteSheet = "Tile/TileMaterial/MechParts";
        private const string InsertedSpriteSuffix = "_插入";

        [SerializeField] private List<MechPartType> parts = new();
        [SerializeField] private List<bool> temporaryParts = new();
        [SerializeField] private double permanentBaseFu;
        [SerializeReference] private TileMaterial materialBeforeTemporaryConversion;

        // The guard belongs to the material because all mechanical-part state must stay out of Tile.
        [NonSerialized] private HashSet<Tile> reactorScoringTiles = new();

        public TileMaterialMechPart(int id, MechPartType part, bool temporary = false) : base(id, GetNameKey(part), null)
        {
            parts.Add(part);
            temporaryParts.Add(temporary);
        }

        private TileMaterialMechPart(int id, string nameKey, IEnumerable<MechPartType> parts,
            IEnumerable<bool> temporaryParts, double permanentBaseFu,
            TileMaterial materialBeforeTemporaryConversion)
            : base(id, nameKey, null)
        {
            this.parts = parts.Take(MaxParts).ToList();
            this.temporaryParts = temporaryParts.Take(MaxParts).ToList();
            this.permanentBaseFu = permanentBaseFu;
            this.materialBeforeTemporaryConversion = materialBeforeTemporaryConversion;
            NormalizePartMetadata();
        }

        public TileMaterialMechPart()
            : base(83, GetNameKey(MechPartType.Gear), null)
        {
            parts.Add(MechPartType.Gear);
            temporaryParts.Add(false);
        }

        public IReadOnlyList<MechPartType> GetParts()
        {
            parts ??= new List<MechPartType>();
            NormalizePartMetadata();
            return parts;
        }

        public bool IsPartTemporary(int index)
        {
            NormalizePartMetadata();
            return index >= 0 && index < temporaryParts.Count && temporaryParts[index];
        }

        public int GetPartCount(MechPartType part)
        {
            return GetParts().Count(installed => installed == part);
        }

        public int GetCommonTriggerCount()
        {
            return 1 + GetPartCount(MechPartType.IntegratedChip) * IntegratedChipExtraTriggers;
        }

        public bool HasEmptySlot => GetParts().Count < MaxParts;

        public override Rarity GetRarity()
        {
            return GetParts().Any(part => GetPartRarity(part) == Rarity.EPIC) ? Rarity.EPIC : Rarity.COMMON;
        }

        public override Effect[] GetEffects(Player player, Permutation permutation)
        {
            return BuildScoringPartEffects(player).ToArray();
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            effects.AddRange(BuildScoringPartEffects(player));

            bool hasPlayedPart = GetParts().Any(part => part == MechPartType.Led);
            if (hasPlayedPart && player != null && tile != null && player.IsPlayingTile(tile))
            {
                effects.AddRange(BuildPlayedPartEffects(player, tile));
            }

            AppendReactorEffects(player, perm, tile, effects);
        }

        public override TileMaterial Copy()
        {
            NormalizePartMetadata();
            return new TileMaterialMechPart(GetSpriteID(), nameKey.Replace("_material", ""), GetParts(),
                temporaryParts, permanentBaseFu, materialBeforeTemporaryConversion?.Copy());
        }

        public bool TryInstall(TileMaterialMechPart incoming)
        {
            bool changed = false;
            for (int i = 0; i < incoming.GetParts().Count; i++)
            {
                if (!HasEmptySlot) break;
                parts.Add(incoming.GetParts()[i]);
                bool temporary = incoming.IsPartTemporary(i);
                temporaryParts.Add(temporary);
                if (!temporary) materialBeforeTemporaryConversion = null;
                changed = true;
            }

            return changed;
        }

        public bool TryInstall(MechPartType part, bool temporary = false)
        {
            NormalizePartMetadata();
            if (!HasEmptySlot) return false;
            parts.Add(part);
            temporaryParts.Add(temporary);
            if (!temporary) materialBeforeTemporaryConversion = null;
            return true;
        }

        public void RememberMaterialBeforeTemporaryConversion(TileMaterial material)
        {
            materialBeforeTemporaryConversion ??= material?.Copy();
        }

        public TileMaterial TakeMaterialBeforeTemporaryConversion()
        {
            TileMaterial material = materialBeforeTemporaryConversion ?? TileMaterial.PLAIN;
            materialBeforeTemporaryConversion = null;
            return material;
        }

        public static TileMaterialMechPart Create(MechPartType part, bool temporary = false)
        {
            int spriteId = part switch
            {
                MechPartType.Gear => 83,
                MechPartType.DriveRod => 84,
                MechPartType.NetworkCard => 85,
                MechPartType.Led => 86,
                MechPartType.Shield => 87,
                MechPartType.IntegratedChip => 88,
                MechPartType.Reactor => 89,
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
            };
            return new TileMaterialMechPart(spriteId, part, temporary);
        }

        public bool TryRemoveRandomCommonPart(Player player, Tile carrier, out MechPartType removed,
            MechanicalPartChangeKind kind = MechanicalPartChangeKind.Destroyed)
        {
            NormalizePartMetadata();
            List<int> candidates = Enumerable.Range(0, parts.Count)
                .Where(index => GetPartRarity(parts[index]) == Rarity.COMMON)
                .ToList();
            if (candidates.Count == 0)
            {
                removed = default;
                return false;
            }

            int index = candidates[player.GenerateRandomInt(candidates.Count, "mech_remove_part")];
            removed = parts[index];
            parts.RemoveAt(index);
            temporaryParts.RemoveAt(index);
            EventBus.Publish(new MechanicalPartChangedEvent(player, carrier, kind, removed, null, 1));
            return true;
        }

        public int TransformAll(Player player, Tile carrier, MechPartType from, MechPartType to)
        {
            int transformed = 0;
            for (int i = 0; i < GetParts().Count; i++)
            {
                if (parts[i] != from) continue;
                parts[i] = to;
                transformed++;
            }

            if (transformed > 0)
            {
                EventBus.Publish(new MechanicalPartChangedEvent(player, carrier,
                    MechanicalPartChangeKind.Transformed, from, to, transformed));
            }
            return transformed;
        }

        public bool TransferRandomNonNetworkPartTo(Player player, Tile source, Tile target)
        {
            if (target?.properties?.material is not TileMaterialMechPart targetMaterial ||
                !targetMaterial.HasEmptySlot)
            {
                return false;
            }

            NormalizePartMetadata();
            List<int> candidates = Enumerable.Range(0, parts.Count)
                .Where(index => parts[index] != MechPartType.NetworkCard)
                .ToList();
            if (candidates.Count == 0) return false;

            int index = candidates[player.GenerateRandomInt(candidates.Count, "mech_transfer_part")];
            MechPartType part = parts[index];
            bool temporary = temporaryParts[index];
            parts.RemoveAt(index);
            temporaryParts.RemoveAt(index);
            targetMaterial.TryInstall(part, temporary);
            EventBus.Publish(new MechanicalPartChangedEvent(player, source,
                MechanicalPartChangeKind.Transferred, part, part, 1, target));
            return true;
        }

        public int RemoveTemporaryParts()
        {
            NormalizePartMetadata();
            int removed = 0;
            for (int index = parts.Count - 1; index >= 0; index--)
            {
                if (!temporaryParts[index]) continue;
                parts.RemoveAt(index);
                temporaryParts.RemoveAt(index);
                removed++;
            }
            if (parts.Count > 0) materialBeforeTemporaryConversion = null;
            return removed;
        }

        public override Sprite GetSprite(Player player)
        {
            return base.GetSprite(player);
        }

        public static Sprite GetPartSprite(MechPartType part, bool inserted)
        {
            string spriteName = GetPartSpriteName(part) + (inserted ? InsertedSpriteSuffix : string.Empty);
            return SpriteManager.GetSpritesOrLoad(PartSpriteSheet)
                .FirstOrDefault(sprite => sprite.name == spriteName);
        }

        public static string GetPartSpriteName(MechPartType part)
        {
            return part switch
            {
                MechPartType.Gear => "齿轮",
                MechPartType.DriveRod => "传动杆",
                MechPartType.NetworkCard => "网卡",
                MechPartType.Led => "LED",
                MechPartType.Shield => "屏蔽器",
                MechPartType.IntegratedChip => "集成芯片",
                MechPartType.Reactor => "反应炉",
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
            };
        }

        protected override string GetDescription(Func<string, string> localizer)
            => GetDescription(localizer, null);

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(localizer("tile_mech_part_material_description"), GetParts().Count, MaxParts,
                GetPartsDescription(localizer, player));
        }

        public static string GetSlotDescription(Func<string, string> localizer) =>
            string.Format(CultureInfo.InvariantCulture, localizer("tooltip_jixiepai_content"), MaxParts);

        public static string GetPartDescription(MechPartType part, Func<string, string> localizer, Player player = null)
        {
            string description = localizer($"tile_{GetNameKey(part)}_material_description");
            return part switch
            {
                MechPartType.Gear => string.Format(CultureInfo.InvariantCulture, description, GetGearFan(player)),
                MechPartType.DriveRod => string.Format(CultureInfo.InvariantCulture, description, DriveRodFu),
                MechPartType.NetworkCard => string.Format(CultureInfo.InvariantCulture, description, NetworkCardDiscards),
                MechPartType.Led => string.Format(CultureInfo.InvariantCulture, description, LedMoneyPerColor),
                MechPartType.Shield => string.Format(CultureInfo.InvariantCulture, description, ShieldFan),
                MechPartType.IntegratedChip => string.Format(CultureInfo.InvariantCulture, description, IntegratedChipExtraTriggers),
                MechPartType.Reactor => string.Format(CultureInfo.InvariantCulture, description, ReactorDestroyedParts, ReactorExtraScores),
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
            };
        }

        public string GetPartsDescription(Func<string, string> localizer, Player player = null)
        {
            return string.Join("\n", GetParts().Select((part, index) =>
            {
                string partName = localizer($"tile_{GetNameKey(part)}_material_name");
                if (IsPartTemporary(index)) partName += localizer("tile_mech_part_temporary_suffix");
                return $"<style=\"yellow\">{partName}</style>: " +
                       GetPartDescription(part, localizer, player);
            }));
        }

        public string GetPartSubheader(Func<string, string> localizer)
        {
            string rarityName = GetRarity() == Rarity.COMMON ? "common" : "epic";
            return localizer($"rarity_{rarityName}_name") + " " + localizer("tile_mech_part_name");
        }

        public static TileProperties ResolveIncomingMaterial(TileProperties current, TileProperties incoming)
        {
            if (current?.material is not TileMaterialMechPart currentMech ||
                incoming?.material is not TileMaterialMechPart incomingMech ||
                ReferenceEquals(currentMech, incomingMech))
            {
                return incoming;
            }

            TileMaterialMechPart merged = (TileMaterialMechPart)currentMech.Copy();
            if (!merged.TryInstall(incomingMech) && incomingMech.GetParts().Count > 0)
            {
                // Explicit enhancement replaces slot zero when full; automatic installs still need an empty slot.
                merged.parts[0] = incomingMech.GetParts()[0];
                merged.temporaryParts[0] = incomingMech.IsPartTemporary(0);
                if (!merged.temporaryParts[0]) merged.materialBeforeTemporaryConversion = null;
            }
            incoming.ChangeMaterial(merged);
            return incoming;
        }

        private static double GetGearFan(Player player)
        {
            int completedCircles = (Math.Max(1, player?.Level ?? 1) - 1) / LevelsPerCircle;
            double previous = GearPreviousFan;
            double current = GearInitialFan;
            for (int circle = 0; circle < completedCircles; circle++)
            {
                (previous, current) = (current, previous + current);
            }
            return current;
        }

        private IEnumerable<Effect> BuildScoringPartEffects(Player player)
        {
            int triggerCount = GetCommonTriggerCount();
            bool shieldActive = GetParts().All(part => part == MechPartType.Shield);

            foreach (MechPartType part in GetParts())
            {
                if (part == MechPartType.Shield && !shieldActive)
                {
                    continue;
                }

                for (int trigger = 0; trigger < triggerCount; trigger++)
                {
                    switch (part)
                    {
                        case MechPartType.Gear:
                            yield return ScoreEffect.AddFan(GetGearFan(player), null);
                            break;
                        case MechPartType.DriveRod:
                            yield return ScoreEffect.AddFu(DriveRodFu, null);
                            break;
                        case MechPartType.Shield:
                            yield return ScoreEffect.AddFan(ShieldFan, null);
                            break;
                    }
                }
            }
        }

        private IEnumerable<Effect> BuildPlayedPartEffects(Player player, Tile tile)
        {
            int triggerCount = GetCommonTriggerCount();
            IReadOnlyList<LedDetectedColor> colors = GetLedColors(tile, player);
            IReadOnlyList<MechPartType> installedParts = GetParts();
            for (int slot = 0; slot < installedParts.Count; slot++)
            {
                MechPartType part = installedParts[slot];
                for (int trigger = 0; trigger < triggerCount; trigger++)
                {
                    switch (part)
                    {
                        case MechPartType.Led when tile != null && player != null:
                            foreach (LedDetectedColor color in colors)
                            {
                                yield return new LedEarnMoneyEffect(LedMoneyPerColor, color, slot);
                            }
                            break;
                    }
                }
            }
        }

        private void AppendReactorEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            int reactorCount = GetPartCount(MechPartType.Reactor);
            if (reactorCount == 0 || player?.playHandEffectStack == null || tile == null ||
                !player.IsPlayingTile(tile))
            {
                return;
            }

            reactorScoringTiles ??= new HashSet<Tile>();
            if (!reactorScoringTiles.Add(tile))
            {
                return;
            }

            for (int i = 0; i < reactorCount * ReactorDestroyedParts; i++)
            {
                effects.Add(new SimpleEffect((_, loc) => loc("tile_mech_reactor_material_name"), null,
                    p => TryRemoveRandomCommonPart(p, tile, out _), "Break"));
            }

            for (int i = 0; i < reactorCount * ReactorExtraScores; i++)
            {
                effects.Add(new TileScoringEffectAppendEffect(player, tile, perm, player.playHandEffectStack));
            }

            effects.Add(new SilentEffect(() => reactorScoringTiles.Remove(tile)));
        }

        public static int CountPrimaryColors(Tile tile, Player player)
        {
            int count = 0;
            if (tile.ContainsRed(player)) count++;
            if (tile.ContainsGreen(player)) count++;
            if (tile.ContainsBlue(player)) count++;
            return count;
        }

        public static int CountLedColors(Tile tile, Player player)
        {
            return GetLedColors(tile, player).Count;
        }

        public static IReadOnlyList<LedDetectedColor> GetLedColors(Tile tile, Player player)
        {
            var colors = new List<LedDetectedColor>(4);
            if (tile == null || player == null) return colors;
            if (tile.ContainsRed(player)) colors.Add(LedDetectedColor.Red);
            if (tile.ContainsGreen(player)) colors.Add(LedDetectedColor.Green);
            if (tile.ContainsBlue(player)) colors.Add(LedDetectedColor.Blue);

            // Black and faded are separate detectable colors, but a single face can only be one of them.
            if (player.GetArtifacts().Contains(Artifacts.Blackbody) && HasBlackOrFadedFace(tile))
                colors.Add(tile.properties.font.GetRegName() == TileFont.COLORLESS.GetRegName()
                    ? LedDetectedColor.Faded : LedDetectedColor.Black);
            return colors;
        }

        private static bool HasBlackOrFadedFace(Tile tile)
        {
            string font = tile.properties.font.GetRegName();
            if (font == TileFont.COLORLESS.GetRegName()) return true;
            if (font != TileFont.PLAIN.GetRegName()) return false;

            // Order markers retain the visible face, while dyes replace its original colors.
            Pair<Tile.Category, int> display = tile.GetLastVisibleDisplay();
            int order = display.elem2;
            // Black pixels in Tile/TileFont coexist with red and green on several default faces.
            return display.elem1 switch
            {
                Tile.Category.Wan or Tile.Category.Bing => order is >= 1 and <= 9,
                Tile.Category.Feng => order is >= 1 and <= 4,
                Tile.Category.Jian => order == 5,
                Tile.Category.SiJi or Tile.Category.SiYi or Tile.Category.SiYe => order is >= 1 and <= 4,
                Tile.Category.JunZi => order == 1,
                _ => false
            };
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm,
            List<IAnimationEffect> effects, Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            int triggerCount = GetCommonTriggerCount();
            int networkCards = GetPartCount(MechPartType.NetworkCard);
            for (int i = 0; i < networkCards * triggerCount; i++)
            {
                effects.Add(new IncreDiscardEffect("mech_network_card", null, NetworkCardDiscards).OnTile(tile, isClone));
            }
        }

        private void NormalizePartMetadata()
        {
            parts ??= new List<MechPartType>();
            temporaryParts ??= new List<bool>();
            if (parts.Count > MaxParts) parts.RemoveRange(MaxParts, parts.Count - MaxParts);
            if (temporaryParts.Count > parts.Count)
                temporaryParts.RemoveRange(parts.Count, temporaryParts.Count - parts.Count);
            while (temporaryParts.Count < parts.Count) temporaryParts.Add(false);
        }

        public static Rarity GetPartRarity(MechPartType part)
        {
            return part is MechPartType.IntegratedChip or MechPartType.Reactor
                ? Rarity.EPIC
                : Rarity.COMMON;
        }

        public static string GetNameKey(MechPartType part)
        {
            return part switch
            {
                MechPartType.Gear => "mech_gear",
                MechPartType.DriveRod => "mech_drive_rod",
                MechPartType.NetworkCard => "mech_network_card",
                MechPartType.Led => "mech_led",
                MechPartType.Shield => "mech_shield",
                MechPartType.IntegratedChip => "mech_integrated_chip",
                MechPartType.Reactor => "mech_reactor",
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
            };
        }
    }

    public enum MechPartType
    {
        Gear,
        DriveRod,
        NetworkCard,
        Led,
        Shield,
        IntegratedChip,
        Reactor
    }
}
