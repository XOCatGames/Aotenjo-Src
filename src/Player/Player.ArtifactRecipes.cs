using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        [NonSerialized] private HashSet<Artifact> craftingArtifacts;
        [NonSerialized] private List<Artifact> recipeInventoryPreview;
        [NonSerialized] private bool checkingRecipeAdmission;
        [NonSerialized] private bool updatingArtifactOrder;
        [NonSerialized] private int reservedRecipeSlots;
        [NonSerialized] private Artifact prospectiveRecipeMaterial;
        [NonSerialized] private ArtifactAffixState prospectiveRecipeAffix;
        // 只在真实合成尝试时抽选；失败、重复查询与续局沿用同一材料组合已经抽中的来源。
        [SerializeField] private SerializableMap<string, string> recipeAffixChoices = new();

        private bool IsReservedForRecipe(Artifact artifact) => checkingRecipeAdmission ||
            (artifact != null && craftingArtifacts?.Contains(artifact) == true);

        // 保留既有规则：破损和临时标记由合成前的整个库存决定，不改成只扫描材料。
        private static int RecipeStatus(IEnumerable<Artifact> inventory) =>
            inventory.Any(a => !a.IsBroken && !a.IsTemporary) ? 0 : inventory.Any(a => a.IsBroken) ? 1 : 2;

        private ArtifactAffixState RecipeSourceAffix(Artifact artifact) => ReferenceEquals(artifact, prospectiveRecipeMaterial)
            ? prospectiveRecipeAffix : artifact.Query(new ArtifactQueryEvent.Transfer(this, artifact)).Affix;

        private static string RecipeAffixKey(ArtifactAffixState affix) => affix.Builtin != ArtifactAffix.None
            ? "builtin:" + affix.Builtin : "custom:" + affix.CustomId;

        // 每次预览独立查询一次材料词条，筛选、去重和存档键共用该快照。
        // 保留无词条材料和原顺序；准入回调后的配方校验会重新建立快照，不复用上次查询。
        private List<(Artifact artifact, ArtifactAffixState affix)> SnapshotRecipeAffixes(IEnumerable<Artifact> inputs) =>
            inputs.Select(a => (a, RecipeSourceAffix(a))).ToList();

        private static List<(Artifact artifact, ArtifactAffixState affix)> RecipeAffixSources(
            IEnumerable<(Artifact artifact, ArtifactAffixState affix)> inputs) => inputs
            .Where(a => a.affix.Builtin != ArtifactAffix.None || !string.IsNullOrEmpty(a.affix.CustomId))
            .GroupBy(a => RecipeAffixKey(a.affix)).Select(g => g.First()).ToList();

        private static string RecipeChoiceKey(ArtifactRecipe recipe, IEnumerable<(Artifact artifact, ArtifactAffixState affix)> inputs, bool legacy = false) =>
            (legacy ? "" : "instances:") + string.Concat(new[] { recipe.name ?? "" }.Concat(recipe.outputID).Concat(inputs.SelectMany(a =>
                new[] { legacy ? a.artifact.GetRegName() : a.artifact.InstanceId.ToString(), RecipeAffixKey(a.affix) })).Select(part => part.Length + ":" + part));

        private (Artifact artifact, ArtifactAffixState affix) FrozenRecipeAffix(string key, string legacyKey,
            List<(Artifact artifact, ArtifactAffixState affix)> sources)
        {
            if (recipeAffixChoices?.Contains(key) == true)
                return sources.FirstOrDefault(a => a.artifact.InstanceId.ToString() == recipeAffixChoices.Get(key));
            if (recipeAffixChoices?.Contains(legacyKey) != true) return default;
            // 旧档冻结结果按类型记录；首次解析时固定到实际材料，后续同名材料组不能借用该抽选。
            var chosen = sources.FirstOrDefault(a => a.artifact.GetRegName() == recipeAffixChoices.Get(legacyKey));
            if (chosen.artifact != null)
            {
                recipeAffixChoices.Add(key, chosen.artifact.InstanceId.ToString());
                recipeAffixChoices.Remove(legacyKey);
            }
            return chosen;
        }

        private List<Artifact> SelectRecipeInputs(ArtifactRecipe recipe, Artifact incoming = null)
        {
            var candidates = GetArtifacts();
            if (incoming != null && !candidates.Contains(incoming)) candidates.Add(incoming);
            var inputs = new List<Artifact>();
            foreach (string id in recipe.inputID)
            {
                var material = candidates.FirstOrDefault(a => a.GetRegName() == id);
                if (material == null) return null;
                inputs.Add(material);
                candidates.Remove(material);
            }
            return inputs;
        }

        private List<Artifact> SelectRecipeOutputs(ArtifactRecipe recipe, bool preview)
        {
            var outputs = new List<Artifact>();
            foreach (string id in recipe.outputID)
            {
                var output = orderedRunArtifacts.FirstOrDefault(a => a.GetRegName() == id &&
                    IsArtifactAvailableForInventory(a) && !outputs.Contains(a));
                // 预判只构造展示对象，不分配编号、不预初始化，也不写入运行目录。
                outputs.Add(output ?? (preview ? ArtifactRegistry.CreatePreview(id) : CreateArtifactInstance(id)));
            }
            return outputs;
        }

        private bool FitsRecipeInventory(List<Artifact> inputs, List<Artifact> outputs, ArtifactAffixState affix, Artifact incoming = null)
        {
            var inventory = GetArtifacts();
            if (incoming != null && !inventory.Contains(incoming)) inventory.Add(incoming);
            // 对变化后的候选库存发布容量查询，规则来自各实例词条。
            // 产物尚未获取，材料也可能已退订；查询不能借用实局订阅或临时写入真实库存。
            var capacities = inventory.Except(inputs).Select(a => QueryArtifactCapacity(a, RecipeSourceAffix(a)))
                .Concat(outputs.Select(a => QueryArtifactCapacity(a, affix))).ToList();
            int temporaryInputs = inputs.Count(a => a.IsTemporary && !a.status.temporaryOccupiesSlot);
            int temporaryOutputs = RecipeStatus(inventory) == 2 ? outputs.Count(a => !a.status.temporaryOccupiesSlot) : 0;
            int incomingTemporary = incoming != null && incoming.IsTemporary && !incoming.status.temporaryOccupiesSlot ? 1 : 0;
            int artifactLimit = Math.Max(0, properties.ArtifactLimit + incomingTemporary - temporaryInputs + temporaryOutputs + capacities.Sum(c => c.ArtifactSlots));
            return inventory.Count - inputs.Count + outputs.Count + ReservedArtifactSlots <= artifactLimit &&
                ProjectGadgetInventory().Count <= GetGadgetLimit();
        }

        public bool CanCraftWithArtifactAffixes(ArtifactRecipe recipe, bool requireEveryChoice = false, Artifact incoming = null)
        {
            if (recipe == null || !recipe.HasValidStructure || recipe.outputID.Any(id => !ArtifactRegistry.TryGetDefinition(id, out _))) return false;
            if (incoming != null && !IsArtifactAvailableForInventory(incoming)) incoming = null;
            var inputs = SelectRecipeInputs(recipe, incoming);
            if (inputs == null || inputs.Any(HasPendingArtifactReplacement)) return false;
            var outputs = SelectRecipeOutputs(recipe, true);
            var snapshots = SnapshotRecipeAffixes(inputs);
            var sources = RecipeAffixSources(snapshots);
            var frozen = FrozenRecipeAffix(RecipeChoiceKey(recipe, snapshots), RecipeChoiceKey(recipe, snapshots, true), sources);
            if (frozen.artifact != null) return FitsRecipeInventory(inputs, outputs, frozen.affix, incoming);
            if (sources.Count == 0) return FitsRecipeInventory(inputs, outputs, new ArtifactAffixState(), incoming);
            // 查询不抽随机数。普通尝试按实际抽中的词条准入；购买最后一件材料时，
            // 只有每种候选结果都放得下才豁免购买栏位，避免先买到超限库存再赌合成结果。
            return requireEveryChoice ? sources.All(a => FitsRecipeInventory(inputs, outputs, a.affix, incoming))
                : sources.Any(a => FitsRecipeInventory(inputs, outputs, a.affix, incoming));
        }

        private bool IgnoresArtifactSlotLimit(Artifact artifact, ArtifactAffix? offeredAffix)
        {
            if (!offeredAffix.HasValue) return artifact.IsTemporary || artifact.CanBeBoughtWithoutSlotLimit(this);
            var previousMaterial = prospectiveRecipeMaterial;
            var previousAffix = prospectiveRecipeAffix;
            prospectiveRecipeMaterial = artifact;
            prospectiveRecipeAffix = new ArtifactAffixState(offeredAffix.Value, null);
            // 购买预判包含最后一件材料的待赋予词条，但不能提前写入运行对象或抽样。
            try { return artifact.IsTemporary || artifact.CanBeBoughtWithoutSlotLimit(this); }
            finally { prospectiveRecipeMaterial = previousMaterial; prospectiveRecipeAffix = previousAffix; }
        }

        internal bool TryCraftArtifactRecipe(ArtifactRecipe recipe)
        {
            if (craftingArtifacts != null || !recipe.CheckFulfillRecipeRequirement(this)) return false;
            var inventory = GetArtifacts();
            var inputs = SelectRecipeInputs(recipe);
            var outputs = SelectRecipeOutputs(recipe, false);
            var snapshots = SnapshotRecipeAffixes(inputs);
            var sources = RecipeAffixSources(snapshots);
            string choiceKey = RecipeChoiceKey(recipe, snapshots);
            var chosen = FrozenRecipeAffix(choiceKey, RecipeChoiceKey(recipe, snapshots, true), sources);
            if (chosen.artifact == null && sources.Count > 0)
            {
                chosen = sources[sources.Count == 1 ? 0 : GenerateRandomInt(sources.Count, "artifact_recipe_affix")];
                (recipeAffixChoices ??= new()).Add(choiceKey, chosen.artifact.InstanceId.ToString());
            }
            // 相同词条只参加一次抽样；同 ID 不同成长按配方材料顺序取首个来源，深复制且不叠加。
            var inheritedAffix = chosen.affix ?? new ArtifactAffixState();
            if (!FitsRecipeInventory(inputs, outputs, inheritedAffix)) return false;
            int flag = RecipeStatus(inventory);
            int temporarySlots = inputs.Count(a => a.IsTemporary && !a.status.temporaryOccupiesSlot);
            craftingArtifacts = new HashSet<Artifact>(inputs.Concat(outputs));
            try
            {
                // 先用消费材料后的只读库存视图检查全部产物。扩音器等互斥遗物必须看到材料已腾出；
                // 此时不移除材料、不触发生命周期，也不覆盖产物的原始状态。预约与获取前事件都能拒绝。
                checkingRecipeAdmission = true;
                recipeInventoryPreview = inventory.Except(inputs).ToList();
                int extraSlots = -temporarySlots;
                foreach (var output in outputs)
                {
                    if (!IsArtifactAvailableForInventory(output)) return false;
                    var originalStatus = output.status;
                    var originalAffix = output.RuntimeAffix;
                    output.RuntimeAffix = inheritedAffix.Copy();
                    output.status = new Artifact.ArtifactStatus
                    {
                        isBroken = flag == 1, isTemporary = flag == 2,
                        temporaryOccupiesSlot = originalStatus.temporaryOccupiesSlot,
                        isDebuffed = originalStatus.isDebuffed
                    };
                    try
                    {
                        extraSlots += QueryArtifactCapacity(output).ArtifactSlots;
                        if (!TryObtainArtifact(output, extraSlots)) return false;
                        if (output.IsTemporary && !output.status.temporaryOccupiesSlot) extraSlots++;
                    }
                    finally { output.status = originalStatus; output.RuntimeAffix = originalAffix; }
                    recipeInventoryPreview.Add(output);
                }
                recipeInventoryPreview = null;
                checkingRecipeAdmission = false;

                // 获取前事件返回后再核对容量和唯一性。材料、产物在整个事务中被保留，
                // 回调不能重入消费材料或抢先授予／预约同一产物；失败至此都尚未消耗材料。
                if (!recipe.CheckFulfillRecipeRequirement(this) || !inventory.SequenceEqual(heldArtifacts) ||
                    RecipeStatus(inventory) != flag || outputs.Any(a => !IsArtifactAvailableForInventory(a)) ||
                    !FitsRecipeInventory(inputs, outputs, inheritedAffix)) return false;
                reservedRecipeSlots = outputs.Count(a => flag != 2 || a.status.temporaryOccupiesSlot);

                // 从此只执行已经准入的移除和授予，不再调用可能返回拒绝的公开获取入口。
                // 保留原有移除、重置、获取及反馈顺序；预留产物栏位防止移除回调抢占腾出的空间。
                foreach (var input in inputs) RemoveArtifactCore(input, true, false);
                foreach (var output in outputs)
                {
                    output.IsBroken = flag == 1;
                    output.IsTemporary = flag == 2;
                    output.RuntimeAffix = inheritedAffix.Copy();
                    if (!output.IsTemporary || output.status.temporaryOccupiesSlot) reservedRecipeSlots--;
                    GrantArtifactCore(output, null);
                }
                recipeAffixChoices?.Remove(choiceKey);
            }
            finally
            {
                recipeInventoryPreview = null;
                checkingRecipeAdmission = false;
                reservedRecipeSlots = 0;
                craftingArtifacts = null;
            }
            OnArtifactOrderChanged();
            return true;
        }
    }
}
