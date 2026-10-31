using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    /// <summary>
    /// 一件 Artifact 独占的词条实例。构造时只声明监听，实局绑定和解绑由宿主生命周期驱动。
    /// 状态随实例保存；静态定义只用于创建实例，不保存宿主、成长或订阅。
    /// 同步查询在实例内发布相同的查询事件，可用于尚未获取的候选对象，不临时修改实局总线。
    /// </summary>
    public abstract class ArtifactAffixInstance
    {
        public Artifact Host { get; }
        public string Id => State.Id;
        public ArtifactAffixState Snapshot => State.Copy();
        internal ArtifactAffixState State { get; }
        private readonly List<Binding> bindings = new();
        private Player subscribedPlayer;

        protected ArtifactAffixInstance(Artifact host, ArtifactAffixState state)
        {
            Host = host;
            State = state;
            // 赋予规则在候选实例上查询；清除时没有候选词条，因此不受破损、临时状态限制。
            Listen<ArtifactQueryEvent.ApplyAffix>(evt => evt.Allowed &= !Host.IsBroken && !Host.IsTemporary);
            Listen<ArtifactQueryEvent.Transfer>(evt => evt.Affix = State.Copy());
        }

        public virtual string GetName(Func<string, string> loc) => ArtifactAffixes.Name(State.Builtin, loc);
        public virtual string GetDescription(Func<string, string> loc) => ArtifactAffixes.Description(State.Builtin, loc);
        public virtual string GetData(string key, string fallback = "") => fallback;
        internal virtual bool SetData(string key, string value) => false;

        private abstract class Binding
        {
            internal abstract void Subscribe(Player player);
            internal abstract void Unsubscribe(Player player);
            internal abstract void Query(ArtifactQueryEvent evt);
        }

        private sealed class Binding<T> : Binding where T : PlayerEvent
        {
            private readonly Action<T> handler, liveHandler;
            internal Binding(ArtifactAffixInstance owner, Action<T> handler)
            {
                this.handler = handler;
                // 退订不能撤回总线已经取得的快照；旧词条、已移除宿主和已退出对局必须即时失效。
                liveHandler = evt =>
                {
                    if (owner.IsSubscribed && ReferenceEquals(evt.player, owner.subscribedPlayer) &&
                        (evt is not ArtifactQueryEvent query || ReferenceEquals(query.artifact, owner.Host))) handler(evt);
                };
            }
            internal override void Subscribe(Player player) => EventBus.Subscribe(player, liveHandler);
            internal override void Unsubscribe(Player player) => EventBus.Unsubscribe(player, liveHandler);
            internal override void Query(ArtifactQueryEvent evt)
            {
                if (evt.GetType() == typeof(T)) handler((T)(PlayerEvent)evt);
            }
        }

        protected void Listen<T>(Action<T> handler) where T : PlayerEvent => bindings.Add(new Binding<T>(this, handler));
        protected bool IsSubscribed => subscribedPlayer != null && ReferenceEquals(Host.Affix, this) &&
            subscribedPlayer.HoldsArtifact(Host);
        protected bool IsPositiveActive(Player player) => !Host.IsBroken && !Host.IsTemporary && !player.IsArtifactDebuffed(Host);
        protected bool Matches(ArtifactEffectEvent evt) => ReferenceEquals(evt.Artifact, Host) && IsPositiveActive(evt.player);

        internal void SubscribeToPlayer(Player player)
        {
            UnsubscribeFromPlayer();
            subscribedPlayer = player;
            try { foreach (var binding in bindings) binding.Subscribe(player); }
            catch { UnsubscribeFromPlayer(); throw; }
        }

        internal void UnsubscribeFromPlayer()
        {
            var player = subscribedPlayer;
            subscribedPlayer = null;
            if (player != null) foreach (var binding in bindings) binding.Unsubscribe(player);
        }

        internal void PublishQuery(ArtifactQueryEvent evt)
        {
            if (!ReferenceEquals(evt.artifact, Host)) throw new ArgumentException("词条查询必须指向自己的宿主。");
            foreach (var binding in bindings) binding.Query(evt);
        }

        internal static ArtifactAffixInstance Create(Artifact host, ArtifactAffixState state)
        {
            if (!string.IsNullOrEmpty(state.CustomId)) return new CustomArtifactAffix(host, state);
            return state.Builtin switch
            {
                ArtifactAffix.Rooted => new RootedArtifactAffix(host, state),
                ArtifactAffix.Miniature => new MiniatureArtifactAffix(host, state),
                ArtifactAffix.Gilded => new GildedArtifactAffix(host, state),
                ArtifactAffix.Lucky => new LuckyArtifactAffix(host, state),
                ArtifactAffix.Unstable => new UnstableArtifactAffix(host, state),
                _ => null
            };
        }
    }

    internal sealed class RootedArtifactAffix : ArtifactAffixInstance
    {
        internal RootedArtifactAffix(Artifact host, ArtifactAffixState state) : base(host, state)
        {
            Listen<ArtifactEffectEvent.Self>(evt =>
            {
                if (Matches(evt)) evt.Effects.Add(ScoreEffect.AddFu(30, Host));
            });
            Listen<ArtifactQueryEvent.Sale>(evt => evt.Allowed = false);
        }
    }

    internal sealed class MiniatureArtifactAffix : ArtifactAffixInstance
    {
        internal MiniatureArtifactAffix(Artifact host, ArtifactAffixState state) : base(host, state)
        {
            // 容量属于持有规则，不随压制停用；订阅和重绑本身不能增减基础栏位。
            Listen<ArtifactQueryEvent.Capacity>(evt => evt.ArtifactSlots++);
            // 在最终取值事件中修正宿主自己的番符效果，覆盖动态数值、包装效果与额外触发。
            // 乘番以 X1 为无增益基准折半；此代价不随压制解除，也不改写原效果或成长数据。
            Listen<ArtifactScoreValueEvent>(evt =>
            {
                if (ReferenceEquals(evt.artifact, Host))
                    evt.Value = evt.Type == ScoreEffect.EffectType.MUL_FAN ? 1 + (evt.Value - 1) / 2 : evt.Value / 2;
            });
        }
    }

    internal sealed class GildedArtifactAffix : ArtifactAffixInstance
    {
        internal GildedArtifactAffix(Artifact host, ArtifactAffixState state) : base(host, state) =>
            Listen<ArtifactEffectEvent.RoundEnd>(OnRoundEnd);

        private void OnRoundEnd(ArtifactEffectEvent.RoundEnd evt)
        {
            if (!Matches(evt)) return;
            // 收集只追加效果，预览不发钱；动态展开宿主时读取售价，沿用本体后的动画顺序。
            int amount = Math.Max(0, Host.GetSellingPrice());
            if (amount > 0) evt.Effects.Add(new EarnMoneyEffect(amount, Host));
        }
    }

    internal sealed class LuckyArtifactAffix : ArtifactAffixInstance
    {
        internal LuckyArtifactAffix(Artifact host, ArtifactAffixState state) : base(host, state) =>
            Listen<RarityWeightsEvent>(evt =>
            {
                if (evt.InShop && IsPositiveActive(evt.player))
                    evt.AddModifier("artifact.lucky", ArtifactAffixes.AdjustRarityWeights);
            });
    }

    internal sealed class UnstableArtifactAffix : ArtifactAffixInstance
    {
        internal UnstableArtifactAffix(Artifact host, ArtifactAffixState state) : base(host, state)
        {
            Listen<ArtifactEffectEvent.Self>(evt =>
            {
                if (Matches(evt)) evt.Effects.Add(ScoreEffect.MulFan(1.2, Host));
            });
            Listen<PlayerRoundEvent.Start.Prepare>(evt =>
            {
                if (!Host.IsBroken && !Host.IsTemporary && !evt.player.HasPendingArtifactReplacement(Host))
                    evt.Enqueue(Host, reserved => Transform(evt.player, reserved));
            });
        }

        private void Transform(Player player, HashSet<Artifact> reserved)
        {
            if (!IsSubscribed || Host.IsBroken || Host.IsTemporary || player.HasPendingArtifactReplacement(Host)) return;
            var held = player.GetArtifacts();
            int index = held.IndexOf(Host);
            if (index < 0) return;
            var candidates = player.GetArtifactPool().Where(a => !reserved.Contains(a) && !held.Contains(a) &&
                a.GetRarity() == Host.GetRarity() && !a.IsUnique() && a.IsAvailableGlobally(player) &&
                a.IsAvailableInShops(player) && a.CanObtainBy(player) &&
                !ArtifactRecipes.recipes.Any(r => r.inputID.Contains(a.GetRegName()) &&
                    r.inputID.All(id => id == a.GetRegName() || held.Any(h => h != Host && h.GetRegName() == id))))
                .ToList();
            if (candidates.Count == 0) return;
            var next = candidates[player.GenerateRandomInt(candidates.Count, "artifact_affix_transform")];
            // 本次准备先取得全部来源，再按订阅顺序执行；新宿主不会递归参加同一次准备。
            // 沿用真正的移除和获取，保留赠礼、回池和统计，只有词条快照传递给新宿主。
            var state = Snapshot;
            if (!player.RemoveArtifact(Host, true)) return;
            next.ResetArtifactState(player);
            next.RuntimeAffix = state;
            if (!player.GrantArtifact(next, null, true))
            {
                next.RuntimeAffix = new ArtifactAffixState();
                Host.RuntimeAffix = state;
                player.GrantArtifact(Host, null, true);
                return;
            }
            reserved.Add(next);
            held = player.GetArtifacts();
            held.Remove(next);
            held.Insert(Math.Min(index, held.Count), next);
            player.SetArtifactOrder(held.ToArray());
            MessageManager.Instance.OnActivateArtifactEvent(next, new TextEffect("effect_artifact_affix_unstable", next));
        }
    }

    internal sealed class CustomArtifactAffix : ArtifactAffixInstance
    {
        private CustomArtifactAffixDefinition Definition => CustomArtifactAffixes.Resolve(State.Custom);

        public override string GetName(Func<string, string> loc) => Definition is { } definition ? loc(definition.NameKey) : Id;
        public override string GetDescription(Func<string, string> loc) => Definition is { } definition
            ? definition.Description?.Invoke(Host.RuntimeOwner, loc, Host) ?? loc(definition.DescriptionKey) : Id;
        public override string GetData(string key, string fallback = "") =>
            key == null ? fallback : State.Custom.data?.Get(key) ?? fallback;

        // 实例只维护自己的成长；Player 在调用前检查库存事务，并在成功后使计分缓存失效。
        // 写入不对外暴露，避免旧实例或 Lua 绕过持有、合成和待升级限制修改状态。
        internal override bool SetData(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || Definition == null) return false;
            var data = State.Custom.data ??= new();
            if (value == null) data.Remove(key);
            else data.Add(key, value);
            return true;
        }

        internal CustomArtifactAffix(Artifact host, ArtifactAffixState state) : base(host, state)
        {
            // 继续使用原 Lua Builder 签名，但每件宿主拥有独立的监听和成长状态。
            Listen<ArtifactQueryEvent.ApplyAffix>(evt =>
            {
                var definition = Definition;
                evt.Allowed = evt.Allowed && definition != null && (definition.CanApply?.Invoke(evt.player, Host) ?? true);
            });
            Listen<ArtifactEffectEvent.Self>(evt =>
            {
                if (Matches(evt)) Definition?.OnSelf?.Invoke(evt.player, evt.Permutation, evt.Effects, Host);
            });
            Listen<ArtifactEffectEvent.Tile>(evt =>
            {
                if (Matches(evt)) Definition?.OnTile?.Invoke(evt.player, evt.Permutation, evt.Target, evt.Effects, Host);
            });
            Listen<ArtifactEffectEvent.Block>(evt =>
            {
                if (Matches(evt)) Definition?.OnBlock?.Invoke(evt.player, evt.Permutation, evt.Target, evt.Effects, Host);
            });
            Listen<ArtifactEffectEvent.RoundEnd>(evt =>
            {
                if (Matches(evt)) Definition?.OnRoundEnd?.Invoke(evt.player, evt.Permutation, evt.Effects, Host);
            });
        }
    }
}
