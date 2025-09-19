using System.Collections.Generic;

namespace Aotenjo
{
    public abstract class PirateChestReward
    {
        public static PirateChestReward CreateArtifactReward(Artifact artifact)
        {
            var lst = new List<Artifact>();
            lst.Add(artifact);
            return CreateArtifactReward(lst);
        }

        public static PirateChestReward CreateArtifactReward(List<Artifact> baseArtifacts)
        {
            return new Artifacts(baseArtifacts);
        }

        public static PirateChestReward CreateGadgetReward(Gadget artifact)
        {
            var lst = new List<Gadget>();
            lst.Add(artifact);
            return CreateGadgetReward(lst);
        }

        public static PirateChestReward CreateGadgetReward(List<Gadget> gadgets)
        {
            return new Gadgets(gadgets);
        }

        public static PirateChestReward CreateYakuPackReward(YakuPack yakuPack)
        {
            return new YakuPacks(yakuPack, 1);
        }

        public static PirateChestReward CreateYakuPacksReward(YakuPack yakuPack, int amount)
        {
            return new YakuPacks(yakuPack, amount);
        }

        public static PirateChestReward CreateMoneyReward(int money)
        {
            return new Money(money);
        }

        public static PirateChestReward CreateSlotReward(Rarity rarity)
        {
            return new Slot(rarity);
        }

        public class Artifacts : PirateChestReward
        {
            public List<Artifact> artifacts;

            public Artifacts(List<Artifact> artifacts)
            {
                this.artifacts = artifacts;
            }
        }

        public class Gadgets : PirateChestReward
        {
            public List<Gadget> gadgets;

            public Gadgets(List<Gadget> gadgets)
            {
                this.gadgets = gadgets;
            }
        }

        public class YakuPacks : PirateChestReward
        {
            public YakuPack yakuPack;
            public int amount;

            public YakuPacks(YakuPack yakuPack, int amount)
            {
                this.yakuPack = yakuPack;
                this.amount = amount;
            }
        }

        public class Money : PirateChestReward
        {
            public int amount;

            public Money(int amount)
            {
                this.amount = amount;
            }
        }

        public class Slot : PirateChestReward
        {
            public Rarity rarity;

            public Slot(Rarity rarity)
            {
                this.rarity = rarity;
            }
        }
    }
}