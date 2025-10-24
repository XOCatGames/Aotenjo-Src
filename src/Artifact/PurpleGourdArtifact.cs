using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Aotenjo
{
    public class PurpleGourdArtifact : Artifact
    {
        [Serializable]
        public class PurpleGourdData
        {
            public List<string> consumedBosses = new List<string>();
        }
        
        public List<Artifact> bossArtifacts = new List<Artifact>();
        
        private class ConsumeBossEffect : Effect
        {
            private readonly Boss playerCurrentBoss;
            private readonly PurpleGourdArtifact purpleGourdArtifact;

            public ConsumeBossEffect(Boss playerCurrentBoss, PurpleGourdArtifact purpleGourdArtifact)
            {
                this.playerCurrentBoss = playerCurrentBoss;
                this.purpleGourdArtifact = purpleGourdArtifact;
            }


            public override string GetEffectDisplay(Player player, Func<string, string> func)
            {
                return string.Format(func("effect_purple_gourd_consume_boss_format"), 
                    func(playerCurrentBoss.GetName(player, func)));
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return "ERR";
            }

            public override Artifact GetEffectSource()
            {
                return purpleGourdArtifact;
            }

            public override void Ingest(Player player)
            {
                purpleGourdArtifact.ConsumeBoss(playerCurrentBoss, player);
            }
        }

        public PurpleGourdData data;

        public PurpleGourdArtifact() : base("purple_gourd", Rarity.EPIC)
        {
            data = new PurpleGourdData();
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            // 基础描述部分
            string desc = localizer("artifact_cannot_sell") + "\n" +
                          base.GetDescription(player,localizer) + "\n\n";

            // 已吸收 Boss
            if (data.consumedBosses.Count is > 0 and <= 5)
            {
                int index = 1;
                for (var i = 0; i < data.consumedBosses.Count; i++)
                {
                    string bossName = data.consumedBosses[i];
                    Boss boss = Bosses.GetBossOrElseRedraw(bossName, player.ascensionLevel >= 8);

                    // 名字
                    string bossLine = $"{index}. <style=\"red\">{boss.GetName(player, localizer)}</style>\n";

                    // 效果（boss 的反转 Artifact 描述）
                    string effectLine = "- " + bossArtifacts[i].GetDescription(player, localizer) +"\n";

                    desc += bossLine + effectLine + "\n";
                    index++;
                }
            }
            else if (data.consumedBosses.Count > 5)
            {
                int index = 1;
                for (var i = 0; i < data.consumedBosses.Count; i++)
                {
                    string bossName = data.consumedBosses[i];
                    Boss boss = Bosses.GetBossOrElseRedraw(bossName, player.ascensionLevel >= 8);

                    // 名字
                    string bossLine = $"<link=\"boss_reversed_effect_{boss.name}\"><style=\"red\">{boss.GetName(player, localizer)}</style></link>";
                    desc += bossLine;
                    if(i != data.consumedBosses.Count - 1)
                        desc += ", ";
                    index++;
                }
            }

            return desc.TrimEnd();
        }


        public override void AppendOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            base.AppendOnSelfEffects(player, permutation, effects);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.AppendOnSelfEffects(player, permutation, effects);
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            if ((player.CurrentAccumulatedScore + 1) >= player.levelTarget && player.Level % 4 == 0 && player.currentBoss != null && (player.currentBoss.name != Bosses.Timeless.name || !
                    ((TimelessBoss)Bosses.Timeless).firstTime))
            {
                effects.Add(new ConsumeBossEffect(player.currentBoss, this));
            }
        }
        
        private Artifact[] GetConsumedBossArtifacts(Player player)
        {
            return bossArtifacts.ToArray();
        }

        public override void AppendOnUnusedTileEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendOnUnusedTileEffects(player, perm, tile, effects);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.AppendOnUnusedTileEffects(player, perm, tile, effects);
            }
        }

        public override bool ShouldHighlightTile(Tile tile, Player player)
        {
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                if (bossArtifact.ShouldHighlightTile(tile, player))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool CanBeSellByPlayer()
        {
            return false;
        }

        public override bool IsUnique()
        {
            return true;
        }

        public override void ResetArtifactState(Player player)
        {
            base.ResetArtifactState(player);
            
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.ResetArtifactState(player);
            }
            data.consumedBosses.Clear();
            bossArtifacts.Clear();
        }

        #region 原生函数


        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.SubscribeToPlayer(player);
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.UnsubscribeToPlayer(player);
            }
        }

        public override void AppendDiscardTileEffects(Player player, Tile tile, List<IAnimationEffect> onDiscardTileEffects, bool withForce, bool isClone)
        {
            base.AppendDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.AppendDiscardTileEffects(player, tile, onDiscardTileEffects, withForce, isClone);
            }
        }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(data);
        }

        public override void Deserialize(string json)
        {
            data = JsonConvert.DeserializeObject<PurpleGourdData>(json) ?? new PurpleGourdData();
            bossArtifacts = data.consumedBosses.Select(boss => Bosses.GetBossOrElseRedraw(boss, false).GetReversedArtifact(this)).ToList();
        }

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                try
                {
                    bossArtifact.AppendOnTileEffects(player, permutation, tile, effects);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        public override void AddOnTileEffectsPostEvents(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AddOnTileEffectsPostEvents(player, permutation, tile, effects);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.AddOnTileEffectsPostEvents(player, permutation, tile, effects);
            }
        }

        public override void AddOnBlockEffects(Player player, Permutation permutation, Block block, List<Effect> effects)
        {
            base.AddOnBlockEffects(player, permutation, block, effects);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.AddOnBlockEffects(player, permutation, block, effects);
            }
        }

        public override void AppendPostBlockAnimationEffects(Player player, Permutation permutation, Block block, List<IAnimationEffect> effects)
        {
            base.AppendPostBlockAnimationEffects(player, permutation, block, effects);
            foreach (var bossArtifact in GetConsumedBossArtifacts(player))
            {
                bossArtifact.AppendPostBlockAnimationEffects(player, permutation, block, effects);
            }
        }
        

        #endregion

        public void ConsumeBoss(Boss playerCurrentBoss, Player player)
        {
            if (data.consumedBosses.Contains(playerCurrentBoss.name)) return;
            data.consumedBosses.Add(playerCurrentBoss.name);
            var reversedArtifact = playerCurrentBoss.GetReversedArtifact(this);
            bossArtifacts.Add(reversedArtifact);
            reversedArtifact.SubscribeToPlayer(player);
        }
    }

    
}