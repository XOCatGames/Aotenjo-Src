using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Aotenjo.Tile;

namespace Aotenjo
{
    public class ScarletCoreArtifact : Artifact
    {
        [Serializable]
        public class ScarletCoreArtifactData
        {
            public Category mainCategory = Category.Wan;
            public Category subCategory = Category.Wan;
            public Category unusedCategory = Category.Wan;

            public int wanLevel;
            public int suoLevel;
            public int bingLevel;
        }

        private const int WAN_LV1_FU_PER_TILE = 3;

        #region 基础信息

        public ScarletCoreArtifactData data;

        public ScarletCoreArtifact() : base("scarlet_core", Rarity.COMMON)
        {
            data = new ScarletCoreArtifactData();
            
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            data = new ScarletCoreArtifactData();
        }

        public override string GetName(Func<string, string> localizer)
        {
            return localizer($"{base.GetNameKey()}_level_{Math.Max(1, GetTotalLevel())}");
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            StringBuilder sb = new StringBuilder();

            int j = 1;

            for (int i = 1; i <= GetCategoryLevel(Category.Wan); i++)
            {
                sb.AppendLine($"<style=\"yellow\">{j++}.</style> " + localizer($"bonus_effect_scarlet_core_wan_{i}"));
            }

            for (int i = 1; i <= GetCategoryLevel(Category.Suo); i++)
            {
                sb.AppendLine($"<style=\"yellow\">{j++}.</style> " + localizer($"bonus_effect_scarlet_core_suo_{i}"));
            }

            for (int i = 1; i <= GetCategoryLevel(Category.Bing); i++)
            {
                sb.AppendLine($"<style=\"yellow\">{j++}.</style> " + localizer($"bonus_effect_scarlet_core_bing_{i}"));
            }

            return string.Format(base.GetDescription(player, localizer), sb);
        }

        public override Rarity GetRarity()
        {
            int totalLevel = GetTotalLevel();

            return totalLevel switch
            {
                0 => Rarity.COMMON,
                1 => Rarity.COMMON,
                2 => Rarity.COMMON,
                3 => Rarity.RARE,
                _ => Rarity.EPIC
            };
        }

        protected override int GetSpriteID()
        {
            int totalLevel = Math.Max(1, GetTotalLevel());
            return base.GetSpriteID() + totalLevel - 1;
        }

        public override bool IsAvailableInShops(Player player)
        {
            return false;
        }

        public override bool CanBeSellByPlayer()
        {
            return false;
        }

        public override string Serialize()
        {
            return JsonUtility.ToJson(data);
        }

        public override void Deserialize(string json)
        {
            data = JsonUtility.FromJson<ScarletCoreArtifactData>(json);
        }

        #endregion

        public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
        {
            base.AppendOnTileEffects(player, permutation, tile, effects);
            ScarletPlayer scarletPlayer = (ScarletPlayer)player;

            //WAN LV1
            if (tile.GetCategory() == Category.Wan &&
                scarletPlayer.GetCategoryLevel(Category.Wan) >= 1 &&
                scarletPlayer.IsCompatibleWithMainCategory(Category.Wan))
            {
                effects.Add(ScoreEffect.AddFu(WAN_LV1_FU_PER_TILE, this));
            }

            //WAN LV2
            if (tile.GetCategory() == Category.Wan &&
                scarletPlayer.GetCategoryLevel(Category.Wan) >= 2)
            {
                if (scarletPlayer.GenerateRandomDeterminationResult(5))
                    effects.Add(ScoreEffect.AddFu(40, this));
            }

            //WAN LV3
            if (tile.GetCategory() == Category.Wan &&
                scarletPlayer.GetCategoryLevel(Category.Wan) >= 3 &&
                scarletPlayer.IsCompatibleWithMainCategory(Category.Wan))
            {
                effects.Add(ScoreEffect.MulFan(1.1D, this));
            }

            //SUO LV1
            if (tile.GetCategory() == Category.Suo &&
                scarletPlayer.GetCategoryLevel(Category.Suo) >= 1 &&
                scarletPlayer.IsCompatibleWithMainCategory(Category.Suo) &&
                scarletPlayer.Selecting(tile))
            {
                effects.Add(new GrowFuEffect(this, tile, 2));
            }

            //BING LV1
            if (tile.GetCategory() == Category.Bing &&
                scarletPlayer.GetCategoryLevel(Category.Bing) >= 1 &&
                scarletPlayer.IsCompatibleWithMainCategory(Category.Bing))
            {
                if (scarletPlayer.GenerateRandomDeterminationResult(5))
                    effects.Add(ScoreEffect.AddFu(() => scarletPlayer.GetMoney(), this));
            }

            //BING LV4
            if (tile.GetCategory() == Category.Bing &&
                scarletPlayer.GetCategoryLevel(Category.Bing) >= 4 &&
                scarletPlayer.IsCompatibleWithMainCategory(Category.Bing))
            {
                effects.Add(new EarnMoneyEffect(1, this));
            }
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostAddSingleTileAnimationEffectEvent += Player_PostAddSingleTileAnimationEffectEvent;
            player.PostAddTileEvent += OnPostAddTile;
            player.PostObtainArtifactEvent += Player_PostObtainArtifactEvent;
            player.DetermineYaojiuTileEvent += Player_DetermineYaojiuTileEvent;
        }

