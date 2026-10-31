using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;
using Newtonsoft.Json;

/// <summary>
/// 组合本轮东、南、西风圈的三个 Boss。仅保存本次遭遇拥有的实例，不订阅注册表中的共享预览。
/// 每轮以十六关为一组，故无尽模式也只继承当前轮；缺失或递归引用的位置由「不对」补齐。
/// </summary>
[Serializable]
public class LosslessBoss : Boss
{
    private const string AbilityLinkPrefix = "boss_ability_";
    [NonSerialized] private Boss[] inheritedBosses;
    [NonSerialized] private Player subscribedPlayer;

    protected virtual bool UseHarderBosses => false;

    public LosslessBoss() : base("Lossless") { }

    private static string[] GetSourceBossNames(Player player)
    {
        int firstIndex = Math.Max(0, ((player?.Level ?? 1) - 1) / 16) * 4;
        var names = new string[3];
        for (int i = 0; i < names.Length; i++)
        {
            int index = firstIndex + i;
            names[i] = player?.upcomingBosses != null && index < player.upcomingBosses.Count
                ? player.upcomingBosses[index]
                : null;
        }
        return names;
    }

    private static Boss[] CreateInheritedBosses(IEnumerable<string> names, bool harderBoss)
    {
        string[] sourceNames = names?.Take(3).ToArray() ?? Array.Empty<string>();
        return Enumerable.Range(0, 3).Select(index => index < sourceNames.Length ? sourceNames[index] : null)
            .Select(bossName => CreateInheritedBoss(bossName, harderBoss)).ToArray();
    }

    private static Boss CreateInheritedBoss(string bossName, bool harderBoss)
    {
        // 缺失来源及不失自身不能走全局重抽，否则读取说明也会改变随机状态，或形成递归继承。
        // 「不对」只在不失内部补位，保留目标翻倍及对应反转；它不注册为可查询的独立 Boss。
        return bossName != "Lossless" && Bosses.TryCreateEncounter(bossName, harderBoss, out Boss boss)
            ? boss
            : new IncorrectBoss();
    }

    private Boss[] GetInheritedBosses(Player player)
    {
        // 实战说明必须读取当前子 Boss 的状态，例如「不定」已经切换的奇偶压制。
        // 非实战预览只读现有排期，不补抽、不订阅事件，也不消耗随机数。
        return inheritedBosses != null && ReferenceEquals(player, subscribedPlayer)
            ? inheritedBosses
            : CreateInheritedBosses(GetSourceBossNames(player), UseHarderBosses);
    }

    public override double GetStartingTarget(Player player, double basicTarget)
    {
        foreach (Boss boss in GetInheritedBosses(player))
            basicTarget = boss.GetStartingTarget(player, basicTarget);
        return basicTarget;
    }

    public override string GetDescription(Player player, Func<string, string> loc)
    {
        // 正文只列出东、南、西三个来源，能力详情交给已有的 TMP 悬停系统。
        // 链接保留难度、来源位置及身份，避免重复 Boss 或切换难度后显示错误的能力。
        string[] names = GetInheritedBosses(player).Select((boss, index) =>
            $"<link=\"{AbilityLinkPrefix}{(UseHarderBosses ? 1 : 0)}_{index}_{boss.name}\">" +
            $"<style=\"red\">{boss.GetName(player, loc)}</style></link>").ToArray();
        return string.Format(base.GetDescription(player, loc), names[0], names[1], names[2]);
    }

    /// <summary>
    /// 解析不失的来源链接。实战优先返回同一位置的子 Boss，让不定等有状态能力显示当前效果；
    /// 预览或难度不一致时创建只读实例，不订阅事件、不推进排期，缺失或递归身份仍显示不对。
    /// </summary>
    public static bool TryResolveAbilityLink(string linkId, Player player, out Boss boss)
    {
        boss = null;
        if (linkId == null || !linkId.StartsWith(AbilityLinkPrefix, StringComparison.Ordinal)) return false;
        string[] parts = linkId.Substring(AbilityLinkPrefix.Length).Split(new[] { '_' }, 3);
        if (parts.Length != 3 || (parts[0] != "0" && parts[0] != "1") ||
            !int.TryParse(parts[1], out int index) || index < 0 || index >= 3 || string.IsNullOrEmpty(parts[2]))
            return false;

        bool harder = parts[0] == "1";
        string name = parts[2];
        if (player?.currentBoss is LosslessBoss lossless && lossless.UseHarderBosses == harder)
        {
            Boss current = lossless.GetInheritedBosses(player)[index];
            if (current.name == name)
            {
                boss = current;
                return true;
            }
        }

        boss = CreateInheritedBoss(name, harder);
        return true;
    }

    public override void SubscribeToPlayerEvents(Player player)
    {
        // 读档重新绑定或显式重进关卡时先退订旧实例，再按东南西顺序创建并订阅。
        // 保持各 Boss 原有事件阶段和同阶段顺序，状态重置仍由各自的入场逻辑负责。
        if (subscribedPlayer != null)
            UnsubscribeFromPlayerEvents(subscribedPlayer);
        inheritedBosses = CreateInheritedBosses(GetSourceBossNames(player), UseHarderBosses);
        subscribedPlayer = player;
        foreach (Boss boss in inheritedBosses)
            boss.SubscribeToPlayerEvents(player);
    }

