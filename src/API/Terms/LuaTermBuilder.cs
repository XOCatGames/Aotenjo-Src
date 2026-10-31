using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Aotenjo
{
    /// <summary>
    /// Registers session-only TMP keyword styles for Lua mods. Tooltip text is read
    /// from tooltip_{id}_header and tooltip_{id}_content in the mod's lang files.
    /// </summary>
    public static class LuaTermBuilder
    {
        private static readonly ConstructorInfo StyleConstructor = typeof(TMP_Style).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic, null,
            new[] { typeof(string), typeof(string), typeof(string) }, null);
        private static readonly FieldInfo StyleListField = typeof(TMP_StyleSheet).GetField(
            "m_StyleList", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo DefaultStyleSheetField = typeof(TMP_Settings).GetField(
            "m_defaultStyleSheet", BindingFlags.Instance | BindingFlags.NonPublic);

        private static TMP_Settings settings;
        private static TMP_StyleSheet originalStyleSheet;
        private static TMP_StyleSheet runtimeStyleSheet;

        /// <summary>
        /// Registers an ID using yellow, purple, green, blue or red. IDs and colors
        /// are normalized to lowercase. Invalid or conflicting registrations return false.
        /// Call from init.lua before displaying text containing the new style.
        /// </summary>
        public static bool Register(string id, string color)
        {
            id = id?.ToLowerInvariant();
            color = color?.ToLowerInvariant();
            if (!IsValidId(id))
                return Reject("ID must start with a letter and contain only ASCII letters, digits or underscores (max 128 characters).");
            if (color != "yellow" && color != "purple" && color != "green" && color != "blue" && color != "red")
                return Reject($"Unsupported color '{color}' for '{id}'.");
            // 这些前缀由专用悬停规则解析，模组术语不能占用，否则会显示错误类型的说明。
            if (id.StartsWith("pattern_", StringComparison.Ordinal) ||
                id.StartsWith("gadget_", StringComparison.Ordinal) ||
                id.StartsWith("tilemat_", StringComparison.Ordinal) ||
                id.StartsWith("tilefont_", StringComparison.Ordinal) ||
                id.StartsWith("boss_reversed_effect_", StringComparison.Ordinal) || id == "jixiepai")
                return Reject($"ID '{id}' is reserved for a specialized tooltip.");
            if (StyleConstructor == null || StyleListField == null || DefaultStyleSheetField == null)
                return Reject("The installed TMP version does not expose the expected style members.");

            TMP_Settings currentSettings = TMP_Settings.instance;
            TMP_StyleSheet sheet = currentSettings == null ? null : TMP_Settings.defaultStyleSheet;
            if (sheet == null)
                return Reject("The default TMP style sheet is not available.");
            if (runtimeStyleSheet != null && (currentSettings != settings || sheet != runtimeStyleSheet))
                return Reject("The default TMP style sheet changed during this registration session.");
            if (sheet.GetStyle(id) != null)
                return Reject($"Style '{id}' already exists; the existing definition was kept.");
            foreach (TMP_StyleSheet localSheet in Resources.LoadAll<TMP_StyleSheet>("Style Sheets"))
            {
                if (localSheet.GetStyle(id) != null)
                    return Reject($"Style '{id}' already exists in '{localSheet.name}'; the existing definition was kept.");
            }
            if (sheet.GetStyle(color) == null)
                return Reject($"Color style '{color}' is not available.");

            // Clone before modifying: Play Mode and mod loading must not edit the source asset.
            if (runtimeStyleSheet == null)
            {
                settings = currentSettings;
                originalStyleSheet = sheet;
                runtimeStyleSheet = UnityEngine.Object.Instantiate(sheet);
                runtimeStyleSheet.name = sheet.name + " (Lua terms)";
                runtimeStyleSheet.hideFlags = HideFlags.HideAndDontSave;
                DefaultStyleSheetField.SetValue(settings, runtimeStyleSheet);
                Application.quitting -= ResetSession;
                Application.quitting += ResetSession;
            }

            var styles = (List<TMP_Style>)StyleListField.GetValue(runtimeStyleSheet);
            var style = (TMP_Style)StyleConstructor.Invoke(new object[]
            {
                id, $"<style=\"{color}\"><link=\"{id}\">", "</link></style>"
            });
            styles.Add(style);
            runtimeStyleSheet.RefreshStyles();
            return true;
        }

        private static bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length > 128 || id[0] < 'a' || id[0] > 'z') return false;
            foreach (char c in id)
                if (!(c >= 'a' && c <= 'z') && !(c >= '0' && c <= '9') && c != '_') return false;
            return true;
        }

        private static bool Reject(string reason)
        {
            Debug.LogWarning("[LuaTermBuilder] " + reason);
            return false;
        }

        internal static void BeginLoadSession() => ResetSession();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            Application.quitting -= ResetSession;
            if (settings != null && runtimeStyleSheet != null &&
                ReferenceEquals(DefaultStyleSheetField.GetValue(settings), runtimeStyleSheet))
                DefaultStyleSheetField.SetValue(settings, originalStyleSheet);
            if (runtimeStyleSheet != null)
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(runtimeStyleSheet);
                else UnityEngine.Object.DestroyImmediate(runtimeStyleSheet);
            }
            settings = null;
            originalStyleSheet = null;
            runtimeStyleSheet = null;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InstallEditorCleanup()
        {
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= ResetSession;
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += ResetSession;
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) ResetSession();
        }
#endif
    }
}