        private void Player_DetermineYaojiuTileEvent(PlayerTileEvent tileEvent)
        {
            ScarletPlayer scarletPlayer = (ScarletPlayer)tileEvent.player;
            if (tileEvent.tile.CompactWithCategory(Category.Wan) &&
                scarletPlayer.GetCategoryLevel(Category.Wan) >= 4)
            {
                tileEvent.canceled = false;
            }
        }

        private void Player_PostObtainArtifactEvent(PlayerArtifactEvent evt)
        {
            if (evt.artifact.IsTemporary) return;
            //BING LV3
            ScarletPlayer player = (ScarletPlayer)evt.player;
            if (!(player.GetCategoryLevel(Category.Bing) >= 3)) return;
            rarity = evt.artifact.GetRarity();
            List<Artifact> list = player.DrawRandomArtifact(rarity, 5);
            list.RemoveAll(a => a.IsUnique());
            if (list.Count == 0) return;
            Artifact tempArtifact = LotteryPool<Artifact>.DrawFromCollection(list, player.GenerateRandomInt);
            tempArtifact.IsTemporary = true;
            player.ObtainArtifact(tempArtifact);
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostAddSingleTileAnimationEffectEvent -= Player_PostAddSingleTileAnimationEffectEvent;
            player.PostAddTileEvent -= OnPostAddTile;
            player.PostObtainArtifactEvent -= Player_PostObtainArtifactEvent;
            player.DetermineYaojiuTileEvent -= Player_DetermineYaojiuTileEvent;
        }

        private void OnPostAddTile(PlayerTileEvent tileEvent)
        {
            //SUO LV2
            ScarletPlayer p = (ScarletPlayer)tileEvent.player;

            if (!(tileEvent.tile.GetCategory() == Category.Suo &&
                  p.GetCategoryLevel(Category.Suo) >= 2)) return;

            if (p.GenerateRandomDeterminationResult(8))
            {
                tileEvent.tile.SetMaterial(p.GenerateRandomTileProperties(0, 0, 100, 0).material, p);
            }
        }

        private void Player_PostAddSingleTileAnimationEffectEvent(Permutation arg1, Player arg2,
            List<OnTileAnimationEffect> arg3, OnTileAnimationEffect arg4, Tile tile)
        {
            ScarletPlayer scarletPlayer = (ScarletPlayer)arg2;
            if (
                tile.GetCategory() == Category.Suo &&
                scarletPlayer.GetCategoryLevel(Category.Suo) >= 3 &&
                arg4.GetEffect() is ScoreEffect scEff && scEff.type == ScoreEffect.EffectType.ADD_FU)
            {
                arg3.Add(ScoreEffect.AddFan(() => scEff.value, this).OnTile(tile));
            }
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            ScarletPlayer p = (ScarletPlayer)player;
            if (player.Level % 4 != 0 && p.GetCategoryLevel(Category.Suo) >= 4 &&
                p.CurrentAccumulatedScore < p.levelTarget)
                effects.Add(new SimpleEffect("effect_wudi_name", this,
                    p => p.levelTarget = p.CurrentAccumulatedScore - 1));
        }

        public int GetTotalLevel()
        {
            return Math.Min(4, data.wanLevel + data.suoLevel + data.bingLevel);
        }

        public int GetCategoryLevel(Category category)
        {
            return category switch
            {
                Category.Wan => data.wanLevel,
                Category.Suo => data.suoLevel,
                Category.Bing => data.bingLevel,
                _ => throw new ArgumentException("Invalid category for ScarletCore")
            };
        }

        internal string GetCategoryBonusDescription(Category mainCategory, Func<string, string> localizer)
        {
            int level = GetCategoryLevel(mainCategory);
            if (level < 0 || level > 3) return string.Empty;
            if (mainCategory == Category.Wan) return localizer($"bonus_effect_scarlet_core_wan_{level + 1}");
            if (mainCategory == Category.Suo) return localizer($"bonus_effect_scarlet_core_suo_{level + 1}");
            if (mainCategory == Category.Bing) return localizer($"bonus_effect_scarlet_core_bing_{level + 1}");
            return "";
        }
    }
}