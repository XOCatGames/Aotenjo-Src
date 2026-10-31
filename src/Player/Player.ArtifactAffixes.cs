using Aotenjo.Console;
using UnityEngine;

namespace Aotenjo
{
    public partial class Player
    {
        [SerializeField] private SerializableMap<string, ArtifactAffix> heldArtifactAffixes = new();

        // 公开兼容入口只允许访问本局持有对象；名称、成长和行为由词条实例负责。
        private ArtifactAffixInstance GetHeldArtifactAffix(Artifact artifact) =>
            HoldsArtifact(artifact)
                ? artifact.Affix : null;

        public ArtifactAffix GetArtifactAffix(Artifact artifact) => GetHeldArtifactAffix(artifact)?.State.Builtin ?? ArtifactAffix.None;

        public bool ObtainArtifactWithAffix(Artifact artifact, ArtifactAffix affix, bool forced = false)
        {
            if (!IsArtifactAvailableForInventory(artifact) || !ArtifactAffixes.IsKnown(affix)) return false;
            var candidate = new ArtifactAffixState(affix, null);
            if (!forced && (!CanApplyArtifactAffix(artifact, candidate) ||
                !TryObtainArtifact(artifact, QueryArtifactCapacity(artifact, candidate).ArtifactSlots, affix))) return false;
            var previous = artifact.RuntimeAffix;
            artifact.RuntimeAffix = candidate;
            // 准入已发布获取前事件，统一授予入口不能再次发布或提前执行获取回调。
            if (GrantArtifact(artifact, null, true)) return true;
            artifact.RuntimeAffix = previous;
            return false;
        }

        public bool CanApplyArtifactAffix(Artifact artifact, ArtifactAffix affix) =>
            ArtifactAffixes.IsKnown(affix) && CanApplyArtifactAffix(artifact, new ArtifactAffixState(affix, null));

        // 内置与自定义词条共用库存准入；Player 只比较候选事件返回的容量贡献，不解释词条类型。
        private bool CanApplyArtifactAffix(Artifact artifact, ArtifactAffixState candidate)
        {
            if (!OwnsArtifactInstance(artifact) || IsReservedForRecipe(artifact) || HasPendingArtifactReplacement(artifact)) return false;
            var before = HoldsArtifact(artifact) ? QueryArtifactCapacity(artifact) : new ArtifactQueryEvent.Capacity(this, artifact);
            var after = QueryArtifactCapacity(artifact, candidate);
            int artifactDelta = after.ArtifactSlots - before.ArtifactSlots;
            if (artifactDelta < 0 && GetArtifacts().Count + ReservedArtifactSlots > GetArtifactLimit() + artifactDelta) return false;
            return artifact.Query(new ArtifactQueryEvent.ApplyAffix(this, artifact), candidate).Allowed;
        }

        public bool SetArtifactAffix(Artifact artifact, ArtifactAffix affix) =>
            ArtifactAffixes.IsKnown(affix) && SetArtifactAffix(artifact, new ArtifactAffixState(affix, null));

        private bool SetArtifactAffix(Artifact artifact, ArtifactAffixState candidate)
        {
            if (!HoldsArtifact(artifact) || !CanApplyArtifactAffix(artifact, candidate)) return false;
            // 同 ID 赋予保留原成长和监听；通过准入后统一提交，避免两套入口分别处理重绑与缓存。
            if ((GetHeldArtifactAffix(artifact)?.Id ?? "") == candidate.Id) return true;
            artifact.RuntimeAffix = candidate;
            RebindArtifactSubscriptions(artifact);
            ClearCachedSelectedPerm();
            return true;
        }

        [AotenjoCommand("affix", "ToArtifact", "ToString")]
        public string DebugSetArtifactAffix(Artifact artifact,
            [AotenjoCommandCompletion(typeof(ArtifactAffix))] string name)
        {
            if (!ArtifactAffixes.TryParse(name, out ArtifactAffix affix))
                return "Affix: rooted/筑根, lightweight/轻盈, gilded/金纹, lucky/幸运, unstable/不稳, none/清除";
            bool result = SetArtifactAffix(artifact, affix);
            return result ? $"{artifact.GetRegName()}: {affix}" :
                "Cannot set affix: the relic must be held, permanent and intact; check artifact capacity and pending transactions.";
        }
    }
}
