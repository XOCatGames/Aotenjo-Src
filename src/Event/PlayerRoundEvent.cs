using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PlayerRoundEvent : PlayerEvent
    {
        public PlayerRoundEvent(Player player) : base(player)
        {
        }
        
        public class Skip : PlayerRoundEvent
        {
            public Skip(Player player) : base(player)
            {
            }
            
            public class Pre : Skip
            {
                public Pre(Player player) : base(player)
                {
                }
            }

            public class Post : Skip
            {
                public Post(Player player) : base(player)
                {
                }
            }
        }
        
        public class Start : PlayerRoundEvent
        {
            public Start(Player player) : base(player)
            {
            }

            /// <summary>
            /// 正式开局广播之前的独立准备阶段。遗物替换与重新订阅须在此阶段完成，
            /// 后续 Pre 才会取得新宿主的监听快照。
            /// </summary>
            public sealed class Prepare : Start
            {
                private readonly List<(Artifact source, Action<HashSet<Artifact>> apply)> changes = new();
                internal bool HasChanges => changes.Count > 0;
                public Prepare(Player player) : base(player) { }

                // 监听按订阅顺序提交准备动作；全部收集后才执行，新增宿主不会进入本次快照。
                // 共享预留集合防止多个准备动作选择同一目标或重新选择已被替换的来源。
                internal void Enqueue(Artifact source, Action<HashSet<Artifact>> apply) => changes.Add((source, apply));
                internal void Apply()
                {
                    var reserved = new HashSet<Artifact>(changes.Select(change => change.source));
                    foreach (var change in changes) change.apply(reserved);
                }
            }
            
            public class Pre : Start
            {
                public Pre(Player player) : base(player)
                {
                }
            }
            
            public class Post : Start
            {
                public Post(Player player) : base(player)
                {
                }
            }
        }
        
        public class End : PlayerRoundEvent
        {
            public End(Player player) : base(player)
            {
            }

            public class PrePre : End
            {
                public PrePre(Player player) : base(player)
                {
                }
            }
            public class Pre : End
            {
                public Pre(Player player) : base(player)
                {
                }
            }
            
            public class PostPre : End
            {
                public PostPre(Player player) : base(player)
                {
                }
            }
            
            public class Post : End
            {
                public Post(Player player) : base(player)
                {
                }
            }
        }
    }
}