    public override void UnsubscribeFromPlayerEvents(Player player)
    {
        if (subscribedPlayer == null) return;
        foreach (Boss boss in inheritedBosses)
            boss.UnsubscribeFromPlayerEvents(subscribedPlayer);
        subscribedPlayer = null;
        inheritedBosses = null;
    }

    public override Boss GetHarderBoss() => new LosslessHarderBoss();

    public override Artifact GetReversedArtifact(Artifact baseArtifact)
    {
        return new LosslessReversedArtifact(baseArtifact, inheritedBosses?.Select(boss => boss.name).ToArray());
    }

    /// <summary>
    /// 仅供不失缺失来源的说明、悬停和组合能力使用，不能从全局注册表创建或被独立抽取。
    /// 每个缺失位置仍让不失的目标翻倍；葫芦吸收不失时保留该位置对应的反转能力。
    /// </summary>
    private sealed class IncorrectBoss : Boss
    {
        public IncorrectBoss() : base("Incorrect") { }

        public override double GetStartingTarget(Player player, double basicTarget) => basicTarget * 2;

        public override void SubscribeToPlayerEvents(Player player) { }

        public override void UnsubscribeFromPlayerEvents(Player player) { }

        public override Artifact GetReversedArtifact(Artifact baseArtifact)
        {
            return new IncorrectReversedArtifact(baseArtifact);
        }

        /// <summary>
        /// 只作为不失缺失来源的反转能力存在；来源保持为吸收它的遗物，每个位置独立追加一次翻番。
        /// </summary>
        private sealed class IncorrectReversedArtifact : Artifact
        {
            private readonly Artifact baseArtifact;

            public IncorrectReversedArtifact(Artifact baseArtifact) : base("Incorrect_reversed", Rarity.COMMON)
            {
                this.baseArtifact = baseArtifact;
            }

            public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
            {
                effects.Add(ScoreEffect.MulFan(2, baseArtifact));
            }
        }
    }

    /// <summary>
    /// 葫芦吸收的是击败时的三种反转能力，不能随无尽模式换轮而变更。
    /// 名称快照单独序列化；各计分入口和事件订阅完整转发给原有反转遗物，保留其触发阶段。
    /// </summary>
    private sealed class LosslessReversedArtifact : Artifact
    {
        private readonly Artifact baseArtifact;
        private string[] bossNames;
        private Artifact[] artifacts;
        private Player subscribedPlayer;

        public LosslessReversedArtifact(Artifact baseArtifact, string[] bossNames)
            : base("Lossless_reversed", Rarity.COMMON)
        {
            this.baseArtifact = baseArtifact;
            this.bossNames = bossNames;
        }

        private Artifact[] GetArtifacts(Player player)
        {
            if (artifacts != null) return artifacts;
            // 快照缺失时同样使用占位，不能拿读档时的新一轮 Boss 冒充吸收时的来源。
            Boss[] bosses = CreateInheritedBosses(bossNames, false);
            bossNames = bosses.Select(boss => boss.name).ToArray();
            return artifacts = bosses.Select(boss => boss.GetReversedArtifact(baseArtifact)).ToArray();
        }

        public override string GetDescription(Player player, Func<string, string> loc)
        {
            Artifact[] children = GetArtifacts(player);
            return base.GetDescription(player, loc) + "\n" + string.Join("\n",
                children.Select((artifact, i) => loc("boss_" + bossNames[i] + "_name") + ": " +
                    artifact.GetDescription(player, loc)));
        }

        public override string Serialize() => JsonConvert.SerializeObject(bossNames);

        public override void Deserialize(string data)
        {
            bossNames = string.IsNullOrEmpty(data) ? null : JsonConvert.DeserializeObject<string[]>(data);
            artifacts = null;
        }

        public override void SubscribeToPlayer(Player player)
        {
            if (subscribedPlayer != null) UnsubscribeToPlayer(subscribedPlayer);
            base.SubscribeToPlayer(player);
            subscribedPlayer = player;
            foreach (Artifact artifact in GetArtifacts(player)) artifact.SubscribeToPlayer(player);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            if (subscribedPlayer != null)
                foreach (Artifact artifact in GetArtifacts(subscribedPlayer)) artifact.UnsubscribeToPlayer(subscribedPlayer);
            subscribedPlayer = null;
            base.UnsubscribeToPlayer(player);
        }

        public override void ResetArtifactState(Player player)
        {
            base.ResetArtifactState(player);
            foreach (Artifact artifact in GetArtifacts(player)) artifact.ResetArtifactState(player);
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
            => GetArtifacts(player).Any(artifact => artifact.ShouldHighlightTile(tile, player));

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AppendOnTileEffects(player, permutation, tile, effects);
        }

        public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AddOnTileEffectsPostEvents(player, permutation, tile, effects);
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AddOnBlockEffects(player, permutation, block, effects);
        }

        public override void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block, List<IAnimationEffect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AppendPostBlockAnimationEffects(player, permutation, block, effects);
        }

        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AppendOnSelfEffects(player, permutation, effects);
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AppendOnUnusedTileEffects(player, permutation, tile, effects);
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> effects, bool withForce, bool isClone)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AppendDiscardTileEffects(player, tile, effects, withForce, isClone);
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            foreach (Artifact artifact in GetArtifacts(player)) artifact.AddOnRoundEndEffects(player, permutation, effects);
        }
    }
}

[Serializable]
[HarderBoss]
public sealed class LosslessHarderBoss : LosslessBoss
{
    protected override bool UseHarderBosses => true;

    public override Boss GetHarderBoss() => this;
}
