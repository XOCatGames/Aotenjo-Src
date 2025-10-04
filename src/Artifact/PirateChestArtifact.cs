using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class PirateChestArtifact : LevelingArtifact
    {
        public PirateChestArtifact() : base("pirate_chest", Rarity.EPIC, 0)
        {
            SetHighlightRequirement((t, _) => (t.CompactWithCategory(Tile.Category.Bing)));
        }

        public override int GetSellingPrice()
        {
            return 3;
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Level);
        }

        public override bool IsUnique()
        {
            return true;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostSkipRoundEndEvent += ConsumeSkipEvent;
        }

        private void ConsumeSkipEvent(PlayerEvent data)
        {
            if (Level < 100)
                Level += 2;
        }

        public override void AddOnSelfEffects(Player player, Permutation permutation, List<Effect> effects)
        {
            var blocks = player.GetCurrentSelectedBlocks();
            foreach (var block in blocks)
            {
                if (block.GetCategory() == Tile.Category.Bing)
                {
                    effects.Add(new UpgradeEffect(this));
                }
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostSkipRoundEndEvent -= ConsumeSkipEvent;
            if (!player.GetArtifacts().Contains(this))
            {
                List<PirateChestReward> rewards = GenerateRewards(player.GenerateRandomInt, player);
                EventManager.Instance.OnOpenPirateChest(rewards, player);
            }
        }

        private List<PirateChestReward> GenerateRewards(Func<int, int> randInt, Player player)
        {
            var lst = new List<PirateChestReward>();

            if (Level < 5)
            {
                lst.Add(PirateChestReward.CreateMoneyReward(2 + randInt(3)));
            }
            else if (Level < 10)
            {
                lst.AddRange(GenerateCommonRewards(player).Draw(player.GenerateRandomInt));
            }
            else if (Level < 15)
            {
                lst.AddRange(GenerateUncommonRewards(player).Draw(player.GenerateRandomInt));
            }
            else if (Level < 20)
            {
                lst.AddRange(GenerateRareRewards(player).Draw(player.GenerateRandomInt));
            }
            else if (Level < 30)
            {
                lst.AddRange(GenerateEpicRewards(player).Draw(player.GenerateRandomInt));
            }
            else if (Level < 50)
            {
                lst.AddRange(GenerateLegendaryRewards(player).Draw(player.GenerateRandomInt));
            }
            else if (Level < 100)
            {
                lst.AddRange(GenerateAncientRewards(player).Draw(player.GenerateRandomInt));
            }
            else
            {
                lst.AddRange(GenerateTopRewards(player).Draw(player.GenerateRandomInt));
            }

            return lst;
        }

        private LotteryPool<List<PirateChestReward>> GenerateCommonRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(GenerateUncommonRewards(player), 1);

            pool.Add(
                new List<PirateChestReward>
                    { PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.RARE, 1)) }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateGadgetReward(player.DrawRandomGadget(Rarity.COMMON, 2)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 1 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(player.GenerateRandomInt(5))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateMoneyReward(8 + player.GenerateRandomInt(3))
            }, 20);

            return pool;
        }

        private LotteryPool<List<PirateChestReward>> GenerateUncommonRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(GenerateRareRewards(player), 1);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 1)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 1 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(6 + player.GenerateRandomInt(2))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateGadgetReward(player.DrawRandomGadget(Rarity.COMMON, 2)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 4 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(10 + player.GenerateRandomInt(2))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateMoneyReward(15 + player.GenerateRandomInt(5))
            }, 20);

            pool.Add(new List<PirateChestReward>
            {
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 6 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(18 + player.GenerateRandomInt(2))
            }, 20);

            return pool;
        }

        /// <summary>
        /// 20层奖励
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private LotteryPool<List<PirateChestReward>> GenerateRareRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(GenerateEpicRewards(player), 1);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 1)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 5 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(15 + player.GenerateRandomInt(5))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateGadgetReward(player.DrawRandomGadget(Rarity.RARE, 1)),
                PirateChestReward.CreateMoneyReward(16 + player.GenerateRandomInt(5))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.RARE, 2)),
                PirateChestReward.CreateMoneyReward(14 + player.GenerateRandomInt(5))
            }, 20);

            return pool;
        }

        /// <summary>
        /// 35层奖励
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private LotteryPool<List<PirateChestReward>> GenerateEpicRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(GenerateLegendaryRewards(player), 1);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.COMMON),
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.COMMON, 1)),
                PirateChestReward.CreateMoneyReward(2 + player.GenerateRandomInt(5))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateMoneyReward(35 + player.GenerateRandomInt(20)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 2 + player.GenerateRandomInt(2)),
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 2)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 3 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(12 + player.GenerateRandomInt(10))
            }, 20);

            return pool;
        }

        /// <summary>
        /// 60层奖励
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private LotteryPool<List<PirateChestReward>> GenerateLegendaryRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(GenerateAncientRewards(player), 1);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.EPIC),
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 1)),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 5 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(24 + player.GenerateRandomInt(10))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.COMMON),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 5 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(34 + player.GenerateRandomInt(20))
            }, 30);

            pool.Add(new List<PirateChestReward>
            {
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 15 + player.GenerateRandomInt(5)),
                PirateChestReward.CreateMoneyReward(77 + player.GenerateRandomInt(20))
            }, 20);

            return pool;
        }

        /// <summary>
        /// 100层奖励
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private LotteryPool<List<PirateChestReward>> GenerateAncientRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(GenerateTopRewards(player), 1);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                PirateChestReward.CreateSlotReward(Rarity.COMMON),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 5 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateMoneyReward(55 + player.GenerateRandomInt(20))
            }, 30);

            //pool.Add(new List<PirateChestReward> {
            //    PirateChestReward.CreateSlotReward(Rarity.RARE),
            //    PirateChestReward.CreateSlotReward(Rarity.COMMON),
            //    //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 10 + player.GenerateRandomInt(5)),
            //    PirateChestReward.CreateMoneyReward(65 + player.GenerateRandomInt(20))
            //}, 30);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 5 + player.GenerateRandomInt(2)),
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 2)),
                PirateChestReward.CreateMoneyReward(25 + player.GenerateRandomInt(10))
            }, 20);

            return pool;
        }

        /// <summary>
        /// 150层奖励
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private LotteryPool<List<PirateChestReward>> GenerateTopRewards(Player player)
        {
            LotteryPool<List<PirateChestReward>> pool = new LotteryPool<List<PirateChestReward>>();

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 2)),
                PirateChestReward.CreateGadgetReward(player.DrawRandomGadget(Rarity.RARE, 2)),
                PirateChestReward.CreateMoneyReward(20 + player.GenerateRandomInt(20))
            }, 30);

            //pool.Add(new List<PirateChestReward> {
            //    PirateChestReward.CreateSlotReward(Rarity.RARE),
            //    PirateChestReward.CreateSlotReward(Rarity.COMMON),
            //    PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 2)),
            //    //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 10 + player.GenerateRandomInt(5)),
            //    PirateChestReward.CreateMoneyReward(40 + player.GenerateRandomInt(50))
            //}, 30);

            //pool.Add(new List<PirateChestReward> {
            //    PirateChestReward.CreateSlotReward(Rarity.EPIC),
            //    PirateChestReward.CreateArtifactReward(player.DrawRandomArtifact(Rarity.EPIC, 1)),
            //    PirateChestReward.CreateGadgetReward(player.DrawRandomGadget(Rarity.RARE, 2)),
            //    //PirateChestReward.CreateYakuPacksReward(yakuPacks[player.GenerateRandomInt(yakuPacks.Count)], 20 + player.GenerateRandomInt(5)),
            //    PirateChestReward.CreateMoneyReward(80 + player.GenerateRandomInt(30))
            //}, 20);

            pool.Add(new List<PirateChestReward>
            {
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                PirateChestReward.CreateSlotReward(Rarity.RARE),
                PirateChestReward.CreateSlotReward(Rarity.COMMON),
                PirateChestReward.CreateMoneyReward(80 + player.GenerateRandomInt(10))
            }, 30);

            return pool;
        }

        private class UpgradeEffect : Effect
        {
            private PirateChestArtifact artifact;

            public UpgradeEffect(PirateChestArtifact artifact)
            {
                this.artifact = artifact;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return artifact.Level >= 100 ? func("effect_pirate_chest_full") : func("effect_pirate_chest_upgrade");
            }

            public override Artifact GetEffectSource()
            {
                return artifact;
            }

            public override void Ingest(Player player)
            {
                if (artifact.Level >= 100) return;
                artifact.Level += 2;
            }
        }
    }
}